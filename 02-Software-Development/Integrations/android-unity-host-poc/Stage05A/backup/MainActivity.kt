package com.sk2lb.astrogateway.poc

import android.app.Activity
import android.content.*
import android.os.*
import android.util.Log
import android.widget.*
import java.util.UUID

class MainActivity : Activity() {
    private lateinit var status: TextView
    private lateinit var lastMessage: TextView
    private var activeLaunch = ""
    private val preferences by lazy { getSharedPreferences("poc_evidence", MODE_PRIVATE) }
    private val receiver = object : BroadcastReceiver() {
        override fun onReceive(context: Context?, intent: Intent?) {
            if (intent?.action != "$packageName.POC_EVENT") return
            val launch = intent.getStringExtra("launchId") ?: return
            if (launch != activeLaunch) return
            val type = intent.getStringExtra("type") ?: return
            val message = intent.getStringExtra("message") ?: return
            if (message.length > 2048 || type !in listOf("READY", "ACK", "RESULT", "ERROR", "RETURN")) return
            preferences.edit().putString("type", type).putString("message", message).apply()
            Log.i("AstroGatewayPOC", "HOST $type launch=$launch message=$message")
            render()
        }
    }
    override fun onCreate(state: Bundle?) {
        super.onCreate(state)
        activeLaunch = state?.getString("launch") ?: preferences.getString("launch", "")!!
        val layout = LinearLayout(this).apply { orientation = LinearLayout.VERTICAL; setPadding(32, 56, 32, 32) }
        layout.addView(TextView(this).apply { text = "AstroGateway"; textSize = 30f })
        layout.addView(TextView(this).apply { text = "Mission Red Horizon — Android integration POC"; textSize = 18f })
        status = TextView(this).apply { textSize = 18f; setPadding(0, 32, 0, 16) }
        lastMessage = TextView(this).apply { textSize = 16f; setPadding(0, 16, 0, 24) }
        layout.addView(status); layout.addView(lastMessage)
        layout.addView(Button(this).apply { text = "Launch Mission Red Horizon"; setOnClickListener { launchUnity() } })
        layout.addView(TextView(this).apply { text = "Temporary cube scene only. No account or mission gameplay. Return keeps Unity paused for relaunch." })
        setContentView(layout)
        val filter = IntentFilter("$packageName.POC_EVENT")
        if (Build.VERSION.SDK_INT >= 33) registerReceiver(receiver, filter, RECEIVER_NOT_EXPORTED)
        else { @Suppress("DEPRECATION") registerReceiver(receiver, filter) }
        render()
        handleReturn(intent)
    }
    private fun launchUnity() {
        activeLaunch = UUID.randomUUID().toString()
        preferences.edit().putString("launch", activeLaunch).putString("type", "LAUNCHING").putString("message", "Awaiting Unity handshake").apply()
        render()
        Log.i("AstroGatewayPOC", "HOST LAUNCH $activeLaunch")
        startActivity(Intent(this, PocUnityActivity::class.java).apply {
            addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            putExtra("launchId", activeLaunch); putExtra("missionId", "mission-red-horizon-poc")
        })
    }
    private fun render() {
        status.text = "Integration status: ${preferences.getString("type", "NOT TESTED")}"
        lastMessage.text = "Last received test message:\n${preferences.getString("message", "None yet")}"
    }
    private fun handleReturn(incoming: Intent?) {
        if (incoming?.getStringExtra("launchId") != activeLaunch) return
        if (incoming.getBooleanExtra("returned", false)) {
            val result = incoming.getStringExtra("result") ?: ""
            if (result.isNotBlank() && result.length <= 2048) {
                preferences.edit().putString("type", "RESULT").putString("message", result).apply()
                Log.i("AstroGatewayPOC", "HOST RESULT launch=$activeLaunch message=$result")
                render()
            }
            Log.i("AstroGatewayPOC", "HOST DASHBOARD_RETURN launch=$activeLaunch")
            status.text = "Dashboard returned. Last bridge status: ${preferences.getString("type", "UNKNOWN") }"
        }
    }
    override fun onNewIntent(incoming: Intent) { super.onNewIntent(incoming); setIntent(incoming); render(); handleReturn(incoming) }
    override fun onResume() { super.onResume(); if (::status.isInitialized) { render(); handleReturn(intent) } }
    override fun onSaveInstanceState(out: Bundle) { out.putString("launch", activeLaunch); super.onSaveInstanceState(out) }
    override fun onDestroy() { unregisterReceiver(receiver); super.onDestroy() }
}
