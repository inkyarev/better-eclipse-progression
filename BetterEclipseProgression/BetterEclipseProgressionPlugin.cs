using System.Linq;
using BepInEx;
using RoR2;

namespace BetterEclipseProgression;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class BetterEclipseProgressionPlugin : BaseUnityPlugin
{
    private const string PluginGUID = PluginAuthor + "." + PluginName;
    private const string PluginAuthor = "InkyaRev";
    private const string PluginName = "BetterEclipseProgression";
    private const string PluginVersion = "1.0.0";
    
    public void Awake()
    {
        Log.Init(Logger);

        On.RoR2.EclipseRun.OnClientGameOver += (orig, self, report) =>
        {
	        if (!report.gameEnding.isWin)
	        {
		        orig(self, report);
		        return;
	        }
	        
	        var playerInstances = PlayerCharacterMasterController.instances;
	        foreach (var playerInstance in playerInstances)
	        {
		        var currentEclipseLevel = EclipseRun.GetEclipseLevelFromRuleBook(self.ruleBook);
		        
		        var networkUser = playerInstance.networkUser;
		        
		        var eclipseLevelCompleted = EclipseRun.GetNetworkUserSurvivorCompletedEclipseLevel(networkUser, networkUser.GetSurvivorPreference());

			    if (currentEclipseLevel != EclipseRun.maxEclipseLevel ||
			        eclipseLevelCompleted != EclipseRun.maxEclipseLevel - 1)
			    {
				    orig(self, report);
				    return;
			    }

		        var localUser = networkUser?.localUser;
		        if (localUser is null) continue;

		        foreach (var survivor in SurvivorCatalog.survivorDefs)
		        {
			        if(survivor == networkUser.GetSurvivorPreference()) continue; // since orig(self, report) already gives +1 eclipse level
			        
			        var completedEclipseLevel = EclipseRun.GetNetworkUserSurvivorCompletedEclipseLevel(networkUser, survivor);
			        var nextEclipseLevelUnlock = EclipseRun.GetEclipseLevelUnlockablesForSurvivor(survivor)
				        .ElementAtOrDefault(completedEclipseLevel);
			        if (nextEclipseLevelUnlock is not null)
			        {
				        localUser.userProfile.GrantUnlockable(nextEclipseLevelUnlock);
			        }
		        }
	        }

	        orig(self, report);
        };
    }
}