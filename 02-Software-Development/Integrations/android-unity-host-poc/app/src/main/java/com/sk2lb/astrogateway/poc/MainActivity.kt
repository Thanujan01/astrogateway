package com.sk2lb.astrogateway.poc

import android.app.Activity
import android.app.AlertDialog
import android.content.*
import android.os.*
import android.graphics.Color
import android.graphics.Typeface
import android.graphics.drawable.GradientDrawable
import android.util.Log
import android.view.*
import android.widget.*
import android.window.OnBackAnimationCallback
import android.window.BackEvent
import android.window.OnBackInvokedCallback
import android.window.OnBackInvokedDispatcher
import java.util.UUID
import kotlinx.coroutines.*
import kotlinx.coroutines.flow.collect
import android.text.InputType
import android.view.inputmethod.EditorInfo

/** Stage 05A Views and Unity lifecycle retained; private routes require the configured account provider. */
class MainActivity : Activity() {
    private val activityScope = CoroutineScope(SupervisorJob() + Dispatchers.Main.immediate)
    private val account by lazy { AuthCoordinator.get(this) }
    private var authUi = AuthUi()
    private var lastUser = ""
    private var pendingCallback: android.net.Uri? = null
    private val authInputs = mutableMapOf<String,EditText>()
    private var profileDraft: String? = null
    private var launchPending = false
    private var missionSessionId = ""
    private var missionUserId = ""
    private val privateScreens = setOf("Dashboard","Missions","Launch","Profile","Edit profile","Progress","Settings")
    private val stack = mutableListOf("Welcome")
    private var screen = "Splash"
    private var activeLaunch = ""
    private var returnedLaunch = ""
    private var status: TextView? = null
    private var evidence: TextView? = null
    private lateinit var body: LinearLayout
    private val bg = Color.rgb(10,17,25)
    private val panel = Color.rgb(20,31,42)
    private val ink = Color.rgb(233,239,242)
    private val muted = Color.rgb(168,184,197)
    private val accent = Color.rgb(222,151,116)
    private val preferences by lazy { getSharedPreferences("poc_evidence", MODE_PRIVATE) }
    private val backCallback: OnBackInvokedCallback by lazy {
        if (Build.VERSION.SDK_INT >= 34) object : OnBackAnimationCallback {
            override fun onBackStarted(event: BackEvent) { }
            override fun onBackProgressed(event: BackEvent) {
                if (!getSharedPreferences("ui_preferences", MODE_PRIVATE).getBoolean("reduced_motion", false)) {
                    body.translationX = dp(24) * event.progress
                    body.alpha = 1f - .18f * event.progress
                }
            }
            override fun onBackCancelled() { body.translationX = 0f; body.alpha = 1f }
            override fun onBackInvoked() { body.translationX = 0f; body.alpha = 1f; back() }
        } else OnBackInvokedCallback { back() }
    }
    private val receiver = object : BroadcastReceiver() {
        override fun onReceive(context: Context?, intent: Intent?) {
            if (intent?.action != "$packageName.POC_EVENT" || intent.getStringExtra("launchId") != activeLaunch) return
            val type = intent.getStringExtra("type") ?: return
            val message = intent.getStringExtra("message") ?: return
            if (message.length > 2048 || type !in listOf("READY", "ACK", "RESULT", "ERROR", "RETURN")) return
            preferences.edit().putString("type", type).putString("message", message).apply()
            Log.i("AstroGatewayPOC", "HOST $type launch=$activeLaunch message=$message")
            if (account.state.value.authenticated && account.state.value.profile?.userId == missionUserId) renderEvidence()
        }
    }
    override fun onCreate(state: Bundle?) {
        super.onCreate(state)
        activeLaunch = state?.getString("launch") ?: preferences.getString("launch", "")!!
        state?.getStringArrayList("stack")?.let { stack.clear(); stack.addAll(it) }
        val filter = IntentFilter("$packageName.POC_EVENT")
        if (Build.VERSION.SDK_INT >= 33) registerReceiver(receiver, filter, RECEIVER_NOT_EXPORTED) else registerReceiver(receiver, filter)
        missionSessionId = preferences.getString("mission_session", "")!!
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
                    if (lastUser.isNotBlank()) { profileDraft=null; preferences.edit().clear().apply(); activeLaunch=""; missionSessionId=""; missionUserId=""; lastUser="" }
                    if (screen in privateScreens || screen=="Splash" || screen=="Reset password") { stack.clear(); stack.add("Welcome"); show("Welcome") } else show(screen)
                }
                if (!update.busy) pendingCallback?.let { uri -> pendingCallback=null; account.completeCallback(uri) }
            }
        }
    }
    private fun captureCallback(incoming: Intent?) {
        incoming?.data?.let { pendingCallback=it; incoming.data=null }
    }

    private fun dp(v: Int) = (v * resources.displayMetrics.density).toInt()
    private fun shape(color: Int, border: Int = Color.rgb(43,60,74)) = GradientDrawable().apply { setColor(color); cornerRadius=dp(18).toFloat(); setStroke(dp(1),border) }
    private fun text(value: String, size: Float=16f, color: Int=ink): TextView = TextView(this).apply {
        text=value; textSize=size; setTextColor(color); setPadding(0,dp(8),0,dp(8)); typeface=Typeface.create("sans-serif",if(size>=26)Typeface.BOLD else Typeface.NORMAL)
    }
    private fun label(value:String) { body.addView(text(value,12f,accent).apply { letterSpacing=.12f }) }
    private fun paragraph(value:String) { body.addView(text(value,16f,muted)) }
    private fun action(value:String, primary:Boolean=false, run:()->Unit) {
        body.addView(Button(this).apply { text=value; isAllCaps=false; textSize=16f; minHeight=dp(56); setTextColor(if(primary)bg else ink); background=shape(if(primary)accent else panel); setPadding(dp(16),dp(12),dp(16),dp(12)); contentDescription=value; isEnabled=!authUi.busy && !launchPending; alpha=if(isEnabled)1f else .55f; setOnClickListener { run() } },LinearLayout.LayoutParams(-1,-2).apply { topMargin=dp(12); bottomMargin=dp(4) })
    }
    private fun card(title:String, detail:String) {
        val box=LinearLayout(this).apply { orientation=1; setPadding(dp(18),dp(14),dp(18),dp(14)); background=shape(panel) }
        box.addView(text(title,20f)); box.addView(text(detail,15f,muted)); body.addView(box,LinearLayout.LayoutParams(-1,-2).apply { topMargin=dp(12); bottomMargin=dp(4) })
    }
    private fun progress(name:String,value:Int) { card(name,"$value% · no assessment recorded"); body.addView(ProgressBar(this,null,android.R.attr.progressBarStyleHorizontal).apply { max=100; progress=value; progressTintList=android.content.res.ColorStateList.valueOf(accent) },LinearLayout.LayoutParams(-1,dp(6))) }
    private fun go(next:String) { stack.add(next); show(next) }
    private fun dashboard() { if(!account.state.value.authenticated) { stack.clear(); stack.add("Welcome"); show("Welcome"); return }; stack.clear(); stack.add("Dashboard"); show("Dashboard") }
    private fun back() { if(stack.size>1) { stack.removeAt(stack.lastIndex); show(stack.last()) } else moveTaskToBack(true) }
    @Deprecated("Compatibility path") override fun onBackPressed() { back() }
    private fun show(requested:String) {
        val next=if(requested in privateScreens && !account.state.value.authenticated) "Welcome" else requested
        screen=next; status=null; evidence=null; authInputs.clear()
        if (next in setOf("Sign in","Sign up","Reset password")) window.addFlags(WindowManager.LayoutParams.FLAG_SECURE) else window.clearFlags(WindowManager.LayoutParams.FLAG_SECURE)
        if(Build.VERSION.SDK_INT>=33) { onBackInvokedDispatcher.unregisterOnBackInvokedCallback(backCallback); onBackInvokedDispatcher.registerOnBackInvokedCallback(OnBackInvokedDispatcher.PRIORITY_DEFAULT,backCallback) }
        val outer=LinearLayout(this).apply { orientation=1; setBackgroundColor(bg) }
        outer.setOnApplyWindowInsetsListener { v,i -> if(Build.VERSION.SDK_INT>=30) { val bars=i.getInsets(WindowInsets.Type.systemBars() or WindowInsets.Type.displayCutout()); v.setPadding(bars.left,bars.top,bars.right,bars.bottom) } else { @Suppress("DEPRECATION") v.setPadding(i.systemWindowInsetLeft,i.systemWindowInsetTop,i.systemWindowInsetRight,i.systemWindowInsetBottom) }; i }
        val scroll=ScrollView(this).apply { isFillViewport=true }
        val centered=LinearLayout(this).apply { orientation=1; gravity=Gravity.CENTER_HORIZONTAL }
        body=LinearLayout(this).apply { orientation=1; setPadding(dp(24),dp(18),dp(24),dp(28)) }
        val width=minOf(resources.displayMetrics.widthPixels,dp(640))
        centered.addView(body,LinearLayout.LayoutParams(width,-2)); scroll.addView(centered); outer.addView(scroll,LinearLayout.LayoutParams(-1,0,1f)); setContentView(outer)
        label("ASTROGATEWAY  /  SK2LB-ASTROLAB")
        if(next!="Splash" && next!="Welcome" && next!="Dashboard") action("Back") { back() }
        body.addView(text(when(next){"Dashboard"->"Your next horizon.";"Missions"->"Mission control";"Launch"->"Mission Red Horizon";"Profile"->"Astronaut profile";"Edit profile"->"Your astronaut identity";"Progress"->"Training progress";"Sign up"->"Create your profile";"Sign in"->"Welcome back";"Welcome"->"Prepare for a world beyond.";"Password reset"->"Recover your account";"Reset password"->"Set a new password";"Splash"->"AstroGateway";else->next},32f))
        when(next) {
            "Splash" -> { label("MISSION RED HORIZON"); paragraph("Astronaut training • Android foundation"); card("Systems initializing","Native interface and simulator connection") }
            "Welcome" -> {
                label("EXPLORE. UNDERSTAND. ENGINEER."); paragraph("A scientific training workspace for your journey to Mars.")
                card("One AstroGateway account","Your account ID belongs to the configured account API and is shared with mission launches. Local MongoDB is the prototype default.")
                card("Account status",authUi.message)
                action("Sign in",true){go("Sign in")}; action("Create account"){go("Sign up")}
                action("Retry session validation"){account.retrySession()}
                action("Clear local session"){account.signOut()}
            }
            "Sign up","Sign in" -> {
                label("SECURE ASTROGATEWAY ACCOUNT"); paragraph(authUi.message)
                if(next=="Sign up") field("Display name")
                field("Email address"); field("Password")
                paragraph("Passwords go to the configured account service and are never saved by this app. Use at least 12 characters. Local emulator mode connects to the API on your Mac.")
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
                paragraph(authUi.message); field("New password"); action("Cancel recovery and sign out"){account.signOut()}
                action("Save new password",true) { val field=authInputs["New password"]!!; val password=field.text.toString(); field.setText(""); if(password.length>=12) account.changeRecoveredPassword(password) else formError("Use at least 12 characters.") }
            }
            "Dashboard" -> { label("ASTRONAUT  /  ${authUi.profile?.displayName ?: ""}"); paragraph("Welcome, astronaut. Continue your scientific preparation."); card("Training level · ${authUi.profile?.trainingLevel ?: 0}","Account-linked profile. No mission assessment has been recorded."); card("Mission Red Horizon","Mars systems exploration • Simulator preview available"); action("Continue training",true){go("Launch")}; action("Browse missions"){go("Missions")}; label("SCIENTIFIC SKILLS"); card("Geology • Energy • Life support","Observation, engineering trade-offs, and resource awareness"); card("Recent mission","No completed missions. Integration test results are not training scores."); status=text(""); evidence=text("",14f,muted); body.addView(status); body.addView(evidence); action("Astronaut profile"){go("Profile")}; action("Training progress"){go("Progress")}; action("Settings"){go("Settings")}; renderEvidence() }
            "Missions" -> { label("01  /  MARS EXPLORATION"); card("Mission Red Horizon","Jezero-inspired training site • Desktop world in development • Android integration preview"); action("Review Mission Red Horizon",true){go("Launch")}; card("Future missions","Unavailable in this foundation. No additional downloads or game installations.") }
            "Launch" -> { label("SIMULATOR CONNECTION"); card("Mission Red Horizon","Native dashboard → embedded Unity → dashboard. Launches the Mars Missions gameplay scene. Local mission completion is provisional; online score synchronization is not yet connected."); paragraph("Your signed-in account launches a bounded mission context. Return to the dashboard with Android Back."); status=text(""); evidence=text("",14f,muted); body.addView(status); body.addView(evidence); action("Launch Mars Missions",true){launchUnity()}; paragraph("Use the dashboard return control or Android Back. Unity is retained for a warm relaunch."); renderEvidence() }
            "Profile" -> { label("SUPABASE USER IDENTITY"); card(authUi.profile?.displayName ?: "Astronaut", "${authUi.profile?.traineeProfile} • Level ${authUi.profile?.trainingLevel}\nUser ID: ${authUi.profile?.userId}\nProfile created: ${authUi.profile?.createdAt}"); action("Edit display name",true){profileDraft=authUi.profile?.displayName; go("Edit profile")}; card("Learning focus","Mars geology, power systems and habitat engineering"); action("View training progress",true){go("Progress")} }
            "Edit profile" -> {
                label("PERSONAL PROFILE"); paragraph("Your user ID stays the same across web and Android. Your display name can change.")
                field("Display name"); authInputs["Display name"]!!.apply {
                    setText(profileDraft ?: authUi.profile?.displayName ?: "")
                    isEnabled=!authUi.busy
                    addTextChangedListener(object: android.text.TextWatcher {
                        override fun beforeTextChanged(s:CharSequence?,start:Int,count:Int,after:Int) {}
                        override fun onTextChanged(s:CharSequence?,start:Int,before:Int,count:Int) { profileDraft=s?.toString() }
                        override fun afterTextChanged(s:android.text.Editable?) {}
                    })
                }
                paragraph(authUi.message)
                action("Save display name",true) {
                    val name=authInputs["Display name"]!!.text.toString().trim()
                    if(name.codePointCount(0,name.length) !in 2..64 || name.any { it.code<32 || it.code in 127..159 }) formError("Use 2–64 characters without control characters.")
                    else account.updateDisplayName(name)
                }
                action("Cancel") {profileDraft=null; back()}
                action("View saved profile") {profileDraft=null; back()}
            }
            "Progress" -> { label("NO ASSESSMENTS RECORDED"); progress("Scientific observation",0); progress("Power systems",0); progress("Habitat engineering",0); card("Learning record","No live mission results or certificates. Bridge acknowledgements verify integration only."); action("Continue training",true){go("Launch")} }
            "Settings" -> { label("APPLICATION PREFERENCES"); val prefs=getSharedPreferences("ui_preferences",MODE_PRIVATE); body.addView(Switch(this).apply { text="Reduce interface motion"; textSize=16f; setTextColor(ink); minHeight=dp(56); isChecked=prefs.getBoolean("reduced_motion",false); setOnCheckedChangeListener { _,v -> prefs.edit().putBoolean("reduced_motion",v).apply() } }); paragraph("Disables the subtle predictive Back motion on Android 14 and later. Navigation remains available."); card("Simulator quality","Managed by the existing Unity export. Graphics and audio controls are not connected in this stage."); card("Privacy & account","The configured account API owns your identity; local MongoDB is the prototype default. Sessions are encrypted on this device. No plaintext passwords or credentials are sent to Unity."); action("Sign out"){account.signOut()}; card("Application foundation","Local prototype • Unity 6000.6.4f1 • ARM64\nNative Kotlin Views • Missions gameplay"); action("Back to dashboard",true){dashboard()} }
        }
        Log.i("AstroGatewayUI","SCREEN $next")
    }
    private fun field(name:String) {
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
        if(!validEmail(email) || (if(register) password.length<12 else password.isEmpty()) || (register && name.length !in 2..64)) { formError(if(register) "Enter a valid email, a password of at least 12 characters and a display name of 2–64 characters." else "Enter your valid email address and password."); return }
        if(register) account.signUp(name,email,password) else account.signIn(email,password)
    }
    private fun renderEvidence() { if(!account.state.value.authenticated) return; status?.text="Simulator status · ${preferences.getString("type","NOT TESTED")}"; evidence?.text="Last test message: ${preferences.getString("message","No simulator test yet")}\n${preferences.getString("submission", "")}" }
    private fun launchUnity() {
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
            } finally { launchPending=false; if(!isFinishing && !isDestroyed) show(screen) }
        }
    }
    private fun handleReturn(incoming:Intent?) {
        if(!account.state.value.authenticated || account.state.value.profile?.userId!=missionUserId) return
        if(incoming?.getStringExtra("launchId")!=activeLaunch || !incoming.getBooleanExtra("returned",false) || returnedLaunch==activeLaunch) return
        returnedLaunch=activeLaunch; val result=incoming.getStringExtra("result") ?: ""
        if(result.isNotBlank() && result.length<=2048) { preferences.edit().putString("type","RESULT").putString("message",result).apply(); Log.i("AstroGatewayPOC","HOST RESULT launch=$activeLaunch message=$result") }
        dashboard(); Log.i("AstroGatewayPOC","HOST DASHBOARD_RETURN launch=$activeLaunch"); incoming.removeExtra("returned")
        if(result.startsWith("POC_RESULT_OK") && missionSessionId.isNotBlank()) {
            val session=missionSessionId; val launch=activeLaunch
            activityScope.launch {
                try { account.submitIntegrationResult(session,launch); preferences.edit().putString("submission","Integration check recorded; no training score awarded").apply() }
                catch (_: Exception) { preferences.edit().putString("submission","Result was received locally; server submission was not confirmed. No training score awarded.").apply() }
                renderEvidence()
            }
        }
    }
    override fun onNewIntent(incoming:Intent) { super.onNewIntent(incoming); setIntent(incoming); captureCallback(incoming); if(authUi.ready && !authUi.busy) pendingCallback?.let { pendingCallback=null; account.completeCallback(it) }; handleReturn(incoming) }
    override fun onResume() { super.onResume(); if(screen in privateScreens && !account.state.value.authenticated) { show("Welcome") }; if(account.state.value.authenticated && screen in privateScreens) handleReturn(intent); renderEvidence() }
    override fun onSaveInstanceState(out:Bundle) { out.putString("launch",activeLaunch); out.putStringArrayList("stack",ArrayList(stack)); super.onSaveInstanceState(out) }
    override fun onDestroy() { activityScope.cancel(); unregisterReceiver(receiver); super.onDestroy() }
}
