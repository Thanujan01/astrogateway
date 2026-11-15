/* ---------------------------------------------------------------------------
 * Curated external training videos. Titles/channels are fetched once from
 * YouTube's public oEmbed endpoint and stored statically here -- the page
 * never calls out to YouTube except to embed the player itself.
 * ------------------------------------------------------------------------- */

export interface Tutorial {
  id: string
  title: string
  channel: string
}

const videos: Tutorial[] = [
  { id: 'xuinUzZUTno', title: 'Why NASA Astronauts Need 2 Years of Training Just to Survive Space', channel: 'Doctor Clarify' },
  { id: 'UyXS2tYggiE', title: 'Surprisingly STEM: Astronaut Fitness Trainer', channel: 'Learn With NASA' },
  { id: 'T4dfXIN0MOs', title: 'What It Takes to Become an Astronaut | How Does NASA Select Astronauts?', channel: 'Explore Mode' },
  { id: 'tGDhTNZcmsg', title: 'What It Takes To Become An Astronaut? Astronaut Training Explained', channel: 'The World Of Science' },
  { id: 'AEblL0w2ODQ', title: "Why NASA Astronauts Train in America's Largest Swimming Pool", channel: 'Smithsonian Channel Aviation Nation' },
  { id: '_v9pVNTOzlQ', title: 'What Is the Oldest Language in the World? Top 10 Living Languages Ranked', channel: 'Airlearn Language Show' },
  { id: 'bchzUssFPrE', title: 'NASA Space Psychology Explained', channel: 'Doctor Ali Mattu' },
  { id: 'xt9CzCDRORI', title: 'Unboxing a NASA Astronaut Exam', channel: 'Tibees' },
  { id: 'OL9cpxuN7NY', title: 'Astronaut Scott Kelly on the Psychological Challenges of Going to Mars', channel: 'The Verge' },
  { id: 'BBOJ52gx_hc', title: 'Only 1% Pass These NASA Astronaut Tests: Every NASA Astronaut Test Explained', channel: 'The Space Explained' },
]

export default videos
