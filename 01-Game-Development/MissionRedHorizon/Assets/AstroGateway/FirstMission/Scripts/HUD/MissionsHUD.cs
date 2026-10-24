using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using AstroGateway.FirstMission.Missions;
using AstroGateway.FirstMission.Nav;

// Missions HUD (Mars_FirstMission_Missions). The Explorer HUD plus in-game mission guidance:
//   - a guide popup for every mission step (mission number and title, objective, how-to text, a tip, step dots and a
//     progress bar for charge / repair / checkpoints) that slides in, flashes STEP COMPLETE and collapses into the
//     objective chip (tap the chip to reopen it);
//   - a MISSION COMPLETE banner with the reward and the next mission;
//   - a HOLD TO REPAIR control next to the generator's fault panel;
//   - the mission list in the ⋯ drawer and live states for the mission structures on the map.
namespace AstroGateway.FirstMission.HUD
{
    public class MissionsHUD : ExplorerHUD
    {
        public MissionDirector director;
        public GameObject GuideCard, Banner, RepairButton;
        public HoldControl RepairHold { get; private set; }
        public bool GuideOpen => GuideCard && GuideCard.activeSelf;
        public string GuideObjective => guideObjective ? guideObjective.text : "";
        public string BannerText => bannerTitle ? bannerTitle.text + " " + bannerBody.text : "";
        public int GuidesShown { get; private set; }
        public int BannersShown { get; private set; }
        public const float GuideSeconds = 9f, CompleteFlash = 1.3f, BannerSeconds = 3.4f;

        Text guideHeader, guideObjective, guideBody, guideHint, guideProgressLabel, bannerTitle, bannerBody, repairLabel;
        Image guideBar, guideBarBack, guideIcon, repairFill, guideFlash; RectTransform guideDots;
        GameObject guideProgress; Text flashText;
        float guideUntil, flashUntil, bannerUntil, nextStateRefresh; bool pendingGuide, resumeGuide;
        // cinematic mission title card (letterbox bars + big title) at the start of every mission
        public GameObject TitleCard;
        public MissionFx Fx { get; private set; }
        public int TitlesShown { get; private set; }
        public bool TitleShowing => TitleCard && TitleCard.activeSelf;
        public const float TitleSeconds = 2.8f;
        Text titleSmall, titleBig, titleBrief; RectTransform barTop, barBottom; CanvasGroup titleGroup; bool titlePending; float titleStart = -99;
        MissionDef shownMission; MissionStep shownStep;

        protected override void Build()
        {
            base.Build();
            if (!director) director = FindAnyObjectByType<MissionDirector>();
            BuildGuideCard();
            BuildBanner();
            BuildSurvival();
            BuildFail();
            BuildRepairButton();
            BuildTitleCard();
            if (director)
            {
                Fx = gameObject.AddComponent<MissionFx>(); Fx.Init(director);
                director.ui = this;
                director.StepStarted += (m, s) => { pendingGuide = true; if (director.StepIndex == 0) titlePending = true; };
                director.StepCompleted += (m, s) => { flashUntil = Time.unscaledTime + CompleteFlash; ShowFlash("STEP COMPLETE", s.objective); };
                director.MissionCompleted += m => ShowBanner("MISSION COMPLETE", m.title + "\nReward: " + m.reward + NextLine());
                director.AllMissionsCompleted += () => ShowBanner("YOU SURVIVED THE STORM", "Life support is running, the beacons are lit and the landing site is recorded. The crew is safe.\nProgress saved on this device (local, unscored).");
                director.StormIncoming += () => ShowBanner("STORM WARNING", $"A second dust storm arrives in {director.StormEta / 60:0}:{director.StormEta % 60:00}. Light the beacons and get to shelter.", HudTokens.Amber, "warning");
                director.MissionFailed += why => ShowFail(why);
            }
            PausedOverlay.transform.SetAsLastSibling();
        }

        string NextLine()
        {
            if (director.AllComplete || director.Current == null) return "";
            return "\nNext: MISSION " + (director.MissionIndex + 1) + " • " + director.Current.title;
        }

