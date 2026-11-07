package com.sk2lb.astrogateway.poc

import android.content.Context
import android.content.Intent
import android.net.Uri
import kotlinx.coroutines.*
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import org.json.JSONObject
import java.net.HttpURLConnection
import java.net.URL
import java.time.Instant
import java.util.UUID

/** MongoDB is reached through FastAPI only. Opaque sessions are encrypted by the existing vault. */
class MongoAuthCoordinator private constructor(private val context: Context) : AccountService {
    private val scope = CoroutineScope(SupervisorJob() + Dispatchers.Main.immediate)
    private val operation = Mutex()
    private val vault = SecureSessionStore(context, "mongodb")
    private val mutable = MutableStateFlow(AuthUi())
    override val state: StateFlow<AuthUi> = mutable.asStateFlow()
    private var token = ""
    private var owner = ""
    private var expires = 0L
    private var epoch = 0
    init {
        scope.launch { operation.withLock {
            try {
                vault.loadLocalAccount()?.let { raw ->
                    val saved = JSONObject(raw)
                    token = saved.getString("token"); owner = saved.getString("owner"); expires = saved.getLong("expires")
                }
                if (token.isBlank() || expires <= System.currentTimeMillis()) {
                    clear(); mutable.value = AuthUi(ready=true,message="Local MongoDB account. Sign in or create an account.")
                } else mutable.value = AuthUi(ready=true,profile=verifyNow(),accessExpiresAtMillis=expires,message="Local account restored.")
            } catch (_:Exception) { clear(); mutable.value = AuthUi(ready=true,message="Local account service unavailable, or session expired. Start the local API and sign in.") }
        } }
        scope.launch {
            while (isActive) {
                delay(1000)
                if (expires != 0L && expires <= System.currentTimeMillis()) {
                    epoch++; revokeMission(); operation.withLock { clear(); mutable.value = AuthUi(ready=true,message="Session expired. Sign in again.") }
                }
            }
        }
    }
    private fun revokeMission() { context.sendBroadcast(Intent("${context.packageName}.REVOKE_MISSION").setPackage(context.packageName)) }
    private suspend fun clear() { token=""; owner=""; expires=0L; vault.deleteSession() }
    private fun account(task: suspend (Int) -> Unit) {
        if (mutable.value.busy) return
        val generation = epoch
        scope.launch { operation.withLock {
            mutable.value = mutable.value.copy(busy=true,message="Contacting local account service…")
            try { task(generation) }
            catch (cancelled:CancellationException) { throw cancelled }
            catch (error:Exception) {
                if (generation==epoch) mutable.value = mutable.value.copy(ready=true,busy=false,message=(error as? SafeAccountError)?.uiMessage ?: "Local account service unavailable. Check the API address and retry.")
            }
        } }
    }
    private fun profile(row:JSONObject): AstronautProfile {
        val id=row.getString("user_id"); UUID.fromString(id)
        if (owner.isNotBlank() && owner!=id) throw SafeAccountError("Profile ownership check failed.")
        require(row.getString("trainee_profile")=="astronaut_candidate" && row.getInt("training_level") in 0..10)
        return AstronautProfile(id,row.getString("display_name"),row.getString("trainee_profile"),row.getInt("training_level"),row.getString("created_at"))
    }
    private suspend fun verifyNow():AstronautProfile {
        if (token.isBlank() || expires<=System.currentTimeMillis()) throw SafeAccountError("Session expired. Sign in again.")
        return profile(request("GET","/v1/me"))
    }
    override fun retrySession() = account { generation ->
        val current=verifyNow()
        if (generation==epoch) mutable.value=AuthUi(ready=true,profile=current,accessExpiresAtMillis=expires,message="Local account validated.")
    }
    private fun login(path:String, body:JSONObject) = account { generation ->
        val result=request("POST",path,body,"")
        if (generation!=epoch) return@account
        val nextToken=result.getString("access_token"); require(nextToken.matches(Regex("[A-Za-z0-9_-]{43}")))
        val nextExpiry=Instant.parse(result.getString("expires_at")).toEpochMilli(); require(nextExpiry>System.currentTimeMillis())
        val nextOwner=result.getJSONObject("profile").getString("user_id"); UUID.fromString(nextOwner)
        revokeMission(); token=nextToken; owner=nextOwner; expires=nextExpiry
        val current=verifyNow()
        if (generation!=epoch) return@account
        vault.allowSessionWrites()
        vault.saveLocalAccount(JSONObject().put("token",token).put("owner",owner).put("expires",expires).toString())
        mutable.value=AuthUi(ready=true,profile=current,accessExpiresAtMillis=expires,message="Signed in to local MongoDB. Email delivery is not enabled.")
    }
    override fun signUp(name:String,email:String,password:String) = login("/v1/auth/register",JSONObject().put("display_name",name).put("email",email).put("password",password))
    override fun signIn(email:String,password:String) = login("/v1/auth/login",JSONObject().put("identifier",email).put("password",password))
    override fun resetRequest(email:String) { mutable.value=mutable.value.copy(ready=true,busy=false,message="Email recovery is not configured for local prototype accounts. No email was sent.") }
    override fun completeCallback(uri:Uri) { mutable.value=mutable.value.copy(ready=true,busy=false,message="Cloud verification links are not used by the local MongoDB account provider.") }
    override fun changeRecoveredPassword(password:String) { mutable.value=mutable.value.copy(ready=true,busy=false,message="Password recovery is not configured for local prototype accounts.") }
    override fun updateDisplayName(rawName:String) = account { generation ->
        val before=verifyNow()
        val row=request("POST","/v1/me/display-name",JSONObject().put("display_name",rawName.trim()))
        if (row.getString("user_id")!=before.userId) throw SafeAccountError("Profile ownership check failed.")
        if (generation==epoch) mutable.value=AuthUi(ready=true,profile=profile(row),accessExpiresAtMillis=expires,message="Display name saved in MongoDB.")
    }
    override fun signOut() {
        epoch++; revokeMission(); mutable.value=AuthUi(ready=true,busy=true,message="Signing out…")
        scope.launch { operation.withLock {
            var confirmed=false
            try { if (token.isNotBlank()) request("POST","/v1/auth/logout",JSONObject()); confirmed=true } catch (_:Exception) { }
            finally { clear(); mutable.value=AuthUi(ready=true,message=if(confirmed) "Signed out." else "Device session cleared; server revocation could not be confirmed. The token retains its expiry.") }
        } }
    }
    override suspend fun initializeMission(launchId:String):MissionContext = operation.withLock {
        if (!state.value.authenticated) throw SafeAccountError("Sign in before launching a mission.")
        val generation=epoch; val before=verifyNow()
        val row=request("POST","/v1/mission-sessions",JSONObject().put("launch_id",launchId).put("mission_id","mission-red-horizon-poc"))
        if (generation!=epoch || row.getString("user_id")!=before.userId || row.getString("launch_id")!=launchId) throw SafeAccountError("Mission ownership check failed.")
        val expiry=Instant.parse(row.getString("expires_at")).toEpochMilli()
        require(expiry-System.currentTimeMillis() in 1..3_600_000)
        return@withLock MissionContext(row.getString("id"),before.userId,launchId,"mission-red-horizon-poc",expiry)
    }
    override suspend fun submitIntegrationResult(sessionId:String,launchId:String) { operation.withLock {
        verifyNow(); request("POST","/v1/mission-sessions/$sessionId/complete",JSONObject().put("launch_id",launchId).put("outcome","integration_check"))
    } }
    private suspend fun request(method:String,path:String,body:JSONObject?=null,bearer:String=token):JSONObject = withContext(Dispatchers.IO) {
        if (BuildConfig.AUTH_BACKEND_URL.isBlank()) throw SafeAccountError("Local account API address is not configured.")
        val connection=URL(BuildConfig.AUTH_BACKEND_URL.trimEnd('/')+path).openConnection() as HttpURLConnection
        try {
            connection.instanceFollowRedirects=false; connection.connectTimeout=10000; connection.readTimeout=10000; connection.requestMethod=method
            connection.setRequestProperty("Content-Type","application/json")
            if (bearer.isNotBlank()) connection.setRequestProperty("Authorization","Bearer $bearer")
            if (body!=null) { connection.doOutput=true; connection.outputStream.use { it.write(body.toString().toByteArray(Charsets.UTF_8)) } }
            if (connection.responseCode !in listOf(200,201)) throw SafeAccountError(when(connection.responseCode) {
                401,403 -> "Sign-in details are invalid or your session expired."
                409 -> "Those account details are in use, or the mission changed. Retry."
                422 -> "Check your input: valid email, 12-character password, 2–64 character display name."
                429 -> "Too many requests. Wait and retry."
                else -> "Local account service is unavailable."
            })
            val bytes=connection.inputStream.use { input ->
                val output=java.io.ByteArrayOutputStream(); val buffer=ByteArray(4096)
                var count=input.read(buffer)
                while(count!=-1) { output.write(buffer,0,count); if(output.size()>65536) break; count=input.read(buffer) }
                output.toByteArray()
            }
            if (bytes.size>65536) throw SafeAccountError("Invalid account response.")
            JSONObject(bytes.toString(Charsets.UTF_8))
        } finally { connection.disconnect() }
    }
    companion object {
        @Volatile private var instance:MongoAuthCoordinator?=null
        fun get(context:Context):MongoAuthCoordinator = instance ?: synchronized(this) { instance ?: MongoAuthCoordinator(context.applicationContext).also { instance=it } }
    }
}
