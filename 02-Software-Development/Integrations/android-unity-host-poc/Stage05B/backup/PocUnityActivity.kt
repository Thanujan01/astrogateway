package com.sk2lb.astrogateway.poc

import android.content.Intent
import android.os.Bundle
import android.os.Build
import android.window.OnBackInvokedDispatcher
import android.util.Log
import android.view.Gravity
import android.view.KeyEvent
import android.view.WindowInsets
import android.view.WindowInsetsController
import android.graphics.Color
import android.graphics.drawable.GradientDrawable
import android.widget.Button
import android.widget.FrameLayout
import com.unity3d.player.IUnityPlayerSupport
import com.unity3d.player.UnityPlayerActivity
import com.unity3d.player.UnityPlayer
import org.json.JSONObject

class PocUnityActivity : UnityPlayerActivity() {
    private var sceneReady = false
    private var launchId = ""
    private var missionId = ""
    private var lastResult = ""
    override fun onCreate(state: Bundle?) {
        readLaunch(intent)
        super.onCreate(state)
        val player = (this as IUnityPlayerSupport).getUnityPlayerConnection()
        val button = Button(this).apply {
            text = "Return to AstroGateway dashboard"
            isAllCaps = false
            setTextColor(Color.rgb(233,239,242))
            background = GradientDrawable().apply { setColor(Color.rgb(20,31,42)); cornerRadius=24f; setStroke(2,Color.rgb(222,151,116)) }
            setOnClickListener { showDashboard() }
        }
        val dp = resources.displayMetrics.density
        player.getFrameLayout().addView(button, FrameLayout.LayoutParams((360 * dp).toInt(), (56 * dp).toInt(), Gravity.BOTTOM or Gravity.CENTER_HORIZONTAL).apply { bottomMargin = (36 * dp).toInt() })
        button.setOnApplyWindowInsetsListener { view, insets ->
            val bottom = if (Build.VERSION.SDK_INT >= 30) insets.getInsets(WindowInsets.Type.navigationBars() or WindowInsets.Type.displayCutout()).bottom else insets.systemWindowInsetBottom
            (view.layoutParams as FrameLayout.LayoutParams).apply { bottomMargin = bottom + (12 * dp).toInt(); view.layoutParams = this }
            insets
        }
        button.requestApplyInsets()
        if (Build.VERSION.SDK_INT >= 33) {
            onBackInvokedDispatcher.registerOnBackInvokedCallback(OnBackInvokedDispatcher.PRIORITY_OVERLAY) { showDashboard() }
        }
        Log.i("AstroGatewayPOC", "UNITY_ACTIVITY_CREATE launch=$launchId")
    }
    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        if (hasFocus && Build.VERSION.SDK_INT >= 30) window.decorView.post {
            // Keep Android navigation available alongside the native dashboard overlay.
            window.insetsController?.apply {
                systemBarsBehavior = WindowInsetsController.BEHAVIOR_DEFAULT
                show(WindowInsets.Type.navigationBars())
            }
        }
    }
    private fun readLaunch(incoming: Intent?) {
        launchId = incoming?.getStringExtra("launchId") ?: ""
        missionId = incoming?.getStringExtra("missionId") ?: ""
        lastResult = ""
    }
    override fun onNewIntent(incoming: Intent) {
        super.onNewIntent(incoming); setIntent(incoming); readLaunch(incoming)
        Log.i("AstroGatewayPOC", "UNITY_ACTIVITY_RELAUNCH launch=$launchId ready=$sceneReady")
        if (sceneReady) sendMission()
    }
    // Called from the temporary C# scene after its message receiver exists.
    fun onUnityReady() { runOnUiThread { sceneReady = true; publish("READY", "Unity integration scene ready"); sendMission() } }
    private fun sendMission() {
        if (launchId.isBlank() || missionId.isBlank()) { publish("ERROR", "Missing launch descriptor"); return }
        val descriptor = JSONObject().put("version", 1).put("launchId", launchId).put("missionId", missionId).toString()
        Log.i("AstroGatewayPOC", "NATIVE_TO_UNITY launch=$launchId mission=$missionId")
        UnityPlayer.UnitySendMessage("AstroGatewayBridge", "ReceiveMission", descriptor)
    }
    // Only bounded test evidence is relayed. No credentials or account data exist.
    fun onUnityMessage(type: String, id: String, message: String) {
        runOnUiThread {
            if (id != launchId || type !in listOf("ACK", "RESULT", "ERROR") || message.length > 2048) return@runOnUiThread
            if (type == "RESULT") lastResult = message
            publish(type, message)
        }
    }
    private fun publish(type: String, message: String) {
        Log.i("AstroGatewayPOC", "UNITY_TO_NATIVE $type launch=$launchId message=$message")
        sendBroadcast(Intent("$packageName.POC_EVENT").setPackage(packageName).putExtra("launchId", launchId).putExtra("type", type).putExtra("message", message))
    }
    private fun showDashboard() {
        Log.i("AstroGatewayPOC", "UNITY_RETURN launch=$launchId")
        startActivity(Intent(this, MainActivity::class.java).addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            .putExtra("launchId", launchId).putExtra("returned", true).putExtra("result", lastResult))
    }
    override fun dispatchKeyEvent(event: KeyEvent): Boolean {
        if (event.keyCode == KeyEvent.KEYCODE_BACK) { if (event.action == KeyEvent.ACTION_UP) showDashboard(); return true }; return super.dispatchKeyEvent(event)
    }
    @Deprecated("Legacy Back path for Activity entry")
    override fun onBackPressed() { showDashboard() }
    override fun onUnityPlayerUnloaded() { sceneReady = false; showDashboard() }
    override fun onUnityPlayerQuitted() { sceneReady = false; showDashboard(); finish() }
}
