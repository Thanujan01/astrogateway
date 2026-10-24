package com.sk2lb.astrogateway.poc
import android.app.Instrumentation
import android.content.Intent
import android.os.Bundle
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.graphics.Bitmap
import kotlinx.coroutines.flow.MutableStateFlow
import java.io.File

/** TEST APK ONLY: synthetic UI state; never installs an SDK session or authenticates a live account. */
class ProfileEditSmokeInstrumentation: Instrumentation() {
    override fun onCreate(arguments:Bundle?) {super.onCreate(arguments);start()}
    override fun onStart() {
        val out=Bundle();var activity:MainActivity?=null
        try {
            android.util.Log.i("UnifiedAuthQA","STEP asynchronous activity launch")
            val monitor=addMonitor(MainActivity::class.java.name,null,false)
            targetContext.startActivity(Intent(targetContext,MainActivity::class.java).addFlags(Intent.FLAG_ACTIVITY_NEW_TASK))
            activity=monitor.waitForActivityWithTimeout(20000) as? MainActivity ?: throw IllegalStateException("Activity launch unavailable")
            removeMonitor(monitor)
            android.util.Log.i("UnifiedAuthQA","STEP activity ready")
            val a=activity
            waitForIdleSync();Thread.sleep(2000)
            val coordinator=AuthCoordinator.get(targetContext)
            val field=AuthCoordinator::class.java.getDeclaredField("mutable").apply {isAccessible=true}
            @Suppress("UNCHECKED_CAST") val flow=field.get(coordinator) as MutableStateFlow<AuthUi>
            val fixture=AstronautProfile("11111111-1111-4111-8111-111111111111","LOCAL UI FIXTURE","astronaut_candidate",0,"2026-01-01T00:00:00Z")
            val go=MainActivity::class.java.getDeclaredMethod("go",String::class.java).apply {isAccessible=true}
            fun publish(state:AuthUi) {runOnMainSync {flow.value=state};waitForIdleSync()}
            fun all(view:View):List<View> = listOf(view)+(if(view is ViewGroup)(0 until view.childCount).flatMap {all(view.getChildAt(it))} else emptyList())
            fun views()=all(a.window.decorView)
            fun click(label:String) {runOnMainSync {views().filterIsInstance<Button>().single {it.text.toString()==label}.performClick()};waitForIdleSync()}
            fun input(value:String) {runOnMainSync {views().filterIsInstance<EditText>().single().setText(value)};waitForIdleSync()}
            fun capture(name:String) {val bitmap=uiAutomation.takeScreenshot();check(bitmap!=null);File(targetContext.filesDir,name).outputStream().use {bitmap.compress(Bitmap.CompressFormat.PNG,100,it)};bitmap.recycle()}
            publish(AuthUi(ready=true,profile=fixture,message="SYNTHETIC UI TEST · NOT LIVE AUTHENTICATION",accessExpiresAtMillis=System.currentTimeMillis()+300000))
            runOnMainSync {go.invoke(a,"Profile")};waitForIdleSync();click("Edit display name")
            check(views().filterIsInstance<EditText>().single().text.toString()==fixture.displayName)
            capture("UnifiedAuth_Android_Profile_Editor_SYNTHETIC.png");out.putString("prefill","PASS synthetic UI")
            input("Unsaved draft");click("Cancel")
            check(coordinator.state.value.profile?.displayName==fixture.displayName)
            out.putString("cancel","PASS no profile mutation")
            click("Edit display name");input("x");click("Save display name")
            fun containsText(node:android.view.accessibility.AccessibilityNodeInfo?):Boolean {
                if(node==null)return false
                if(node.text?.contains("Use 2–64")==true)return true
                return (0 until node.childCount).any {containsText(node.getChild(it))}
            }
            check(containsText(uiAutomation.rootInActiveWindow));out.putString("validation","PASS short name rejected before request")
            sendKeyDownUpSync(android.view.KeyEvent.KEYCODE_BACK);waitForIdleSync()
            publish(AuthUi(ready=true,busy=true,profile=fixture,message="SYNTHETIC SAVING STATE",accessExpiresAtMillis=System.currentTimeMillis()+300000))
            runOnMainSync {go.invoke(a,"Edit profile")};waitForIdleSync()
            check(views().filterIsInstance<Button>().filter {it.text.toString() in listOf("Save display name","Cancel")}.all {!it.isEnabled})
            out.putString("loading","PASS Save/Cancel disabled during synthetic busy state")
            publish(AuthUi(ready=true,profile=fixture,message="SYNTHETIC ERROR: request unavailable",accessExpiresAtMillis=System.currentTimeMillis()+300000))
            check(views().filterIsInstance<TextView>().any {it.text.contains("SYNTHETIC ERROR")});check(views().filterIsInstance<Button>().single {it.text.toString()=="Save display name"}.isEnabled)
            out.putString("error","PASS safe error shown; retry enabled")
            publish(AuthUi(ready=true,message="SYNTHETIC SIGNED-OUT STATE"));runOnMainSync {go.invoke(a,"Edit profile")};waitForIdleSync()
            check(views().filterIsInstance<TextView>().none {it.text.toString()=="Your astronaut identity"});out.putString("protected_editor","PASS signed-out state denied")
            out.putString("classification","TEST APK synthetic UI only; no SDK session, live account or successful backend save")
            publish(AuthUi(ready=true,message="Please sign in to continue."));finish(-1,out)
        }catch(e:Throwable){out.putString("failure",e.javaClass.simpleName);finish(0,out)}
    }
}
