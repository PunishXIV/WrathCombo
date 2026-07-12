using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.MathHelpers;
using System.Collections.Frozen;
using System.Linq;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;


namespace WrathCombo.Data.BattleData
{
    internal static partial class BattleData
    {
        private static bool LoadDT()
        {
            bool dataLoaded = true;
            switch (_territoryID)
            {
                case 1241: // Cloud of Darkness Chaotic - Sphere of Naught
                           // Cloud of Darkness = 17950
                           // Stygian Shadow = 17951
                           // Atomos = 17952
                           // Inner Darkness = 4177
                           // Outer Darkness = 4178
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        // Cloud of Darkness = 17950
                        // Stygian Shadow = 17951
                        // Atomos = 17952
                        // Inner Darkness = 4177
                        // Outer Darkness = 4178
                        if (targetID is 17950 && HasStatusEffect(4178, null, true)) return Invincible.True; // If on the platforms
                        if (targetID is 17951 or 17952 && HasStatusEffect(4177, null, true)) return Invincible.True; // If on tiles

                        return Invincible.False;
                    };
                    break;

                case 1248: // Jeuno 1 Ark Angels
                           // ArkAngel HM = 18049
                           // ArkAngel MR = 18051 (A)
                           // ArkAngel GK = 18053 (B)
                           // ArkAngel TT = 18052 (C)
                    _invincibleCheck = (_, targetID, targetStatuses) =>
                    {
                        if (targetID is 18049 && targetStatuses.Contains(4410)) return Invincible.True;

                        if (targetID is 18051 or 18052 or 18053)
                        {
                            if (HasStatusEffect(4192)) return Result(targetID != 18051); // Alliance A Red Epic
                            if (HasStatusEffect(4194)) return Result(targetID != 18053); // Alliance B Yellow Fated
                            if (HasStatusEffect(4196)) return Result(targetID != 18052); // Alliance C Blue Vaunted
                        }
                        return Invincible.False;
                    };
                    break;

                case 1263: // M8S
                           // Wolf of Wind = 18219
                           // Wolf of Stone = 18225
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        if (targetID is 18219 or 18225)
                        {
                            if (HasStatusEffect(4389)) return Result(targetID != 18225); // Target Wolf of Wind
                            if (HasStatusEffect(4390)) return Result(targetID != 18219); // Target Wolf of Stone
                        }
                        return Invincible.False;
                    };
                    break;

                case 1290: //Pilgrim's Traverse
                           // Eminent Grief = 18666
                           // Devoured Eater = 18667
                    _invincibleCheck = (_, targetID, _) =>
                    {
                        if (targetID is 18666 or 18667)
                        {
                            if (HasStatusEffect(4559)) return Result(targetID != 18667); // Target Eminent Grief
                            if (HasStatusEffect(4560)) return Result(targetID != 18666); // Target Devoured Eater
                        }
                        return Invincible.False;
                    };
                    break;

                case 1292: //Meso Terminal
                           // Bloody Headsman = 18576 a
                           // Pale Headsman = 18577 b
                           // Ravenous Headsman = 18578 y
                           // Pestilent Headsman = 18579 d
                           // Hellmaker = 18642

                    // Alpha = 4542 Player / 4546 Boss
                    // Beta = 4543 Player / 4547 Boss
                    // Gamma = 4544 Player / 4548 Boss
                    // Delta = 4545 Player / 4549 Boss
                    _invincibleCheck = (target, targetID, _) =>
                    {
                        if (targetID is 18576 or 18577 or 18578 or 18579 or 18642)
                        {
                            if (HasStatusEffect(3065)) return Result(targetID != 18642 || GetTargetDistance(target) > 20);  // Hellmaker checking for fire floor debuff
                            if (HasStatusEffect(4542)) return Result(targetID != 18576); // Alpha
                            if (HasStatusEffect(4543)) return Result(targetID != 18577); // Beta
                            if (HasStatusEffect(4544)) return Result(targetID != 18578); // Gamma
                            if (HasStatusEffect(4545)) return Result(targetID != 18579); // Delta
                        }
                        return Invincible.False;
                    };
                    break;

                case 1314: // Mistwake
                    _ignoreRaidwideAIDs = new uint[] {
                        43330 // Treno Catoblepas Bedeviling Light
                    }.ToFrozenSet();
                    break;

                case 1323: //M10S
                           // 19287 Red Hot
                           // 19288 Deep Blue
                    _invincibleCheck = (target, targetID, _) =>
                        Result(targetID is 19287 or 19288 && GetTargetCurrentHP(target) <= 1);
                    break;