        // ---------------- guide popup ----------------
        void BuildGuideCard()
        {
            var card = Glass("Mission guide", Safe, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-616, -620), new Vector2(-16, -150), .9f);
            GuideCard = card.gameObject;
            var accent = Img("Accent", card, new Vector2(0, 0), new Vector2(0, 1), null, HudTokens.Orange); accent.rectTransform.offsetMin = new Vector2(0, 18); accent.rectTransform.offsetMax = new Vector2(5, -18);
            guideIcon = Icon("Mission icon", card, "flag", new Vector2(0, 1), new Vector2(0, 1), HudTokens.Orange); guideIcon.rectTransform.sizeDelta = new Vector2(34, 34); guideIcon.rectTransform.anchoredPosition = new Vector2(42, -36);
            guideHeader = Tx("Mission header", card, "", 18, TextAnchor.MiddleLeft, HudTokens.Orange, new Vector2(0, 1), new Vector2(1, 1), FontStyle.Bold);
            guideHeader.rectTransform.offsetMin = new Vector2(70, -58); guideHeader.rectTransform.offsetMax = new Vector2(-24, -14);
            guideObjective = Tx("Step objective", card, "", 27, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(0, 1), new Vector2(1, 1), FontStyle.Bold);
            guideObjective.rectTransform.offsetMin = new Vector2(28, -132); guideObjective.rectTransform.offsetMax = new Vector2(-24, -64);
            guideBody = Tx("Step guide", card, "", 19, TextAnchor.UpperLeft, new Color(1, 1, 1, .88f), new Vector2(0, 1), new Vector2(1, 1));
            guideBody.rectTransform.offsetMin = new Vector2(28, -238); guideBody.rectTransform.offsetMax = new Vector2(-24, -136);
            guideHint = Tx("Step tip", card, "", 17, TextAnchor.UpperLeft, HudTokens.Cyan, new Vector2(0, 1), new Vector2(1, 1));
            guideHint.rectTransform.offsetMin = new Vector2(28, -300); guideHint.rectTransform.offsetMax = new Vector2(-24, -244);

            guideProgress = R("Progress", card, new Vector2(0, 0), new Vector2(1, 0)).gameObject;
            var gp = (RectTransform)guideProgress.transform; gp.offsetMin = new Vector2(28, 112); gp.offsetMax = new Vector2(-24, 150);
            guideProgressLabel = Tx("Progress label", gp, "", 15, TextAnchor.MiddleLeft, HudTokens.TextDim, new Vector2(0, .5f), new Vector2(1, 1), FontStyle.Bold);
            guideBarBack = Img("Bar back", gp, new Vector2(0, 0), new Vector2(1, .38f), HudFx.Round(30), new Color(1, 1, 1, .12f)); guideBarBack.type = Image.Type.Sliced;
            guideBar = Img("Bar fill", guideBarBack.transform, Vector2.zero, new Vector2(0, 1), HudFx.Round(30), HudTokens.Cyan); guideBar.type = Image.Type.Sliced;

            guideDots = R("Step dots", card, new Vector2(0, 0), new Vector2(0, 0)); guideDots.offsetMin = new Vector2(28, 34); guideDots.offsetMax = new Vector2(330, 80);
            var got = PillButton("Guide got it", card, "check", "GOT IT", new Vector2(1, 0), new Vector2(-226, 14), new Vector2(-20, 98), () => { guideUntil = 0; GuideCard.SetActive(false); }, HudTokens.Cyan);
            got.name = "Guide got it";

            guideFlash = Img("Step complete flash", card, Vector2.zero, Vector2.one, HudFx.GlassFill, new Color(.08f, .30f, .18f, .94f)); guideFlash.type = Image.Type.Sliced; guideFlash.raycastTarget = false;
            var fi = Icon("Check", guideFlash.transform, "check", new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudTokens.Green); fi.rectTransform.sizeDelta = new Vector2(84, 84); fi.rectTransform.anchoredPosition = new Vector2(0, 70);
            flashText = Tx("Flash text", guideFlash.transform, "", 24, TextAnchor.UpperCenter, HudTokens.Text, new Vector2(.06f, .1f), new Vector2(.94f, .55f), FontStyle.Bold);
            guideFlash.gameObject.SetActive(false);

            var sl = GuideCard.AddComponent<HudSlideIn>(); sl.from = new Vector2(120, 0); sl.reduced = () => mission.Settings.reducedMotion;
            GuideCard.SetActive(false);
        }

