package com.sk2lb.astrogateway.poc

import android.content.Context
import android.content.Intent
import android.net.Uri
import io.github.jan.supabase.createSupabaseClient
import io.github.jan.supabase.auth.Auth
import io.github.jan.supabase.auth.auth
import io.github.jan.supabase.auth.FlowType
import io.github.jan.supabase.auth.providers.builtin.Email
import io.github.jan.supabase.auth.status.SessionStatus
import io.github.jan.supabase.exceptions.RestException
import io.github.jan.supabase.logging.LogLevel
import io.ktor.client.engine.okhttp.OkHttp
import kotlinx.coroutines.*
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import kotlinx.serialization.json.*
import java.net.HttpURLConnection
import java.net.URL
import java.util.UUID

// Profile values come from a user-scoped RLS query; identity is always Supabase auth.users.id.
data class AstronautProfile(val userId: String, val displayName: String, val traineeProfile: String, val trainingLevel: Int, val createdAt: String)
data class AuthUi(val ready: Boolean=false, val busy: Boolean=false, val profile: AstronautProfile?=null, val message: String="Restoring your secure session…", val recovery: Boolean=false, val accessExpiresAtMillis: Long=0L) {
    val authenticated get() = profile != null && !recovery && accessExpiresAtMillis>System.currentTimeMillis()
}
data class MissionContext(val sessionId: String, val userId: String, val launchId: String, val missionId: String, val expiresAtMillis: Long)
class SafeAccountError(val uiMessage: String) : Exception()

