package com.sk2lb.astrogateway.poc

import android.content.Context
import android.security.keystore.KeyGenParameterSpec
import android.security.keystore.KeyProperties
import android.util.AtomicFile
import io.github.jan.supabase.auth.CodeVerifierCache
import io.github.jan.supabase.auth.SessionManager
import io.github.jan.supabase.auth.user.UserSession
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import kotlinx.coroutines.withContext
import kotlinx.serialization.json.Json
import java.io.File
import java.security.KeyStore
import javax.crypto.Cipher
import javax.crypto.KeyGenerator
import javax.crypto.SecretKey
import javax.crypto.spec.GCMParameterSpec

/** SDK session and PKCE verifier encrypted at rest. Never serializes a password. */
class SecureSessionStore(context: Context, private val namespace: String = "auth") : SessionManager, CodeVerifierCache {
    init { require(namespace.matches(Regex("[a-z]{1,16}"))) }
    private val directory = File(context.noBackupFilesDir, "identity_$namespace").apply { mkdirs() }
    private val mutex = Mutex()
    @Volatile private var sessionWritesAllowed = true
    private val json = Json { ignoreUnknownKeys = true }
    private fun key(): SecretKey {
        val store = KeyStore.getInstance("AndroidKeyStore").apply { load(null) }
        (store.getKey("astrogateway.$namespace.v1", null) as? SecretKey)?.let { return it }
        return KeyGenerator.getInstance(KeyProperties.KEY_ALGORITHM_AES, "AndroidKeyStore").apply {
            init(KeyGenParameterSpec.Builder("astrogateway.$namespace.v1", KeyProperties.PURPOSE_ENCRYPT or KeyProperties.PURPOSE_DECRYPT)
                .setBlockModes(KeyProperties.BLOCK_MODE_GCM).setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_NONE)
                .setRandomizedEncryptionRequired(true).build())
        }.generateKey()
    }
    private suspend fun write(name: String, value: String) = withContext(Dispatchers.IO) {
        mutex.withLock {
            if(name == "session" && !sessionWritesAllowed) return@withLock
            val cipher = Cipher.getInstance("AES/GCM/NoPadding").apply { init(Cipher.ENCRYPT_MODE, key()); updateAAD(name.toByteArray()) }
            val bytes = byteArrayOf(1, cipher.iv.size.toByte()) + cipher.iv + cipher.doFinal(value.toByteArray(Charsets.UTF_8))
            val file = AtomicFile(File(directory, name)); val stream = file.startWrite()
            try { stream.write(bytes); file.finishWrite(stream) } catch (e: Exception) { file.failWrite(stream); throw e }
        }
    }
    private suspend fun read(name: String): String? = withContext(Dispatchers.IO) {
        mutex.withLock {
            val file = AtomicFile(File(directory, name))
            if (!file.baseFile.exists()) return@withLock null
            try {
                val bytes = file.readFully(); require(bytes.size in 30..131072 && bytes[0].toInt() == 1 && bytes[1].toInt() == 12)
                val cipher = Cipher.getInstance("AES/GCM/NoPadding").apply {
                    init(Cipher.DECRYPT_MODE, key(), GCMParameterSpec(128, bytes.copyOfRange(2, 14))); updateAAD(name.toByteArray())
                }
                cipher.doFinal(bytes.copyOfRange(14, bytes.size)).toString(Charsets.UTF_8)
            } catch (_: Exception) { file.delete(); null } // Corrupt/invalidated storage fails closed, without logging its contents.
        }
    }
    private suspend fun delete(name: String) = withContext(Dispatchers.IO) { mutex.withLock { AtomicFile(File(directory, name)).delete() } }
    suspend fun saveLocalAccount(value: String) = write("session", value)
    suspend fun loadLocalAccount(): String? = read("session")
    override suspend fun saveSession(session: UserSession) { if(sessionWritesAllowed) write("session", json.encodeToString(UserSession.serializer(), session)) }
    suspend fun blockAndClearSession() { sessionWritesAllowed=false; deleteSession() }
    fun allowSessionWrites() { sessionWritesAllowed=true }
    override suspend fun loadSession(): UserSession? {
        val value = read("session") ?: return null
        return try { json.decodeFromString(UserSession.serializer(), value) } catch (_: Exception) { deleteSession(); null }
    }
    override suspend fun deleteSession() = delete("session")
    override suspend fun saveCodeVerifier(codeVerifier: String) = write("pkce", codeVerifier)
    override suspend fun loadCodeVerifier(): String? = read("pkce")
    override suspend fun deleteCodeVerifier() = delete("pkce")
    suspend fun setPendingFlow(kind: String) = write("pending", "$kind:${System.currentTimeMillis()}")
    suspend fun pendingFlow(): String? {
        val parts = read("pending")?.split(':') ?: return null
        val timestamp = parts.getOrNull(1)?.toLongOrNull() ?: return null
        return parts[0].takeIf { System.currentTimeMillis() - timestamp in 0..3_600_000 }
    }
    suspend fun clearPendingFlow() { delete("pending"); deleteCodeVerifier() }
}
