from pathlib import Path
p=Path('/Users/kapilash/Projects/AstroGatewayAndroid/app/src/main/java/com/sk2lb/astrogateway/poc/MainActivity.kt')
s=p.read_text().replace('import java.util.UUID','import java.util.UUID\nimport kotlinx.coroutines.*\nimport kotlinx.coroutines.flow.collect\nimport android.text.InputType\nimport android.view.inputmethod.EditorInfo')
s=s.replace('/** Native foundation only. Account fields are intentionally unavailable; no authentication exists. */','/** Stage 05A Views and Unity lifecycle retained; all private routes now require Supabase identity. */')
s=s.replace('    private val stack =', '''    private val activityScope = CoroutineScope(SupervisorJob() + Dispatchers.Main.immediate)
    private val account by lazy { AuthCoordinator.get(this) }
    private var authUi = AuthUi()
    private var lastUser = ""
    private var pendingCallback: android.net.Uri? = null
    private val authInputs = mutableMapOf<String,EditText>()
    private var launchPending = false
    private var missionSessionId = ""
    private var missionUserId = ""
    private val privateScreens = setOf("Dashboard","Missions","Launch","Profile","Progress","Settings")
    private val stack =''')
s=s.replace('            renderEvidence()','            if (account.state.value.authenticated && account.state.value.profile?.userId == missionUserId) renderEvidence()',1)
start=s.index('        if (state == null) {')
end=s.index('\n    private fun dp(',start)
s=s[:start]+'''        missionSessionId = preferences.getString("mission_session", "")!!
        missionUserId = preferences.getString("mission_user", "")!!
        show("Splash")
        captureCallback(intent)
        activityScope.launch {
            account.state.collect { update ->
                authUi = update
                if (!update.ready) { show("Splash"); return@collect }
                if (update.authenticated) {
                    val user = update.profile!!.userId
                    if (lastUser != user || screen !in privateScreens) { lastUser=user; dashboard() } else show(screen)
                    handleReturn(intent)
                } else if (update.recovery) {
                    stack.clear(); stack.add("Reset password"); show("Reset password")
                } else {
                    if (lastUser.isNotBlank()) { preferences.edit().clear().apply(); activeLaunch=""; missionSessionId=""; missionUserId=""; lastUser="" }
                    if (screen in privateScreens || screen=="Splash" || screen=="Reset password") { stack.clear(); stack.add("Welcome"); show("Welcome") } else show(screen)
                }
                if (!update.busy) pendingCallback?.let { uri -> pendingCallback=null; account.completeCallback(uri) }
            }
        }
    }
    private fun captureCallback(incoming: Intent?) {
        incoming?.data?.let { pendingCallback=it; incoming.data=null }
    }
''' + s[end:]
s=s.replace('setOnClickListener { run() }','isEnabled=!authUi.busy && !launchPending; alpha=if(isEnabled)1f else .55f; setOnClickListener { run() }')
s=s.replace('private fun dashboard() { stack.clear();','private fun dashboard() { if(!account.state.value.authenticated) { stack.clear(); stack.add("Welcome"); show("Welcome"); return }; stack.clear();')
s=s.replace('    private fun show(next:String) {\n        screen=next; status=null; evidence=null','''    private fun show(requested:String) {
        val next=if(requested in privateScreens && !account.state.value.authenticated) "Welcome" else requested
        screen=next; status=null; evidence=null; authInputs.clear()
        if (next in setOf("Sign in","Sign up","Reset password")) window.addFlags(WindowManager.LayoutParams.FLAG_SECURE) else window.clearFlags(WindowManager.LayoutParams.FLAG_SECURE)''')