        void ShowGuide()
        {
            var m = director.Current; var s = director.Step; if (m == null || s == null) return;
            shownMission = m; shownStep = s;
            int si = director.StepIndex;
            guideHeader.text = $"MISSION {director.MissionIndex + 1} / {director.Missions.Count} • {m.title}   STEP {si + 1} / {m.steps.Count}";
            guideObjective.text = s.objective;
            guideBody.text = si == 0 ? m.brief + "\n" + s.guide : s.guide;
            guideHint.text = "TIP  " + s.hint;
            guideIcon.sprite = HudIcons.Get(s.repair ? "wrench" : m.id.StartsWith("m5") ? "gate" : m.id.StartsWith("m2") ? "bolt" : m.id.StartsWith("m4") ? "science" : m.id.StartsWith("m6") ? "rover" : "flag");
            foreach (Transform c in guideDots) Destroy(c.gameObject);
            for (int i = 0; i < m.steps.Count; i++)
            {
                var d = Img("Dot", guideDots, new Vector2(0, .5f), new Vector2(0, .5f), HudFx.Circle, i < si ? HudTokens.Green : i == si ? HudTokens.Orange : new Color(1, 1, 1, .25f));
                d.rectTransform.sizeDelta = Vector2.one * (i == si ? 20 : 14); d.rectTransform.anchoredPosition = new Vector2(12 + i * 30, 0);
            }
            guideFlash.gameObject.SetActive(false);
            GuideCard.SetActive(true); GuideCard.GetComponent<HudSlideIn>().enabled = true;
            GuidesShown++;
            guideUntil = Time.unscaledTime + GuideSeconds;
        }

        void ShowFlash(string title, string objective)
        {
            if (!GuideCard.activeSelf) { GuideCard.SetActive(true); }
            guideFlash.gameObject.SetActive(true);
            flashText.text = title + "\n<size=19>" + objective + "</size>";
            flashText.supportRichText = true;
        }

        public void OpenGuide() { if (director && !director.AllComplete) ShowGuide(); }
        protected override void ObjectiveTapped() { if (director && !director.AllComplete) OpenGuide(); else base.ObjectiveTapped(); }

        // ---------------- mission title card ----------------
        void BuildTitleCard()
        {
            var root = R("Mission title card", Safe, Vector2.zero, Vector2.one); TitleCard = root.gameObject;
            titleGroup = TitleCard.AddComponent<CanvasGroup>(); titleGroup.blocksRaycasts = false; titleGroup.interactable = false;
            barTop = Img("Letterbox top", root, new Vector2(0, 1), new Vector2(1, 1), null, new Color(0, 0, 0, .92f)).rectTransform; barTop.pivot = new Vector2(.5f, 1);
            barBottom = Img("Letterbox bottom", root, new Vector2(0, 0), new Vector2(1, 0), null, new Color(0, 0, 0, .92f)).rectTransform; barBottom.pivot = new Vector2(.5f, 0);
            var glow = Img("Title glow", root, new Vector2(.5f, .62f), new Vector2(.5f, .62f), HudFx.Glow, new Color(0, 0, 0, .55f)); glow.rectTransform.sizeDelta = new Vector2(1300, 380);
            titleSmall = Tx("Title mission", root, "", 26, TextAnchor.MiddleCenter, HudTokens.Orange, new Vector2(.2f, .70f), new Vector2(.8f, .76f), FontStyle.Bold);
            titleBig = Tx("Title name", root, "", 72, TextAnchor.MiddleCenter, HudTokens.Text, new Vector2(.1f, .58f), new Vector2(.9f, .70f), FontStyle.Bold);
            titleBrief = Tx("Title brief", root, "", 24, TextAnchor.MiddleCenter, new Color(1, 1, 1, .85f), new Vector2(.15f, .51f), new Vector2(.85f, .58f));
            foreach (var t in new[] { titleSmall, titleBig, titleBrief }) { var sh = t.gameObject.AddComponent<Shadow>(); sh.effectColor = new Color(0, 0, 0, .8f); sh.effectDistance = new Vector2(2, -2); }
            TitleCard.SetActive(false);
        }

        void ShowTitle()
        {
            var m = director.Current; if (m == null) return;
            titleSmall.text = $"MISSION {director.MissionIndex + 1} OF {director.Missions.Count}";
            titleBig.text = m.title; titleBrief.text = m.brief;
            titleStart = Time.unscaledTime; TitleCard.SetActive(true); TitleCard.transform.SetAsLastSibling(); PausedOverlay.transform.SetAsLastSibling();
            TitlesShown++;
        }