                case 1345: // The Clyteum
                           // The Eye of the Scorpion
                           // This finds the helper
                    _pauseActions = () =>
                    {
                        bool pause = false;
                        var MotionScannerHelper = GetClyteumMotionScanner(); // There can be two of these objects, only one appears to be active.

                        if (MotionScannerHelper is IGameObject scanner)
                        {
                            var facingdirection = MathHelper.GetCardinalDirection(MathHelper.RadToDeg(scanner.Rotation));

                            // Scans seem to be West<->East, but added North<->South just incase
                            float signedDistance = facingdirection switch
                            {
                                CardinalDirection.East => (Player.Position.X - scanner.Position.X),        // +X
                                CardinalDirection.West => (scanner.Position.X - Player.Position.X),        // -X
                                CardinalDirection.North => (Player.Position.Z - scanner.Position.Z),        // +Z
                                CardinalDirection.South => (scanner.Position.Z - Player.Position.Z),        // -Z
                                _ => (Player.Position.X - scanner.Position.X) // East, just to have a default
                            };

                            // Positive Distance = Incoming
                            // Negative Distance = Moving Away
                            // Distance will go from positive, to 0 when it fully overtakes the player,
                            // then negative as it moves away, with the status dropping off at around -8y,
                            // but added a buffer just in case.

                            pause = signedDistance switch
                            {
                                // Too far away
                                > 12f => false,
                                // 12y to -8y (about to be overtaken by the field to almost about to clear)
                                > -8f => true,
                                // -8y to -12y, waiting for status to clear, should happen close to -8y but added a buffer just in case
                                > -12f => HasStatusEffect(5191, anyOwner: true),
                                _ => false,
                            };
                        }
                        return pause;
                    };

                    _tankbusterAIDs = new uint[] {
                        // Malphas
                        50315 // Shadow Play Cast
                    }.ToFrozenSet();
                    _raidwideAIDs = new uint[] {
                        // Eye
                        48896, // Eyes On Me
                        48901, // Penetrator Missile (Helper)
                        // Chort
                        48884, // Ripples Of Gloom Cast
                        48886,  // Profane Pressure Cast
                        // Malphas
                        48920,
                        48929
                    }.ToFrozenSet();

                    break;

                case 1363: // Dancing Mad (Ultimate)
                           // Chaos = 19508
                           // Exdeath = 19509
                    _invincibleCheck = (target, targetID, _) =>
                    {
                        if (targetID is 19508 or 19509)
                        {
                            if (HasStatusEffect(4192)) return Result(targetID != 19508); // Epic Hero (α) — Chaos
                            if (HasStatusEffect(4194)) return Result(targetID != 19509); // Fated Hero (β) — Exdeath
                        }
                        return Invincible.False;
                    };
                    break;

                case 1368: // Windurst The Third Walk
                           // Alexander Battle
                           // 19805 Gordius System's Perfect Defense
                    _invincibleCheck = (_, targetID, targetStatuses) =>
                        Result(targetID is 19805 && targetStatuses.Contains(5377));

                    _pauseActions = () =>
                    {
                        // Medusa Swarmsinger
                        if (CheckForGazeCasts(0x4DAB, 50102)) return true; //Petrification cast

                        if (CheckForGazeCasts(0x4D92, 49121) || // Shinryu Paradox - Cataclysmic Vortex
                            CheckForGazeCasts(0x4D96, 49161))   // Hollow King - Cataclysmic Blade
                        {
                            return VfxManager.TrackedEffects
                                .FilterToTarget(Player.Object.GameObjectId)
                                .Any(x =>
                                    x.Path == "vfx/lockon/eff/z6r3_b4_lock_no_mv_7s_c0k2.avfx" || // Don't Move
                                    x.Path == "vfx/lockon/eff/z6r3_b4_lock_no_lk_7s_c0k2.avfx");  // Don't Look
                        }

                        return false;
                    };

                    _tankbusterAIDs = new uint[] {
                        50213, // Shantotto Vidohunir Cast
                        50088, // Feral Lunge Cast
                        50337, 50339,// Promathia Comet/Meteor Cast
                        49134, // Shinryu Dark Nova

                    }.ToFrozenSet();

                    _raidwideAIDs = new uint[] {
                        50215, 50102, // Shantotto Flare Play & Final Exam
                        50103, // Medusa Bellowing Grunt
                        50157, 50153, // Alexander Mega Holy, Divine Judgement
                        50485, // Aeaern Impact Stream
                        50317, 50694, // Promathia Empty Salvation, Deadly Rebirth
                        49182, 49179 // Shinryu & Hollow King Super Nova, Empty Proclamation
                    }.ToFrozenSet();

                    break;
                default:
                    dataLoaded = false;
                    break;
            }
            return dataLoaded;
        }

        private static unsafe IGameObject? GetClyteumMotionScanner()
        {
            return Svc.Objects.FirstOrDefault(x =>
                x.BaseId == 0x4C2D &&
                x.Address != 0 &&
                (int)(x.Struct()->RenderFlags) == 0
            );
        }
    }
}