/** One application-scoped SDK client in the native host; Unity never constructs this object. */
class AuthCoordinator private constructor(private val context: Context) {
    private val scope = CoroutineScope(SupervisorJob() + Dispatchers.Main.immediate)
    private val operation = Mutex()
    private val vault = SecureSessionStore(context)
    private val mutable = MutableStateFlow(AuthUi())
    val state: StateFlow<AuthUi> = mutable.asStateFlow()
    private var signingOut = false
    private var epoch = 0
    private var validation: Job? = null
    private val configured = BuildConfig.SUPABASE_URL.isNotBlank() && BuildConfig.SUPABASE_PUBLISHABLE_KEY.isNotBlank()
    private val client = if (configured) createSupabaseClient(BuildConfig.SUPABASE_URL, BuildConfig.SUPABASE_PUBLISHABLE_KEY) {
        defaultLogLevel = LogLevel.NONE
        httpEngine = OkHttp.create { config { followRedirects(false); followSslRedirects(false) } }
        install(Auth) {
            sessionManager = vault; codeVerifierCache = vault
            autoLoadFromStorage = true; autoSaveToStorage = true; alwaysAutoRefresh = false
            flowType = FlowType.PKCE; scheme = "astrogateway"; host = "auth"
        }
    } else null
    init {
        if (client == null) mutable.value = AuthUi(ready=true, message="Account service is not configured. Add the existing Supabase project URL and publishable key before using accounts.")
        else scope.launch {
            client.auth.sessionStatus.collect { status ->
                if (signingOut) return@collect
                when (status) {
                    is SessionStatus.Authenticated -> {
                        if(mutable.value.recovery || vault.pendingFlow()=="recovery_active") { mutable.value=AuthUi(ready=true,recovery=true,message="Choose a new password to finish account recovery."); return@collect }
                        if (mutable.value.profile?.userId == status.session.user?.id && mutable.value.profile != null) mutable.value=mutable.value.copy(accessExpiresAtMillis=status.session.expiresAt.toEpochMilliseconds())
                        if (mutable.value.profile?.userId != status.session.user?.id || mutable.value.profile == null) { if(mutable.value.profile != null) { revokeMission(); mutable.value=mutable.value.copy(profile=null) }; validateSession() }
                    }
                    is SessionStatus.NotAuthenticated -> {
                        if (mutable.value.profile != null) revokeMission()
                        mutable.value = mutable.value.copy(ready=true, busy=false, profile=null)
                    }
                    is SessionStatus.RefreshFailure -> {
                        revokeMission(); mutable.value = AuthUi(ready=true, message="Session refresh failed. Reconnect and retry session validation, or sign in again.")
                    }
                    SessionStatus.Initializing -> mutable.value = AuthUi(busy=true)
                }
            }
        }
    }
    private val refreshLoop = scope.launch {
        while(isActive) {
            delay(30_000)
            if(client==null || signingOut) continue
            val session=client.auth.currentSessionOrNull() ?: continue
            if(session.expiresAt.toEpochMilliseconds()>System.currentTimeMillis()+60_000) continue
            operation.withLock {
                if(signingOut) return@withLock
                try {
                    val current=client.auth.currentSessionOrNull() ?: return@withLock
                    if(current.expiresAt.toEpochMilliseconds()<=System.currentTimeMillis()+60_000) client.auth.refreshCurrentSession()
                } catch(cancelled:CancellationException) { throw cancelled }
                catch(error:Exception) {
                    revokeMission()
                    if(error is RestException && error.statusCode in listOf(400,401,403)) client.auth.clearSession()
                    mutable.value=AuthUi(ready=true,message="Session refresh failed. Reconnect and retry validation, or sign in again.")
                }
            }
        }
    }
    private fun requiredClient() = client ?: throw SafeAccountError("Account service is not configured. No registration or sign-in request was sent.")
    private fun revokeMission() { context.sendBroadcast(Intent("${context.packageName}.REVOKE_MISSION").setPackage(context.packageName)) }
    private fun safeError(error: Throwable): String = when (error) {
        is SafeAccountError -> error.uiMessage
        is RestException -> when (error.statusCode) {
            429 -> "Too many requests. Please wait and try again."
            400, 401, 403 -> "Account request rejected. Check your credentials and email verification, then try again."
            else -> "Account or profile service is unavailable. Check project configuration and retry."
        }
        else -> "Could not reach the account service. Check your connection and retry."
    }
    private suspend fun verifyNow(): AstronautProfile {
        val c = requiredClient(); c.auth.awaitInitialization()
        val session = c.auth.currentSessionOrNull() ?: throw SafeAccountError("Please sign in to continue.")
        if (session.expiresAt.toEpochMilliseconds() <= System.currentTimeMillis() + 30_000) c.auth.refreshCurrentSession()
        val user = c.auth.retrieveUserForCurrentSession(updateSession=false)
        if (user.emailConfirmedAt == null) throw SafeAccountError("Verify your email, then sign in again.")
        UUID.fromString(user.id)
        val row = backend("GET", "/v1/me")
        if(row.getString("user_id")!=user.id) throw SafeAccountError("Profile ownership check failed.")
        return AstronautProfile(user.id,row.getString("display_name"),row.getString("trainee_profile"),row.getInt("training_level"),row.getString("created_at"))
    }
    private fun validateSession() {
        validation?.cancel(); val generation = epoch
        validation = scope.launch { operation.withLock {
            mutable.value = mutable.value.copy(busy=true)
            try {
                val profile = verifyNow()
                if (generation == epoch && !signingOut) mutable.value = mutable.value.copy(ready=true,busy=false,profile=profile,message="Signed in securely.",accessExpiresAtMillis=client!!.auth.currentSessionOrNull()!!.expiresAt.toEpochMilliseconds())
            } catch (cancelled: CancellationException) { throw cancelled }
            catch (error: Exception) {
                if (generation == epoch && !signingOut) {
                    if (error is RestException && error.statusCode in listOf(401,403)) client?.auth?.clearSession()
                    revokeMission(); mutable.value = AuthUi(ready=true,message=safeError(error))
                }
            }
        } }
    }
    fun retrySession() { if(client != null) validateSession() else mutable.value=AuthUi(ready=true,message="Account service is not configured. No session can be validated.") }
    private fun runAccount(task: suspend () -> Unit) {
        if (mutable.value.busy) return
        scope.launch { operation.withLock {
            mutable.value = mutable.value.copy(busy=true,message="Contacting account service…")
            try { task() } catch (cancelled: CancellationException) { throw cancelled }
            catch (error: Exception) { mutable.value = mutable.value.copy(ready=true,busy=false,message=safeError(error)) }
        } }
    }
    fun signUp(name: String, email: String, password: String) = runAccount {
        val c=requiredClient(); vault.allowSessionWrites(); vault.setPendingFlow("verification")
        c.auth.signUpWith(Email, redirectUrl=CALLBACK) { this.email=email; this.password=password; data=buildJsonObject { put("display_name",name) } }
        if(c.auth.currentSessionOrNull()==null) mutable.value=AuthUi(ready=true,message="If registration can proceed, check your inbox to verify your email. Then return here and sign in.")
        else validateSession()
    }
    fun signIn(email: String, password: String) = runAccount {
        val c=requiredClient(); vault.allowSessionWrites(); c.auth.signInWith(Email) { this.email=email; this.password=password }; vault.clearPendingFlow(); mutable.value=mutable.value.copy(recovery=false); validateSession()
    }
    fun resetRequest(email: String) = runAccount {
        val c=requiredClient(); vault.setPendingFlow("recovery"); c.auth.resetPasswordForEmail(email,redirectUrl=CALLBACK)
        mutable.value=mutable.value.copy(ready=true,busy=false,message="If this address has an account, a recovery email has been requested. Open it on this device.")
    }
    fun completeCallback(uri: Uri) = runAccount {
        if(uri.scheme!="astrogateway" || uri.host!="auth" || uri.path!="/callback" || uri.fragment!=null) throw SafeAccountError("Unsupported verification link. Return to sign-in.")
        val code=uri.getQueryParameter("code")?.takeIf { it.length in 1..2048 } ?: throw SafeAccountError("Verification link is missing or expired. Request a new email.")
        val pending=vault.pendingFlow() ?: throw SafeAccountError("This link has no pending request on this device. Verify in your browser and sign in, or request a new recovery email here.")
        if(vault.loadCodeVerifier()==null) throw SafeAccountError("Verification request expired. Request a new email on this device.")
        val c=requiredClient(); vault.allowSessionWrites(); mutable.value=mutable.value.copy(recovery=pending=="recovery")
        c.auth.exchangeCodeForSession(code); if(pending=="recovery") vault.setPendingFlow("recovery_active") else vault.clearPendingFlow()
        if(pending=="recovery") mutable.value=mutable.value.copy(ready=true,busy=false,message="Choose a new password to finish account recovery.",recovery=true)
        else validateSession()
    }
    fun changeRecoveredPassword(password: String) = runAccount {
        if(!mutable.value.recovery) throw SafeAccountError("Start a password recovery request first.")
        requiredClient().auth.updateUser { this.password=password }; vault.clearPendingFlow(); mutable.value=mutable.value.copy(recovery=false); validateSession()
    }
    fun signOut() {
        if(signingOut) return
        signingOut=true; epoch++; validation?.cancel(); revokeMission()
        mutable.value=AuthUi(ready=true,busy=true,message="Signing out and clearing this device’s session…")
        scope.launch { operation.withLock {
            vault.blockAndClearSession()
            val hasSession=client?.auth?.currentSessionOrNull()!=null
            var trainingRevoked=!hasSession
            var remoteRevoked=!hasSession
            if(hasSession && BuildConfig.AUTH_BACKEND_URL.isNotBlank()) {
                try { backend("POST","/v1/auth-session/revoke",org.json.JSONObject()); trainingRevoked=true } catch (_:Exception) { }
            }
            try { client?.auth?.signOut(); remoteRevoked=true } catch (_: Exception) { remoteRevoked=false }
            finally {
                client?.auth?.clearSession(); vault.deleteSession(); vault.clearPendingFlow()
                signingOut=false
                mutable.value=AuthUi(ready=true,message=if(remoteRevoked && trainingRevoked) "Signed out. Local and training sessions cleared." else "Local session cleared. Server revocation could not be confirmed while offline; existing access tokens expire according to the project policy.")
            }
        } }
    }
    suspend fun initializeMission(launchId: String): MissionContext = operation.withLock {
        if(!state.value.authenticated || signingOut) throw SafeAccountError("Sign in before launching a mission.")
        val generation=epoch; val profile=verifyNow()
        val data=backend("POST","/v1/mission-sessions",JSONObjectBuilder("launch_id" to launchId,"mission_id" to "mission-red-horizon-poc"))
        if(generation!=epoch || signingOut || profile.userId!=state.value.profile?.userId) throw SafeAccountError("Your session changed. Sign in and retry.")
        require(data.getString("user_id")==profile.userId && data.getString("launch_id")==launchId && data.getString("mission_id")=="mission-red-horizon-poc")
        val expiry=java.time.Instant.parse(data.getString("expires_at")).toEpochMilli()
        return@withLock MissionContext(data.getString("id"),profile.userId,launchId,"mission-red-horizon-poc",expiry)
    }
    suspend fun submitIntegrationResult(sessionId: String, launchId: String) {
        operation.withLock {
            if(!state.value.authenticated || signingOut) throw SafeAccountError("Sign in to submit the integration result.")
            backend("POST","/v1/mission-sessions/$sessionId/complete",JSONObjectBuilder("launch_id" to launchId,"outcome" to "integration_check"))
        }
    }
    private fun JSONObjectBuilder(vararg pairs: Pair<String,String>) = org.json.JSONObject().apply { pairs.forEach { put(it.first,it.second) } }
    private suspend fun backend(method: String,path: String,body: org.json.JSONObject?=null): org.json.JSONObject {
        if(BuildConfig.AUTH_BACKEND_URL.isBlank()) throw SafeAccountError("Private profile/mission API is not configured. No protected data or mission was opened.")
        val c=requiredClient(); val session=c.auth.currentSessionOrNull() ?: throw SafeAccountError("Session expired. Sign in again.")
        if(session.expiresAt.toEpochMilliseconds()<=System.currentTimeMillis()+30_000) c.auth.refreshCurrentSession()
        val token=c.auth.currentAccessTokenOrNull() ?: throw SafeAccountError("Session expired. Sign in again.")
        return withContext(Dispatchers.IO) {
            val connection=URL(BuildConfig.AUTH_BACKEND_URL.trimEnd('/')+path).openConnection() as HttpURLConnection
            try {
                connection.instanceFollowRedirects=false; connection.connectTimeout=10000; connection.readTimeout=10000; connection.requestMethod=method
                connection.setRequestProperty("Authorization","Bearer $token"); connection.setRequestProperty("Content-Type","application/json")
                if(body!=null) { connection.doOutput=true; connection.outputStream.use { it.write(body.toString().toByteArray()) } }
                when(connection.responseCode) {
                    200,201 -> { val bytes=connection.inputStream.use { input -> val output=java.io.ByteArrayOutputStream(); val buffer=ByteArray(4096); var count=input.read(buffer); while(count!=-1) { output.write(buffer,0,count); if(output.size()>32768) break; count=input.read(buffer) }; output.toByteArray() }; if(bytes.size>32768) throw SafeAccountError("Invalid mission service response."); org.json.JSONObject(bytes.toString(Charsets.UTF_8)) }
                    401,403 -> throw SafeAccountError("Mission authorization failed. Validate your session and retry.")
                    409 -> throw SafeAccountError("Mission session is expired or already closed. Start a new session.")
                    else -> throw SafeAccountError("Mission validation service is unavailable. Retry when connected.")
                }
            } finally { connection.disconnect() }
        }
    }
    companion object {
        const val CALLBACK="astrogateway://auth/callback"
        @Volatile private var instance: AuthCoordinator?=null
        fun get(context: Context): AuthCoordinator = instance ?: synchronized(this) { instance ?: AuthCoordinator(context.applicationContext).also { instance=it } }
    }
}
