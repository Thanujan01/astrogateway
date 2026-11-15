import { ExternalLink, Smartphone } from 'lucide-react'

/* ---------------------------------------------------------------------------
 * Download App: links out to the Android APK folder on Google Drive. There is
 * no file hosted by this app -- just a redirect to the shared Drive folder.
 * ------------------------------------------------------------------------- */

const DRIVE_FOLDER_URL = 'https://drive.google.com/drive/folders/1DsE8mD1pNGBWLC_IGZgl_6kA11O8pDza?usp=sharing'

export default function DownloadApp() {
  return (
    <section className="ag-profile-card">
      <div className="ag-eyebrow">ANDROID APP</div>
      <h1>Download the app.</h1>
      <p className="ag-lead">
        Get the AstroGateway Android build from Google Drive.
      </p>

      <div className="ag-actions">
        <a className="ag-primary" href={DRIVE_FOLDER_URL} target="_blank" rel="noopener noreferrer">
          <Smartphone size={18}/> Open download folder <ExternalLink size={16}/>
        </a>
      </div>

      <div className="ag-card-rule"/>
      <p className="ag-small">
        This opens a Google Drive folder in a new tab, not a file hosted by this app. Download
        the APK listed there, then allow installs from unknown sources on your Android device if
        prompted.
      </p>
    </section>
  )
}