        void UpdateTitle(float now, bool hide)
        {
            if (!TitleCard.activeSelf) return;
            float t = now - titleStart;
            if (t >= TitleSeconds || hide) { TitleCard.SetActive(false); return; }
            float a = Mathf.Clamp01(t / .35f) * Mathf.Clamp01((TitleSeconds - t) / .45f);
            titleGroup.alpha = a;
            float bars = (mission.Settings.reducedMotion ? 1 : 1 - Mathf.Pow(1 - a, 3)) * 96;
            barTop.sizeDelta = new Vector2(0, bars); barBottom.sizeDelta = new Vector2(0, bars);
            if (!mission.Settings.reducedMotion) titleBig.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.06f, 1f, Mathf.Clamp01(t / 2.2f));
        }

        // ---------------- mission complete banner ----------------
        void BuildBanner()
        {
            var b = Glass("Mission banner", Safe, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-420, 40), new Vector2(420, 290), .92f);
            Banner = b.gameObject;
            var glow = Img("Banner glow", b, Vector2.zero, Vector2.one, HudFx.Glow, new Color(HudTokens.Green.r, HudTokens.Green.g, HudTokens.Green.b, .25f)); glow.raycastTarget = false;
            glow.rectTransform.offsetMin = new Vector2(-60, -60); glow.rectTransform.offsetMax = new Vector2(60, 60); glow.transform.SetAsFirstSibling();
            var ic = Icon("Banner check", b, "check", new Vector2(0, .5f), new Vector2(0, .5f), HudTokens.Green); ic.rectTransform.sizeDelta = new Vector2(96, 96); ic.rectTransform.anchoredPosition = new Vector2(84, 20);
            bannerTitle = Tx("Banner title", b, "", 40, TextAnchor.MiddleLeft, HudTokens.Green, new Vector2(0, .62f), new Vector2(1, .95f), FontStyle.Bold);
            bannerTitle.rectTransform.offsetMin = new Vector2(150, 0); bannerTitle.rectTransform.offsetMax = new Vector2(-24, 0);
            bannerBody = Tx("Banner body", b, "", 21, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(0, .06f), new Vector2(1, .62f));
            bannerBody.rectTransform.offsetMin = new Vector2(150, 0); bannerBody.rectTransform.offsetMax = new Vector2(-24, 0);
            var sl = Banner.AddComponent<HudSlideIn>(); sl.from = new Vector2(0, -60); sl.seconds = .35f; sl.reduced = () => mission.Settings.reducedMotion;
            var tap = Banner.AddComponent<Button>(); tap.onClick.AddListener(() => { bannerUntil = 0; Banner.SetActive(false); });
            Banner.SetActive(false);
        }

        void ShowBanner(string title, string body) => ShowBanner(title, body, HudTokens.Green, "check");
        void ShowBanner(string title, string body, Color c, string icon)
        {
            bannerTitle.text = title; bannerBody.text = body; bannerTitle.color = c;
            var ic = Banner.transform.Find("Banner check").GetComponent<Image>(); ic.sprite = HudIcons.Get(icon); ic.color = c;
            Banner.transform.Find("Banner glow").GetComponent<Image>().color = new Color(c.r, c.g, c.b, .25f);
            Banner.SetActive(true); BannersShown++;
            bannerUntil = Time.unscaledTime + (director.AllComplete ? 6f : BannerSeconds);
        }

        // ---------------- survival panel + failure ----------------
        public GameObject SurvivalPanel, FailCard; public Button RetryButton;
        public string SurvivalText => survO2 ? survO2.text + " | " + survSecond.text : "";
        Text survO2, survSecond, failTitle, failBody; Image survBar, survBar2; RectTransform survRt;

        void BuildSurvival()
        {
            var im = Img("Survival panel", Safe, new Vector2(0, 1), new Vector2(0, 1), HudFx.Round(30), HudFx.ControlFill); im.type = Image.Type.Sliced;
            survRt = im.rectTransform; SurvivalPanel = im.gameObject;
            Hairline(im.transform, 30, new Color(HudTokens.Red.r, HudTokens.Red.g, HudTokens.Red.b, .35f));
            survO2 = Tx("Base O2", im.transform, "", 17, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(0, .5f), new Vector2(.52f, 1), FontStyle.Bold); survO2.rectTransform.offsetMin = new Vector2(24, 0);
            var b1 = Img("O2 bar back", im.transform, new Vector2(.53f, .62f), new Vector2(.95f, .80f), HudFx.Round(30), new Color(1, 1, 1, .12f)); b1.type = Image.Type.Sliced;
            survBar = Img("O2 bar", b1.transform, Vector2.zero, new Vector2(.4f, 1), HudFx.Round(30), HudTokens.Cyan); survBar.type = Image.Type.Sliced;
            survSecond = Tx("Second", im.transform, "", 17, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(0, 0), new Vector2(.52f, .5f), FontStyle.Bold); survSecond.rectTransform.offsetMin = new Vector2(24, 0);
            var b2 = Img("Second bar back", im.transform, new Vector2(.53f, .2f), new Vector2(.95f, .38f), HudFx.Round(30), new Color(1, 1, 1, .12f)); b2.type = Image.Type.Sliced;
            survBar2 = Img("Second bar", b2.transform, Vector2.zero, new Vector2(.4f, 1), HudFx.Round(30), HudTokens.Amber); survBar2.type = Image.Type.Sliced;
        }

        void BuildFail()
        {
            var shade = Img("Mission failed", Safe, Vector2.zero, Vector2.one, null, new Color(.15f, 0, 0, .55f), true); FailCard = shade.gameObject;
            var c = Glass("Fail card", shade.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-440, -170), new Vector2(440, 200), .94f);
            var ic = Icon("Fail icon", c, "warning", new Vector2(0, 1), new Vector2(0, 1), HudTokens.Red); ic.rectTransform.sizeDelta = new Vector2(80, 80); ic.rectTransform.anchoredPosition = new Vector2(76, -76);
            failTitle = Tx("Fail title", c, "", 40, TextAnchor.MiddleLeft, HudTokens.Red, new Vector2(0, .72f), new Vector2(1, .95f), FontStyle.Bold); failTitle.rectTransform.offsetMin = new Vector2(136, 0);
            failBody = Tx("Fail body", c, "", 22, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(0, .34f), new Vector2(1, .70f)); failBody.rectTransform.offsetMin = new Vector2(40, 0); failBody.rectTransform.offsetMax = new Vector2(-30, 0);
            RetryButton = PillButton("Retry mission", c, "recenter2", "RETRY MISSION", new Vector2(.5f, 0), new Vector2(-170, 24), new Vector2(170, 112), () => director.Retry(), HudTokens.Orange);
            FailCard.SetActive(false);
        }

        void ShowFail(string why)
        {
            var parts = why.Split('\n');
            failTitle.text = "MISSION FAILED"; failBody.text = parts[0] + "\n" + (parts.Length > 1 ? parts[1] : "") + "\nRetry: you keep your progress in this mission, with a little extra oxygen or time.";
            FailCard.SetActive(true); FailCard.transform.SetAsLastSibling(); PausedOverlay.transform.SetAsLastSibling();
        }

        static string Clock(float s) => $"{Mathf.FloorToInt(s / 60)}:{Mathf.FloorToInt(s % 60):00}";

        void UpdateSurvival(bool driving, bool hide)
        {
            bool show = !director.AllComplete && !hide;
            SurvivalPanel.SetActive(show);
            if (FailCard.activeSelf && !director.Failed) FailCard.SetActive(false);
            if (!show) return;
            survRt.offsetMin = new Vector2(16, driving ? -322 : -208); survRt.offsetMax = new Vector2(560, driving ? -236 : -122);
            float o2 = director.BaseO2; bool low = o2 < 15 && !director.Repaired;
            float pulse = low && !mission.Settings.reducedMotion ? .55f + .45f * Mathf.Sin(Time.unscaledTime * 8) : 1;
            survO2.text = director.Repaired ? $"BASE O2  {o2:0}%  RESTORING" : $"BASE O2  {o2:0}%  ON RESERVE";
            survO2.color = low ? new Color(HudTokens.Red.r, HudTokens.Red.g, HudTokens.Red.b, pulse) : o2 < 30 && !director.Repaired ? HudTokens.Amber : HudTokens.Text;
            survBar.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(o2 / 100f), 1); survBar.color = director.Repaired ? HudTokens.Green : low ? HudTokens.Red : o2 < 30 ? HudTokens.Amber : HudTokens.Cyan;
            if (director.StormActive)
            {
                float eta = director.StormEta; bool close = eta < 45;
                survSecond.text = $"STORM IN  {Clock(eta)}"; survSecond.color = close ? HudTokens.Red : HudTokens.Amber;
                survBar2.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(eta / MissionDirector.StormSeconds), 1); survBar2.color = close ? HudTokens.Red : HudTokens.Amber;
            }
            else
            {
                float b = director.Battery; survSecond.text = $"ROVER BATTERY  {b:0}%"; survSecond.color = b < 40 ? HudTokens.Amber : HudTokens.Text;
                survBar2.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(b / 100f), 1); survBar2.color = b < 40 ? HudTokens.Amber : HudTokens.Cyan;
            }
        }

        // ---------------- repair control ----------------
        void BuildRepairButton()
        {
            var im = Img("Hold to repair", AstronautControls.transform, new Vector2(1, 0), new Vector2(1, 0), HudFx.Round(30), HudFx.ControlFill, true); im.type = Image.Type.Sliced;
            im.rectTransform.offsetMin = new Vector2(-560, 268); im.rectTransform.offsetMax = new Vector2(-216, 372);
            Hairline(im.transform, 30, new Color(HudTokens.Orange.r, HudTokens.Orange.g, HudTokens.Orange.b, .6f));
            repairFill = Img("Repair fill", im.transform, Vector2.zero, new Vector2(0, 1), HudFx.Round(30), new Color(HudTokens.Orange.r, HudTokens.Orange.g, HudTokens.Orange.b, .45f)); repairFill.type = Image.Type.Sliced; repairFill.raycastTarget = false;
            var ic = Icon("Icon", im.transform, "wrench", new Vector2(0, .5f), new Vector2(0, .5f), HudTokens.Orange); ic.rectTransform.sizeDelta = new Vector2(46, 46); ic.rectTransform.anchoredPosition = new Vector2(46, 0);
            repairLabel = Tx("Repair label", im.transform, "HOLD TO REPAIR", 21, TextAnchor.MiddleLeft, HudTokens.Text, Vector2.zero, Vector2.one, FontStyle.Bold); repairLabel.rectTransform.offsetMin = new Vector2(84, 0);
            RepairHold = im.gameObject.AddComponent<HoldControl>();
            RepairButton = im.gameObject; RepairButton.SetActive(false);
        }

        // ---------------- drawer + objective ----------------
        protected override string MissionSection()
        {
            if (!director || director.Missions.Count == 0) return base.MissionSection();
            var b = new StringBuilder();
            b.AppendLine($"MISSIONS • {(director.AllComplete ? director.Missions.Count : director.MissionIndex)} of {director.Missions.Count} complete (saved on this device, not scored)\n");
            for (int i = 0; i < director.Missions.Count; i++)
            {
                var m = director.Missions[i];
                string st = director.AllComplete || i < director.MissionIndex ? "DONE     " : i == director.MissionIndex ? "ACTIVE   " : "LOCKED   ";
                b.AppendLine($"{st}{i + 1}. {m.title} — {m.brief}");
                if (!director.AllComplete && i == director.MissionIndex)
                    for (int k = 0; k < m.steps.Count; k++) b.AppendLine($"           {(k < director.StepIndex ? "done" : k == director.StepIndex ? "NOW " : "    ")} {m.steps[k].objective}");
            }
            b.AppendLine("\nTap the objective chip to reopen the guide.\n\nScience objectives (sensor records):");
            foreach (var o in mission.Objectives().Where(o => !o.info)) b.AppendLine((o.done ? "DONE     " : "PENDING  ") + o.text);
            return b.ToString();
        }

        protected override void Refresh()
        {
            base.Refresh();
            if (!objectiveText || !director || !director.Ready) return;
            if (director.AllComplete) { objectiveText.text = "All missions complete • free exploration"; return; }
            var s = director.Step; if (s == null) return;
            string pct = s.progress != null ? $" • {s.progress() * 100:0}%" : "";
            objectiveText.text = $"M{director.MissionIndex + 1} • {s.objective}{pct}";
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (!built || !director || !director.Ready) return;
            var p = hud.game.prototype;
            bool paused = hud.environment.paused || hud.modalOpen;
            bool overlay = MapOverlay.activeSelf || Drawer.activeSelf || RecordView.activeSelf;
            bool intro = Cinematics && Cinematics.IntroActive;
            float now = Time.unscaledTime;

            // mission title card: after the intro and any MISSION COMPLETE banner, before the step guide
            if (titlePending && !intro && !Banner.activeSelf && !(paused || overlay)) { titlePending = false; ShowTitle(); }
            UpdateTitle(now, paused || overlay);
            if (pendingGuide && !intro && !titlePending && !TitleShowing && now >= flashUntil && !(paused || overlay)) { pendingGuide = false; ShowGuide(); }
            if (guideFlash.gameObject.activeSelf && now >= flashUntil && !pendingGuide) { guideFlash.gameObject.SetActive(false); if (director.AllComplete) GuideCard.SetActive(false); }
            var s = director.Step;
            // keep the card open while a progress step is actively progressing (charging on the pad, repairing)
            bool busy = s != null && s.progress != null && ((director.OnPad && s.progressLabel.StartsWith("CHARGE")) || (s.repair && director.InRepairRange));
            if (GuideCard.activeSelf && now > guideUntil && now >= flashUntil && !busy) GuideCard.SetActive(false);
            if (busy && !GuideCard.activeSelf && !overlay && !paused && !intro && Current == Tab.Drive) { GuideCard.SetActive(true); guideUntil = now + 2; }
            bool hide = paused || overlay || intro || Current != Tab.Drive;
            // Hidden behind the map/drawer/pause/science panel; comes back (sliding in) when they close.
            if (hide && GuideCard.activeSelf) { GuideCard.SetActive(false); resumeGuide = true; }
            else if (!hide && resumeGuide) { resumeGuide = false; if (now < guideUntil + 1 || busy) { GuideCard.SetActive(true); guideUntil = Mathf.Max(guideUntil, now + 3); } }
            if (hide) guideUntil = Mathf.Max(guideUntil, now + 1);

            // progress bar
            bool hasProgress = s != null && s.progress != null && shownStep == s;
            guideProgress.SetActive(hasProgress);
            if (hasProgress)
            {
                float v = Mathf.Clamp01(s.progress());
                guideBar.rectTransform.anchorMax = new Vector2(Mathf.Lerp(guideBar.rectTransform.anchorMax.x, v, .2f), 1);
                guideBar.color = v >= 1 ? HudTokens.Green : s.repair ? HudTokens.Orange : HudTokens.Cyan;
                string extra = s.progressLabel.StartsWith("CHARGE") ? $"   battery {director.Battery:0}%{(director.OnPad ? (Mathf.Abs(p.speed) < .3f ? "  • CHARGING" : "  • STOP TO CHARGE") : "  • NOT ON PAD")}" : s.repair ? (director.InRepairRange ? "   • HOLD REPAIR" : "   • MOVE CLOSER TO THE PANEL") : $"   {director.CheckpointsPassed} / {director.checkpoints.Count}";
                guideProgressLabel.text = $"{s.progressLabel}  {v * 100:0}%{extra}";
            }

            UpdateSurvival(p.driving, paused || overlay || intro);
            // banner
            if (Banner.activeSelf && (now > bannerUntil || overlay)) Banner.SetActive(false);
            if (Banner.activeSelf) Banner.transform.SetAsLastSibling();

            // repair control
            bool showRepair = s != null && s.repair && director.InRepairRange && !p.driving && !overlay && !paused;
            if (RepairButton.activeSelf != showRepair) RepairButton.SetActive(showRepair);
            director.RepairHeld = showRepair && RepairHold.Held;
            if (showRepair && director.generator)
            {
                repairFill.rectTransform.anchorMax = new Vector2(director.generator.RepairProgress, 1);
                repairLabel.text = RepairHold.Held ? $"REPAIRING… {director.generator.RepairProgress * 100:0}%" : "HOLD TO REPAIR";
            }

            // live states for mission structures on the map
            if (now >= nextStateRefresh)
            {
                nextStateRefresh = now + .5f;
                foreach (var st in MissionStructure.All)
                {
                    var l = Registry.Locations.FirstOrDefault(x => x.id == st.id); if (l == null) continue;
                    l.state = director.StateOf(st);
                    if (!director.AllComplete && director.Step != null && director.Step.targetId == st.id) l.mission = "ACTIVE MISSION TARGET • " + director.Step.objective;
                    else l.mission = "Mission structure";
                }
            }
        }
    }
}
