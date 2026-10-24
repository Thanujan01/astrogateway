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

/** Native foundation only. Account fields are intentionally unavailable; no authentication exists. */
class MainActivity : Activity() {
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
            renderEvidence()
        }
    }
    override fun onCreate(state: Bundle?) {
        super.onCreate(state)
        activeLaunch = state?.getString("launch") ?: preferences.getString("launch", "")!!
        state?.getStringArrayList("stack")?.let { stack.clear(); stack.addAll(it) }
        val filter = IntentFilter("$packageName.POC_EVENT")
        if (Build.VERSION.SDK_INT >= 33) registerReceiver(receiver, filter, RECEIVER_NOT_EXPORTED) else registerReceiver(receiver, filter)
        if (state == null) {
            show("Splash")
            Handler(Looper.getMainLooper()).postDelayed({ if (!isFinishing && screen == "Splash") show(stack.last()) }, 1800)
        } else show(stack.last())
        handleReturn(intent)
    }
    private fun dp(v: Int) = (v * resources.displayMetrics.density).toInt()
    private fun shape(color: Int, border: Int = Color.rgb(43,60,74)) = GradientDrawable().apply { setColor(color); cornerRadius=dp(18).toFloat(); setStroke(dp(1),border) }
    private fun text(value: String, size: Float=16f, color: Int=ink): TextView = TextView(this).apply {
        text=value; textSize=size; setTextColor(color); setPadding(0,dp(8),0,dp(8)); typeface=Typeface.create("sans-serif",if(size>=26)Typeface.BOLD else Typeface.NORMAL)
    }
    private fun label(value:String) { body.addView(text(value,12f,accent).apply { letterSpacing=.12f }) }
    private fun paragraph(value:String) { body.addView(text(value,16f,muted)) }
    private fun action(value:String, primary:Boolean=false, run:()->Unit) {
        body.addView(Button(this).apply { text=value; isAllCaps=false; textSize=16f; minHeight=dp(56); setTextColor(if(primary)bg else ink); background=shape(if(primary)accent else panel); setPadding(dp(16),dp(12),dp(16),dp(12)); contentDescription=value; setOnClickListener { run() } },LinearLayout.LayoutParams(-1,-2).apply { topMargin=dp(12); bottomMargin=dp(4) })
    }
    private fun card(title:String, detail:String) {
        val box=LinearLayout(this).apply { orientation=1; setPadding(dp(18),dp(14),dp(18),dp(14)); background=shape(panel) }
        box.addView(text(title,20f)); box.addView(text(detail,15f,muted)); body.addView(box,LinearLayout.LayoutParams(-1,-2).apply { topMargin=dp(12); bottomMargin=dp(4) })
    }
    private fun progress(name:String,value:Int) { card(name,"$value% · illustrative demo progress"); body.addView(ProgressBar(this,null,android.R.attr.progressBarStyleHorizontal).apply { max=100; progress=value; progressTintList=android.content.res.ColorStateList.valueOf(accent) },LinearLayout.LayoutParams(-1,dp(6))) }
    private fun go(next:String) { stack.add(next); show(next) }
    private fun dashboard() { stack.clear(); stack.add("Dashboard"); show("Dashboard") }
    private fun back() { if(stack.size>1) { stack.removeAt(stack.lastIndex); show(stack.last()) } else moveTaskToBack(true) }
    @Deprecated("Compatibility path") override fun onBackPressed() { back() }
    private fun show(next:String) {
        screen=next; status=null; evidence=null
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
        body.addView(text(when(next){"Dashboard"->"Your next horizon.";"Missions"->"Mission control";"Launch"->"Mission Red Horizon";"Profile"->"Astronaut profile";"Progress"->"Training progress";"Sign up"->"Create your profile";"Sign in"->"Welcome back";"Welcome"->"Prepare for a world beyond.";"Splash"->"AstroGateway";else->next},32f))
        when(next) {
            "Splash" -> { label("MISSION RED HORIZON"); paragraph("Astronaut training • Android foundation"); card("Systems initializing","Native interface and simulator connection") }
            "Welcome" -> { label("EXPLORE. UNDERSTAND. ENGINEER."); paragraph("A scientific training workspace for your journey to Mars."); card("One mission. Many disciplines.","Explore mission systems, build scientific understanding, and review your training in one place."); card("Foundation preview","Demo data only. Authentication is not implemented. The simulator currently opens the validated integration scene."); action("Explore demo dashboard",true){dashboard()}; action("Sign in"){go("Sign in")}; action("Create account"){go("Sign up")} }
            "Sign up","Sign in" -> { label("AUTHENTICATION NOT IMPLEMENTED"); paragraph("This is an interface preview. No account is created and no credentials are collected. Do not enter real passwords."); if(next=="Sign up") field("Full name"); field("Email address"); field("Password"); action("Authentication unavailable"){AlertDialog.Builder(this).setTitle("Preview only").setMessage("Account services are not connected. Use the clearly identified demo workspace instead.").setPositiveButton("Understood",null).show()}; action("Use demo workspace",true){dashboard()}; action(if(next=="Sign up")"Already have an account?" else "Create account"){go(if(next=="Sign up")"Sign in" else "Sign up")} }
            "Dashboard" -> { label("CADET 07  /  DEMO WORKSPACE"); paragraph("Welcome, astronaut. Continue your scientific preparation."); card("Training readiness · 42%","Illustrative overview • 3 of 7 learning modules reviewed. No real assessment has been recorded."); card("Mission Red Horizon","Mars systems exploration • Simulator preview available"); action("Continue training",true){go("Launch")}; action("Browse missions"){go("Missions")}; label("SCIENTIFIC SKILLS"); card("Geology • Energy • Life support","Observation, engineering trade-offs, and resource awareness"); card("Recent mission","No completed missions. Integration test results are not training scores."); status=text(""); evidence=text("",14f,muted); body.addView(status); body.addView(evidence); action("Astronaut profile"){go("Profile")}; action("Training progress"){go("Progress")}; action("Settings"){go("Settings")}; renderEvidence() }
            "Missions" -> { label("01  /  MARS EXPLORATION"); card("Mission Red Horizon","Jezero-inspired training site • Desktop world in development • Android integration preview"); action("Review Mission Red Horizon",true){go("Launch")}; card("Future missions","Unavailable in this foundation. No additional downloads or game installations.") }
            "Launch" -> { label("SIMULATOR CONNECTION"); card("Mission Red Horizon","Native dashboard → embedded Unity → dashboard. The current scene is the temporary cube integration test, not the production Mars environment."); paragraph("Test objective: send a mission identifier, receive an acknowledgement and result, then return and relaunch."); status=text(""); evidence=text("",14f,muted); body.addView(status); body.addView(evidence); action("Launch simulator preview",true){launchUnity()}; paragraph("Use the dashboard return control or Android Back. Unity is retained for a warm relaunch."); renderEvidence() }
            "Profile" -> { label("DEMO IDENTITY"); card("Cadet 07","Astronaut candidate • SK2LB-AstroLab\nPlaceholder profile. No signed-in account."); card("Learning focus","Mars geology, power systems and habitat engineering"); action("View training progress",true){go("Progress")} }
            "Progress" -> { label("MOCK DATA · NOT A REAL ASSESSMENT"); progress("Scientific observation",60); progress("Power systems",35); progress("Habitat engineering",30); card("Learning record","No live mission results or certificates. Bridge acknowledgements verify integration only."); action("Continue training",true){go("Launch")} }
            "Settings" -> { label("APPLICATION PREFERENCES"); val prefs=getSharedPreferences("ui_preferences",MODE_PRIVATE); body.addView(Switch(this).apply { text="Reduce interface motion"; textSize=16f; setTextColor(ink); minHeight=dp(56); isChecked=prefs.getBoolean("reduced_motion",false); setOnCheckedChangeListener { _,v -> prefs.edit().putBoolean("reduced_motion",v).apply() } }); paragraph("Disables the subtle predictive Back motion on Android 14 and later. Navigation remains available."); card("Simulator quality","Managed by the existing Unity export. Graphics and audio controls are not connected in this stage."); card("Privacy & account","Offline demo. No authentication, backend, password storage or telemetry."); card("Application foundation","Stage 05A • Unity 6000.6.4f1 • ARM64\nNative Kotlin Views • Integration scene only"); action("Back to dashboard",true){dashboard()} }
        }
        Log.i("AstroGatewayUI","SCREEN $next")
    }
    private fun field(name:String) { body.addView(text(name,14f)); body.addView(EditText(this).apply { hint="$name unavailable in preview"; isEnabled=false; isSaveEnabled=false; setTextColor(ink); setHintTextColor(muted); background=shape(panel); setPadding(dp(16),dp(12),dp(16),dp(12)); minHeight=dp(56); contentDescription="$name — authentication not implemented" }) }
    private fun renderEvidence() { status?.text="Simulator status · ${preferences.getString("type","NOT TESTED")}"; evidence?.text="Last test message: ${preferences.getString("message","No simulator test yet")}" }
    private fun launchUnity() {
        activeLaunch=UUID.randomUUID().toString(); preferences.edit().putString("launch",activeLaunch).putString("type","LAUNCHING").putString("message","Awaiting Unity handshake").apply(); renderEvidence(); Log.i("AstroGatewayPOC","HOST LAUNCH $activeLaunch")
        try { startActivity(Intent(this,PocUnityActivity::class.java).addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT or Intent.FLAG_ACTIVITY_SINGLE_TOP).putExtra("launchId",activeLaunch).putExtra("missionId","mission-red-horizon-poc")) } catch(e:ActivityNotFoundException) { preferences.edit().putString("type","ERROR").putString("message","Simulator unavailable. Return to dashboard and try again.").apply(); renderEvidence() }
    }
    private fun handleReturn(incoming:Intent?) {
        if(incoming?.getStringExtra("launchId")!=activeLaunch || !incoming.getBooleanExtra("returned",false) || returnedLaunch==activeLaunch) return
        returnedLaunch=activeLaunch; val result=incoming.getStringExtra("result") ?: ""
        if(result.isNotBlank() && result.length<=2048) { preferences.edit().putString("type","RESULT").putString("message",result).apply(); Log.i("AstroGatewayPOC","HOST RESULT launch=$activeLaunch message=$result") }
        dashboard(); Log.i("AstroGatewayPOC","HOST DASHBOARD_RETURN launch=$activeLaunch"); incoming.removeExtra("returned")
    }
    override fun onNewIntent(incoming:Intent) { super.onNewIntent(incoming); setIntent(incoming); handleReturn(incoming) }
    override fun onResume() { super.onResume(); handleReturn(intent); renderEvidence() }
    override fun onSaveInstanceState(out:Bundle) { out.putString("launch",activeLaunch); out.putStringArrayList("stack",ArrayList(stack)); super.onSaveInstanceState(out) }
    override fun onDestroy() { unregisterReceiver(receiver); super.onDestroy() }
}