s=s.replace('"Welcome"->"Prepare for a world beyond.";','"Welcome"->"Prepare for a world beyond.";"Password reset"->"Recover your account";"Reset password"->"Set a new password";')
start=s.index('            "Welcome" ->')
end=s.index('            "Dashboard" ->',start)
s=s[:start]+'''            "Welcome" -> {
                label("EXPLORE. UNDERSTAND. ENGINEER."); paragraph("A scientific training workspace for your journey to Mars.")
                card("One AstroGateway account","Your Supabase identity belongs to this application, the mission trainer and the future web application. Training gameplay remains in development.")
                card("Account status",authUi.message)
                action("Sign in",true){go("Sign in")}; action("Create account"){go("Sign up")}
                action("Retry session validation"){account.retrySession()}
                action("Clear local session"){account.signOut()}
            }
            "Sign up","Sign in" -> {
                label("SECURE ASTROGATEWAY ACCOUNT"); paragraph(authUi.message)
                if(next=="Sign up") field("Display name")
                field("Email address"); field("Password")
                paragraph("Passwords are sent only to Supabase Auth over HTTPS and are not saved by this app. Minimum 12 characters.")
                action(if(next=="Sign up") "Register account" else "Sign in to AstroGateway",true) { submitCredentials(next=="Sign up") }
                if(next=="Sign in") action("Forgot password?"){go("Password reset")}
                action(if(next=="Sign up")"Already have an account?" else "Create account"){go(if(next=="Sign up")"Sign in" else "Sign up")}
            }
            "Password reset" -> {
                paragraph(authUi.message); field("Email address")
                paragraph("Request a recovery email and open it on this device. This screen does not reveal whether an address has an account.")
                action("Send recovery email",true) { val email=authInputs["Email address"]!!.text.toString().trim(); if(validEmail(email)) account.resetRequest(email) else formError("Enter a valid email address.") }
            }
            "Reset password" -> {
                paragraph(authUi.message); field("New password")
                action("Save new password",true) { val field=authInputs["New password"]!!; val password=field.text.toString(); field.setText(""); if(password.length>=12) account.changeRecoveredPassword(password) else formError("Use at least 12 characters.") }
            }
''' +s[end:]
s=s.replace('label("CADET 07  /  DEMO WORKSPACE"); paragraph("Welcome, astronaut. Continue your scientific preparation."); card("Training readiness · 42%","Illustrative overview • 3 of 7 learning modules reviewed. No real assessment has been recorded.");','label("ASTRONAUT  /  ${authUi.profile?.displayName ?: ""}"); paragraph("Welcome, astronaut. Continue your scientific preparation."); card("Training level · ${authUi.profile?.trainingLevel ?: 0}","Account-linked profile. No mission assessment has been recorded.");')
s=s.replace('            "Profile" -> { label("DEMO IDENTITY"); card("Cadet 07","Astronaut candidate • SK2LB-AstroLab\\nPlaceholder profile. No signed-in account.");','            "Profile" -> { label("SUPABASE USER IDENTITY"); card(authUi.profile?.displayName ?: "Astronaut", "${authUi.profile?.traineeProfile} • Level ${authUi.profile?.trainingLevel}\\nUser ID: ${authUi.profile?.userId}\\nProfile created: ${authUi.profile?.createdAt}");')
s=s.replace('label("MOCK DATA · NOT A REAL ASSESSMENT"); progress("Scientific observation",60); progress("Power systems",35); progress("Habitat engineering",30);','label("NO ASSESSMENTS RECORDED"); progress("Scientific observation",0); progress("Power systems",0); progress("Habitat engineering",0);')
s=s.replace('$value% · illustrative demo progress','$value% · no assessment recorded')
s=s.replace('card("Privacy & account","Offline demo. No authentication, backend, password storage or telemetry.");','card("Privacy & account","Supabase is the single identity provider. Sessions are encrypted on this device. No plaintext passwords or credentials are sent to Unity."); action("Sign out"){account.signOut()};')
s=s.replace('Stage 05A • Unity','Stage 05B • Unity')
start=s.index('    private fun field(name:String)')
end=s.index('    private fun renderEvidence()',start)
s=s[:start]+'''    private fun field(name:String) {
        body.addView(text(name,14f))
        val password=name.contains("password",true)
        val edit=EditText(this).apply {
            hint=name; isEnabled=!authUi.busy; isSaveEnabled=false; setTextColor(ink); setHintTextColor(muted)
            background=shape(panel); setPadding(dp(16),dp(12),dp(16),dp(12)); minHeight=dp(56)
            isSingleLine=true; contentDescription=name
            inputType=if(password) InputType.TYPE_CLASS_TEXT or InputType.TYPE_TEXT_VARIATION_PASSWORD else if(name=="Email address") InputType.TYPE_CLASS_TEXT or InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS else InputType.TYPE_CLASS_TEXT or InputType.TYPE_TEXT_FLAG_CAP_WORDS
            imeOptions=EditorInfo.IME_ACTION_DONE or EditorInfo.IME_FLAG_NO_PERSONALIZED_LEARNING
            setAutofillHints(if(password) View.AUTOFILL_HINT_PASSWORD else if(name=="Email address") View.AUTOFILL_HINT_EMAIL_ADDRESS else View.AUTOFILL_HINT_NAME)
        }
        authInputs[name]=edit; body.addView(edit)
    }
    private fun validEmail(email:String)=android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches() && email.length<=254
    private fun formError(message:String) { AlertDialog.Builder(this).setTitle("Check account details").setMessage(message).setPositiveButton("OK",null).show() }
    private fun submitCredentials(register:Boolean) {
        val email=authInputs["Email address"]!!.text.toString().trim()
        val passwordField=authInputs["Password"]!!; val password=passwordField.text.toString(); passwordField.setText("")
        val name=authInputs["Display name"]?.text?.toString()?.trim() ?: ""
        if(!validEmail(email) || password.length<12 || (register && name.length !in 2..64)) { formError("Enter a valid email, a password of at least 12 characters${if(register) " and a display name of 2–64 characters" else ""}."); return }
        if(register) account.signUp(name,email,password) else account.signIn(email,password)
    }
''' +s[end:]
start=s.index('    private fun launchUnity() {')
end=s.index('    private fun handleReturn(',start)
s=s[:start]+'''    private fun launchUnity() {
        if(!account.state.value.authenticated || launchPending) { formError("Sign in and validate your session before launching."); return }
        launchPending=true; show("Launch")
        activityScope.launch {
            try {
                val context=account.initializeMission(UUID.randomUUID().toString())
                if(!account.state.value.authenticated || account.state.value.profile?.userId!=context.userId) return@launch
                activeLaunch=context.launchId; missionSessionId=context.sessionId; missionUserId=context.userId; returnedLaunch=""
                preferences.edit().putString("launch",activeLaunch).putString("mission_session",missionSessionId).putString("mission_user",missionUserId).putString("type","LAUNCHING").putString("message","Authorized session created. Awaiting Unity handshake").apply()
                Log.i("AstroGatewayPOC","HOST LAUNCH $activeLaunch")
                startActivity(Intent(this@MainActivity,PocUnityActivity::class.java).addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT or Intent.FLAG_ACTIVITY_SINGLE_TOP)
                    .putExtra("launchId",activeLaunch).putExtra("missionId",context.missionId).putExtra("missionSessionId",context.sessionId).putExtra("userId",context.userId).putExtra("expiresAtMillis",context.expiresAtMillis))
            } catch (cancelled: CancellationException) { throw cancelled }
            catch(error:Exception) {
                val message=(error as? SafeAccountError)?.uiMessage ?: "Mission launch failed. Check your connection and retry."
                preferences.edit().putString("type","ERROR").putString("message",message).apply()
                if(!isFinishing) { show("Launch"); formError(message) }
            } finally { launchPending=false; if(!isFinishing && screen=="Launch") show("Launch") }
        }
    }
''' +s[end:]
s=s.replace('        if(incoming?.getStringExtra("launchId")!=activeLaunch','        if(!account.state.value.authenticated || account.state.value.profile?.userId!=missionUserId) return\n        if(incoming?.getStringExtra("launchId")!=activeLaunch')
s=s.replace('        dashboard(); Log.i("AstroGatewayPOC","HOST DASHBOARD_RETURN launch=$activeLaunch"); incoming.removeExtra("returned")','''        dashboard(); Log.i("AstroGatewayPOC","HOST DASHBOARD_RETURN launch=$activeLaunch"); incoming.removeExtra("returned")
        if(result.startsWith("POC_RESULT_OK") && missionSessionId.isNotBlank()) {
            val session=missionSessionId; val launch=activeLaunch
            activityScope.launch {
                try { account.submitIntegrationResult(session,launch); preferences.edit().putString("submission","Integration check recorded; no training score awarded").apply() }
                catch (_: Exception) { preferences.edit().putString("submission","Result was received locally; server submission was not confirmed. No training score awarded.").apply() }
                renderEvidence()
            }
        }''')
s=s.replace('private fun renderEvidence() { status?.text=', 'private fun renderEvidence() { if(!account.state.value.authenticated) return; status?.text=')
s=s.replace('No simulator test yet")}" }','No simulator test yet")}\\n${preferences.getString("submission", "")}" }')
s=s.replace('setIntent(incoming); handleReturn(incoming)','setIntent(incoming); captureCallback(incoming); if(authUi.ready && !authUi.busy) pendingCallback?.let { pendingCallback=null; account.completeCallback(it) }; handleReturn(incoming)')
s=s.replace('override fun onResume() { super.onResume(); handleReturn(intent); renderEvidence() }','override fun onResume() { super.onResume(); if(account.state.value.authenticated && screen in privateScreens) handleReturn(intent); renderEvidence() }')
s=s.replace('override fun onDestroy() { unregisterReceiver(receiver); super.onDestroy() }','override fun onDestroy() { activityScope.cancel(); unregisterReceiver(receiver); super.onDestroy() }')
Path('work/stage05b/MainActivity.kt').write_text(s)
