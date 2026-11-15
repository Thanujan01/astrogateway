import videos from './videos'
import './tutorials.css'

/* ---------------------------------------------------------------------------
 * Physical training tutorials: external YouTube videos on astronaut fitness,
 * selection and training, embedded for reference. These are not part of
 * AstroGateway's own training record -- nothing here is scored or synced.
 * ------------------------------------------------------------------------- */

export default function Tutorials() {
  return (
    <section className="agt">
      <div className="ag-eyebrow">ASTRONAUT TRAINING LIBRARY</div>
      <h1>Physical training tutorials.</h1>
      <p className="ag-lead">
        Curated videos on astronaut fitness, selection and training. These play directly
        from YouTube; viewing them is not part of your AstroGateway training record.
      </p>

      <div className="agt-grid">
        {videos.map(video => (
          <article key={video.id} className="agt-card">
            <div className="agt-frame">
              <iframe
                src={`https://www.youtube-nocookie.com/embed/${video.id}`}
                title={video.title}
                loading="lazy"
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                allowFullScreen
              />
            </div>
            <h3>{video.title}</h3>
            <p className="ag-small">{video.channel}</p>
          </article>
        ))}
      </div>
    </section>
  )
}
