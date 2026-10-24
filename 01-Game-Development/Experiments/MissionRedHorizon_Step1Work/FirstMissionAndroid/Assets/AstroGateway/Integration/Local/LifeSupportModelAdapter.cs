using UnityEngine;
using AstroGateway.Stage08B;

// Lets ILifeSupportModel decide suit oxygen use without editing MarsPrototype.
// MarsPrototype keeps draining at its legacy rate; this adapter adds back the legacy amount and subtracts the model's.
// With the default PrototypeLifeSupportModel the two rates are equal, so gameplay is unchanged until an approved model is plugged in.
namespace AstroGateway.Integration
{
    [DefaultExecutionOrder(950)]
    public class LifeSupportModelAdapter : MonoBehaviour
    {
        public MarsPrototype prototype;
        public float lastModelRate;
        public Activity lastActivity;

        public static float LegacyRate(int weather) => weather == 2 ? 0.035f : 0.015f;

        public static Activity Classify(MarsPrototype p)
        {
            if (p.driving) return Activity.Driving;
            float s = Mathf.Abs(p.speed);
            return s > 3f ? Activity.Running : s > 0.1f ? Activity.Walking : Activity.Idle;
        }

        void Update()
        {
            if (!prototype) return;
            float dt = Mathf.Min(Time.deltaTime, .05f);   // Same step MarsPrototype uses.
            if (dt <= 0) return;
            var weather = IntegrationServices.Weather != null ? IntegrationServices.Weather.Current : new WeatherState { dust = (DustLevel)Mathf.Clamp(prototype.weather, 0, 2) };
            lastActivity = Classify(prototype);
            lastModelRate = IntegrationServices.LifeSupport.OxygenUsePerSecond(lastActivity, weather);
            if (prototype.oxygen <= 0) return;
            prototype.oxygen = Mathf.Clamp(prototype.oxygen + dt * (LegacyRate(prototype.weather) - lastModelRate), 0, 100);
        }
    }
}
