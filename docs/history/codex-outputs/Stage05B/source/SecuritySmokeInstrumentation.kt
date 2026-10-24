package com.sk2lb.astrogateway.poc
import android.app.Instrumentation
import android.content.*
import android.os.Bundle
import android.view.KeyEvent
import io.github.jan.supabase.auth.user.UserSession
import kotlinx.coroutines.runBlocking
import java.io.File
import java.security.KeyStore
import java.util.UUID
import java.util.concurrent.CountDownLatch
import java.util.concurrent.TimeUnit

/** Test APK only. Fixtures are not real accounts or authentication evidence. */
class SecuritySmokeInstrumentation : Instrumentation() {
    override fun onCreate(arguments:Bundle?) { super.onCreate(arguments); start() }
    override fun onStart() {
        val result=Bundle()
        try {
            runBlocking {
                val store=SecureSessionStore(targetContext,"qa")
                val fixture=UserSession(accessToken="NONSECRET_FIXTURE_ACCESS",refreshToken="NONSECRET_FIXTURE_REFRESH",expiresIn=300,tokenType="bearer")
                store.saveSession(fixture)
                check(SecureSessionStore(targetContext,"qa").loadSession()?.accessToken==fixture.accessToken)
                val file=File(targetContext.noBackupFilesDir,"identity_qa/session")
                check(!file.readBytes().toString(Charsets.UTF_8).contains("NONSECRET_FIXTURE"))
                val bytes=file.readBytes();bytes[bytes.lastIndex]=(bytes.last().toInt() xor 1).toByte();file.writeBytes(bytes)
                check(store.loadSession()==null)
                store.saveSession(fixture)
                KeyStore.getInstance("AndroidKeyStore").apply { load(null);deleteEntry("astrogateway.qa.v1") }
                check(store.loadSession()==null)
                store.blockAndClearSession();store.saveSession(fixture);check(store.loadSession()==null)
                store.allowSessionWrites();store.saveCodeVerifier("NONSECRET_PKCE_FIXTURE");check(store.loadCodeVerifier()=="NONSECRET_PKCE_FIXTURE")
                store.clearPendingFlow();store.deleteSession()
                File(targetContext.noBackupFilesDir,"identity_qa").deleteRecursively()
                KeyStore.getInstance("AndroidKeyStore").apply { load(null);deleteEntry("astrogateway.qa.v1") }
            }
            result.putString("vault","PASS: persistence, encryption, tamper, key loss, logout write gate, PKCE")
            for(cycle in 1..2) {
                val launch=UUID.randomUUID().toString();val ack=CountDownLatch(1);val done=CountDownLatch(1)
                val receiver=object:BroadcastReceiver() { override fun onReceive(c:Context?,i:Intent?) {
                    if(i?.getStringExtra("launchId")!=launch)return
                    if(i.getStringExtra("type")=="ACK")ack.countDown()
                    if(i.getStringExtra("type")=="RESULT" && i.getStringExtra("message")?.startsWith("POC_RESULT_OK:")==true)done.countDown()
                } }
                targetContext.registerReceiver(receiver,IntentFilter("${targetContext.packageName}.POC_EVENT"),Context.RECEIVER_NOT_EXPORTED)
                try {
                    targetContext.startActivity(Intent(targetContext,PocUnityActivity::class.java).addFlags(Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_REORDER_TO_FRONT or Intent.FLAG_ACTIVITY_SINGLE_TOP)
                        .putExtra("launchId",launch).putExtra("missionId","mission-red-horizon-poc")
                        .putExtra("missionSessionId",UUID.randomUUID().toString()).putExtra("userId",UUID.randomUUID().toString()).putExtra("expiresAtMillis",System.currentTimeMillis()+120000))
                    check(ack.await(45,TimeUnit.SECONDS));check(done.await(10,TimeUnit.SECONDS))
                    sendKeyDownUpSync(KeyEvent.KEYCODE_BACK)
                    Thread.sleep(1500)
                } finally { targetContext.unregisterReceiver(receiver) }
                result.putString("transport_cycle_$cycle","PASS: actual Unity ACK/RESULT and Back return; synthetic context, NOT authentication")
            }
            result.putString("classification","offline storage and private transport regression only; live authentication untested")
            finish(RESULT_OK,result)
        } catch(e:Throwable) { result.putString("failure",e.javaClass.simpleName);finish(RESULT_CANCELED,result) }
    }
    companion object { const val RESULT_OK=-1;const val RESULT_CANCELED=0 }
}
