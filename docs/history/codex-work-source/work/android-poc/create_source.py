from pathlib import Path
p=Path('/Users/kapilash/Projects/AstroGatewayAndroid')
files={
'app/src/main/AndroidManifest.xml':'''<manifest xmlns:android="http://schemas.android.com/apk/res/android" xmlns:tools="http://schemas.android.com/tools">
  <application android:label="AstroGateway POC" android:allowBackup="false" android:theme="@android:style/Theme.Material.Light.NoActionBar" android:enableOnBackInvokedCallback="false" tools:replace="android:label,android:allowBackup,android:theme">
    <activity android:name="com.unity3d.player.UnityPlayerActivity" tools:node="remove" />
    <activity android:name=".MainActivity" android:exported="true" android:launchMode="singleTop">
      <intent-filter><action android:name="android.intent.action.MAIN"/><category android:name="android.intent.category.LAUNCHER"/></intent-filter>
    </activity>
    <activity android:name=".PocUnityActivity" android:exported="false" android:process=":unity" android:launchMode="singleTask" android:screenOrientation="landscape" android:hardwareAccelerated="false" android:theme="@style/UnityThemeSelector" android:configChanges="mcc|mnc|locale|touchscreen|keyboard|keyboardHidden|navigation|orientation|screenLayout|uiMode|screenSize|smallestScreenSize|fontScale|layoutDirection|density">
      <meta-data android:name="unityplayer.UnityActivity" android:value="true"/>
    </activity>
  </application>
</manifest>
''',
'app/build.gradle':'''plugins { id 'com.android.application' }
android {
    namespace 'com.sk2lb.astrogateway.poc'
    compileSdk 36
    ndkVersion '27.2.12479018'
    defaultConfig {
        applicationId 'com.sk2lb.astrogateway.poc'
        minSdk 26
        targetSdk 36
        versionCode 1
        versionName '0.1-poc'
        ndk { abiFilters 'arm64-v8a' }
    }
    compileOptions {
        sourceCompatibility JavaVersion.VERSION_17
        targetCompatibility JavaVersion.VERSION_17
    }
    buildTypes { debug { debuggable true; minifyEnabled false } }
}
dependencies {
    implementation project(':unityLibrary')
    implementation fileTree(dir: new File(project(':unityLibrary').projectDir, 'libs'), include: ['*.jar'])
    implementation 'org.jetbrains.kotlin:kotlin-stdlib:2.2.10'
}
''',
'app/src/main/java/com/sk2lb/astrogateway/poc/MainActivity.kt':'''package com.sk2lb.astrogateway.poc

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
        lastMessage.text = "Last received test message:\\n${preferences.getString("message", "None yet")}".replace("\\n", "\n")
    }
    private fun handleReturn(incoming: Intent?) {
        if (incoming?.getStringExtra("launchId") != activeLaunch) return
        if (incoming.getBooleanExtra("returned", false)) {
            Log.i("AstroGatewayPOC", "HOST DASHBOARD_RETURN launch=$activeLaunch")
            status.text = "Dashboard returned. Last bridge status: ${preferences.getString("type", "UNKNOWN") }"
        }
    }
    override fun onNewIntent(incoming: Intent) { super.onNewIntent(incoming); setIntent(incoming); render(); handleReturn(incoming) }
    override fun onResume() { super.onResume(); if (::status.isInitialized) { render(); handleReturn(intent) } }
    override fun onSaveInstanceState(out: Bundle) { out.putString("launch", activeLaunch); super.onSaveInstanceState(out) }
    override fun onDestroy() { unregisterReceiver(receiver); super.onDestroy() }
}
''',
'app/src/main/java/com/sk2lb/astrogateway/poc/PocUnityActivity.kt':'''package com.sk2lb.astrogateway.poc

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.Gravity
import android.widget.Button
import android.widget.FrameLayout
import com.unity3d.player.IUnityPlayerSupport
import com.unity3d.player.UnityPlayerActivity
import org.json.JSONObject

class PocUnityActivity : UnityPlayerActivity() {
    private var sceneReady = false
    private var launchId = ""
    private var missionId = ""
    override fun onCreate(state: Bundle?) {
        readLaunch(intent)
        super.onCreate(state)
        val player = (this as IUnityPlayerSupport).getUnityPlayerConnection()
        val button = Button(this).apply {
            text = "Return to AstroGateway dashboard"
            setOnClickListener { showDashboard() }
        }
        val dp = resources.displayMetrics.density
        player.getFrameLayout().addView(button, FrameLayout.LayoutParams((360 * dp).toInt(), (56 * dp).toInt(), Gravity.BOTTOM or Gravity.CENTER_HORIZONTAL))
        Log.i("AstroGatewayPOC", "UNITY_ACTIVITY_CREATE launch=$launchId")
    }
    private fun readLaunch(incoming: Intent?) {
        launchId = incoming?.getStringExtra("launchId") ?: ""
        missionId = incoming?.getStringExtra("missionId") ?: ""
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
        (this as IUnityPlayerSupport).getUnityPlayerConnection().UnitySendMessage("AstroGatewayBridge", "ReceiveMission", descriptor)
    }
    // Only bounded test evidence is relayed. No credentials or account data exist.
    fun onUnityMessage(type: String, id: String, message: String) {
        runOnUiThread {
            if (id != launchId || type !in listOf("ACK", "RESULT", "ERROR") || message.length > 2048) return@runOnUiThread
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
            .putExtra("launchId", launchId).putExtra("returned", true))
    }
    @Deprecated("Legacy Back path for Activity entry")
    override fun onBackPressed() { showDashboard() }
    override fun onUnityPlayerUnloaded() { sceneReady = false; showDashboard() }
    override fun onUnityPlayerQuitted() { sceneReady = false; showDashboard(); finish() }
}
''',
'UnityIntegrationProject/Assets/AstroGateway/IntegrationPOC/AstroGatewayPocBridge.cs':'''using System;
using System.Collections;
using UnityEngine;

public sealed class AstroGatewayPocBridge : MonoBehaviour
{
    [Serializable] private class Descriptor { public int version; public string launchId; public string missionId; }
    private string display = "Waiting for native mission descriptor";
    private IEnumerator Start()
    {
        yield return null;
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            activity.Call("onUnityReady");
#endif
    }
    public void ReceiveMission(string json)
    {
        try
        {
            var mission = JsonUtility.FromJson<Descriptor>(json);
            if (mission == null || mission.version != 1 || string.IsNullOrEmpty(mission.launchId) || mission.missionId != "mission-red-horizon-poc")
                throw new ArgumentException("Invalid POC descriptor");
            display = "Native mission received: " + mission.missionId + "\\nLaunch: " + mission.launchId;
            Debug.Log("AstroGatewayPOC UNITY_RECEIVED " + mission.launchId);
            Send("ACK", mission.launchId, "Unity acknowledged " + mission.missionId);
            Send("RESULT", mission.launchId, "POC_RESULT_OK: native mission identifier received and Unity response sent");
        }
        catch (Exception error) { display = "Descriptor error: " + error.Message; Debug.LogError(display); }
    }
    private void Send(string type, string launchId, string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            activity.Call("onUnityMessage", type, launchId, message);
#endif
    }
    private void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(20, Screen.height / 30), wordWrap = true };
        GUI.Label(new Rect(24, 24, Screen.width - 48, 160), "AstroGateway — Unity integration scene\\n" + display, style);
    }
}
''',
'UnityIntegrationProject/Assets/AstroGateway/IntegrationPOC/Editor/AndroidPocExporter.cs':'''using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AndroidPocExporter
{
    public static void Export()
    {
        const string scenePath = "Assets/AstroGateway/IntegrationPOC/UnityIntegration_Test.unity";
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var bridge = new GameObject("AstroGatewayBridge"); bridge.AddComponent<AstroGatewayPocBridge>();
        var camera = new GameObject("POC_Camera").AddComponent<Camera>();
        camera.transform.position = new Vector3(0, 2, -6); camera.transform.LookAt(new Vector3(0, 0.5f, 0));
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(0.11f, 0.15f, 0.19f); camera.tag = "MainCamera";
        var light = new GameObject("POC_KeyLight").AddComponent<Light>(); light.type = LightType.Directional; light.transform.rotation = Quaternion.Euler(40, -30, 0);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube); cube.name = "POC_OneMeterCube"; cube.transform.position = new Vector3(0, 0.5f, 0);
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit")); material.color = new Color(0.76f, 0.44f, 0.22f);
        const string materialPath = "Assets/AstroGateway/IntegrationPOC/POC_Cube.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(materialPath)) AssetDatabase.DeleteAsset(materialPath);
        AssetDatabase.CreateAsset(material, materialPath); cube.GetComponent<Renderer>().sharedMaterial = material;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
        PlayerSettings.companyName = "SK2LB-AstroLab"; PlayerSettings.productName = "AstroGateway Integration POC";
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.sk2lb.astrogateway.poc");
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.useCustomKeystore = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        EditorUserBuildSettings.buildAppBundle = false;
        var exportPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../unity-export"));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[]{scenePath}, locationPathName = exportPath, target = BuildTarget.Android, options = BuildOptions.Development | BuildOptions.AcceptExternalModificationsToPlayer });
        Debug.Log("ASTROGATEWAY_EXPORT_RESULT " + report.summary.result + " " + exportPath);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Android POC export failed: " + report.summary.result);
    }
}
''',
'README.md':'''# AstroGateway Android integration POC

Native Kotlin host source and isolated Unity integration copy. Production Unity/React projects are not edited. Temporary scene contains only a camera, directional light, a one-meter cube and the test receiver. Export only that scene, not the Mars environment.

Unity 6000.6.4f1 Activity entry, separate :unity process. One runtime instance is retained when the dashboard returns; Activity pause/resume is inherited from Unity. Relaunch reuses the existing Activity, sends a fresh launch UUID and repeats ACK/RESULT. This POC deliberately does not claim to free Unity memory on dashboard return. Host process death, Unity exit and unload have distinct behavior and require device tests.

Native host sends version 1 mission descriptor after scene-ready handshake. C# acknowledges then returns a test result through Activity callbacks. Package-restricted non-exported receiver records evidence on the dashboard. Identifiers are test data, not authorization credentials. No backend/Auth services exist.

Build and test status are tracked in logs and the output completion report; source presence is not proof of a working APK.
'''
}
for name,content in files.items():
 target=p/name
 if target.exists():raise SystemExit('Refusing existing file: '+str(target))
 target.parent.mkdir(parents=True,exist_ok=True);target.write_text(content)
print('Wrote',len(files),'isolated POC source files')
