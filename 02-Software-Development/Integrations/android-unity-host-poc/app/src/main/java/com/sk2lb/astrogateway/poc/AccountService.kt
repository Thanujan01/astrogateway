package com.sk2lb.astrogateway.poc
import android.net.Uri
import kotlinx.coroutines.flow.StateFlow

interface AccountService {
    val state: StateFlow<AuthUi>
    fun retrySession()
    fun signUp(name: String, email: String, password: String)
    fun signIn(email: String, password: String)
    fun resetRequest(email: String)
    fun completeCallback(uri: Uri)
    fun changeRecoveredPassword(password: String)
    fun updateDisplayName(rawName: String)
    fun signOut()
    suspend fun initializeMission(launchId: String): MissionContext
    suspend fun submitIntegrationResult(sessionId: String, launchId: String)
}
