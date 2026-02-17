using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    /// <summary>
    /// Periodic cleanup for expired LLM-generated quest instances (temporary spawns + quest unregister).
    /// </summary>
    public static class GeneratedQuestCleanup
    {
        private static bool _init;

        public static void Initialize()
        {
            if (_init)
                return;

            _init = true;
            Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(2), Tick);
            Console.WriteLine("[VystiaGeneratedQuests] Cleanup initialized.");
        }

        private static void Tick()
        {
            try
            {
                foreach (var m in World.Mobiles.Values)
                {
                    if (!(m is PlayerMobile pm) || pm.Deleted)
                        continue;

                    var a = GeneratedQuestInstanceAttachment.Get(pm);
                    if (a == null || a.Instances == null || a.Instances.Count == 0)
                        continue;

                    for (int i = a.Instances.Count - 1; i >= 0; i--)
                    {
                        var inst = a.Instances[i];
                        if (inst == null)
                        {
                            a.Instances.RemoveAt(i);
                            continue;
                        }

                        if (DateTime.UtcNow < inst.ExpiresAtUtc)
                            continue;

                        // Delete spawned mobs
                        if (inst.SpawnedSerials != null)
                        {
                            foreach (int serialValue in inst.SpawnedSerials)
                            {
                                try
                                {
                                    var mob = World.FindMobile((Serial)serialValue);
                                    if (mob != null && !mob.Deleted)
                                        mob.Delete();
                                }
                                catch
                                {
                                    // best-effort cleanup
                                }
                            }
                        }

                        // Unregister quest definition
                        var q = DynamicQuestManager.GetQuest(inst.QuestId);
                        if (q != null)
                            DynamicQuestManager.RemoveDynamicQuest(q);

                        a.Instances.RemoveAt(i);
                        pm.SendMessage($"[Vystia] A generated quest instance has expired and was cleaned up (QuestID: {inst.QuestId}).");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VystiaGeneratedQuests] Cleanup error: {ex.Message}");
            }
        }
    }
}


