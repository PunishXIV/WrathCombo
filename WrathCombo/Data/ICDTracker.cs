using System;
using System.Collections.Generic;
using System.Linq;
namespace WrathCombo.Data;

internal class ICDTracker
{
    public uint StatusID;

    public ulong GameObjectId;

    public DateTime ICDClearedTime;

    public int TimesApplied;

    public ICDTracker(uint statusID, ulong gameObjectId, TimeSpan icdDuration)
    {
        StatusID = statusID;
        GameObjectId = gameObjectId;
        ICDClearedTime = DateTime.Now + icdDuration;
        TimesApplied = 1;
    }

    public static List<ICDTracker> Trackers = new();

    public static void ClearExpiredTrackers() => Trackers.RemoveAll(x => x.ICDClearedTime < DateTime.Now);

    public static bool StatusIsExpired(uint statusId, ulong gameObjectId)
    {
        ClearExpiredTrackers();
        return !Trackers.Any(x => x.GameObjectId == gameObjectId && x.StatusID == statusId);
    }

    public static TimeSpan TimeUntilExpired(uint statusId, ulong gameObjectId)
    {
        ClearExpiredTrackers();
        if (StatusIsExpired(statusId, gameObjectId))
            return TimeSpan.Zero;

        return Trackers.First(x => x.GameObjectId == gameObjectId && x.StatusID == statusId).ICDClearedTime - DateTime.Now;
    }

    public TimeSpan TimeUntilExpired()
    {
        var remaining = ICDClearedTime - DateTime.Now;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
        
    public static int NumberOfTimesApplied(uint statusId, ulong gameObjectId)
    {
        ClearExpiredTrackers();
        return Trackers.FirstOrDefault(x => x.GameObjectId == gameObjectId && x.StatusID == statusId)?.TimesApplied ?? 0;
    }
}