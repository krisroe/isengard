using Priority_Queue;
using QuickGraph;
using System;
using System.Collections.Generic;
namespace IsengardClient
{
    internal class IsengardMap
    {
        private AdjacencyGraph<Room, Exit> _map;
        private List<Area> _areas;
        private Dictionary<string, Area> _areasByName;
        private Dictionary<MapType, RoomGraph> _graphs;
        public Dictionary<HealingRoom, Room> HealingRooms = new Dictionary<HealingRoom, Room>();

        private string UNKNOWN_ROOM_NAME = "!@#UNKNOWN$%^";
        public Dictionary<string, Room> UnambiguousRooms = new Dictionary<string, Room>();
        public Dictionary<string, List<Room>> AmbiguousRooms = new Dictionary<string, List<Room>>();

        private RoomGraph _breeStreetsGraph;

        private Room _orderOfLove = null;
        private Area _aBreePerms;
        private Area _aImladrisTharbadPerms;
        private Area _aNindamosArmenelos;

        private const string AREA_BREE_PERMS = "Bree Perms";
        private const string AREA_IMLADRIS_THARBAD_PERMS = "Imladris/Tharbad Perms";
        private const string AREA_NINDAMOS_ARMENELOS = "Nindamos/Armenelos";

        //CSRTODO: what to do with preferred alignment (drunk/doctor at order of love)
        public IsengardMap(AlignmentType preferredAlignment)
        {
            _graphs = new Dictionary<MapType, RoomGraph>();
            _map = new AdjacencyGraph<Room, Exit>();
            _areas = new List<Area>();
            _areasByName = new Dictionary<string, Area>();

            _aBreePerms = AddArea(AREA_BREE_PERMS);
            _aImladrisTharbadPerms = AddArea(AREA_IMLADRIS_THARBAD_PERMS);
            _aNindamosArmenelos = AddArea(AREA_NINDAMOS_ARMENELOS);

            RoomGraph graphMillwoodMansion = new RoomGraph("Millwood Mansion");
            graphMillwoodMansion.ScalingFactor = 100;
            _graphs[MapType.MillwoodMansion] = graphMillwoodMansion;

            AddBreeCity(out Room oIxell, out Room oBreeTownSquare, out Room oBreeWestGateInside, out Room oSmoulderingVillage, graphMillwoodMansion, out Room oDroolie, out Room oSewerPipeExit, out Room breeEastGateInside, out Room boatswain, out Room breeEastGateOutside, out Room oCemetery);
            AddMayorMillwoodMansion(oIxell);
            AddBreeToHobbiton(oBreeWestGateInside, oSmoulderingVillage);
            AddBreeToImladris(out Room oOuthouse, breeEastGateInside, breeEastGateOutside, out Room imladrisWestGateOutside, oCemetery);
            AddUnderBree(oDroolie, oOuthouse, oSewerPipeExit);
            AddImladrisCity(out Room oImladrisSouthGateInside, out Room oEastGateOfImladrisOutside, imladrisWestGateOutside, out Room healingHand);
            AddEastOfImladris(oEastGateOfImladrisOutside, out Room westGateOfEsgaroth);
            AddImladrisToTharbad(oImladrisSouthGateInside, out Room oTharbadGateOutside);
            AddTharbadCity(oTharbadGateOutside, out Room tharbadWestGateOutside, out Room tharbadDocks, out RoomGraph tharbadGraph, out Room tharbadEastGate);
            AddWestOfTharbad(tharbadWestGateOutside);
            AddEastOfTharbad(tharbadEastGate);
            AddEsgaroth(westGateOfEsgaroth);
            AddNindamos(out Room oArmenelosGatesOutside, out Room oSouthernJunction, out Room oPathThroughTheValleyHiddenPath, out Room nindamosDocks, out RoomGraph nindamosGraph, out Room nindamosVillageCenter);
            AddArmenelos(oArmenelosGatesOutside);
            AddWestOfNindamosAndArmenelos(oSouthernJunction, oPathThroughTheValleyHiddenPath, out Room oEldemondeEastGateOutside);
            AddEldemondeCity(oEldemondeEastGateOutside);
            AddMithlond(boatswain, tharbadDocks, tharbadGraph, nindamosDocks, nindamosGraph);
            AddIntangible(oBreeTownSquare, healingHand, nindamosVillageCenter);

            foreach (KeyValuePair<MapType, RoomGraph> nextGraph in _graphs)
            {
                RoomGraph g = nextGraph.Value;
                var oldRooms = g.Rooms;
                g.Rooms = new Dictionary<Room, System.Windows.Point>();
                foreach (KeyValuePair<Room, System.Windows.Point> next in oldRooms)
                {
                    g.Rooms[next.Key] = new System.Windows.Point(next.Value.X * g.ScalingFactor, next.Value.Y * g.ScalingFactor);
                }
            }
        }

        public static IEnumerable<Exit> GetAllRoomExits(Room room)
        {
            foreach (Exit nextExit in room.Exits)
            {
                yield return nextExit;
            }
        }

        public static IEnumerable<Exit> GetRoomExits(Room room, Func<Exit, bool> exitDiscriminator)
        {
            foreach (Exit nextExit in room.Exits)
            {
                if (exitDiscriminator(nextExit))
                {
                    yield return nextExit;
                }
            }
        }

        public static List<string> GetObviousExits(Room r, out List<string> optionalExits)
        {
            List<string> ret = new List<string>();
            optionalExits = null;
            foreach (Exit nextExit in GetRoomExits(r, HiddenExitDiscriminator))
            {
                string exitText = nextExit.ExitText;
                if (nextExit.PresenceType == ExitPresenceType.Periodic || nextExit.WaitForMessage.HasValue)
                {
                    if (optionalExits == null) optionalExits = new List<string>();
                    if (!optionalExits.Contains(exitText)) optionalExits.Add(exitText);
                }
                else
                {
                    ret.Add(exitText);
                }
            }
            return ret;
        }

        private static bool HiddenExitDiscriminator(Exit e)
        {
            return !e.Hidden;
        }

        private void AddEastOfTharbad(Room tharbadEastGate)
        {
            RoomGraph tharbadEastGraph = new RoomGraph("East of Tharbad");
            tharbadEastGraph.ScalingFactor = 100;
            _graphs[MapType.AlliskPlainsEastOfTharbad] = tharbadEastGraph;

            tharbadEastGraph.Rooms[tharbadEastGate] = new System.Windows.Point(0, 4);

            Room oAlliskPlainsEntrance = AddRoom("Entrance", "Path around Allisk Plains");
            AddBidirectionalExits(tharbadEastGate, oAlliskPlainsEntrance, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEntrance] = new System.Windows.Point(1, 4);

            Room oAlliskPlainsEastPath1 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsEntrance, oAlliskPlainsEastPath1, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath1] = new System.Windows.Point(1.5, 4);
            //CSRTODO: narrow path

            Room oAlliskPlainsEastPath2 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastPath1, oAlliskPlainsEastPath2, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath2] = new System.Windows.Point(2, 4);

            Room oAlliskPlainsEastPath3 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastPath2, oAlliskPlainsEastPath3, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath3] = new System.Windows.Point(2.5, 4);

            Room oAlliskPlainsEastPath4Fork = AddRoom("Fork", "Fork in the Path");
            AddBidirectionalExits(oAlliskPlainsEastPath3, oAlliskPlainsEastPath4Fork, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath4Fork] = new System.Windows.Point(3, 4);

            Room oAlliskPlainsEastTrail1 = AddRoom("Trail", "Trail through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastPath4Fork, oAlliskPlainsEastTrail1, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastTrail1] = new System.Windows.Point(3.5, 4);

            Room oAlliskPlainsEastTrail2 = AddRoom("Trail", "Trail through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastTrail1, oAlliskPlainsEastTrail2, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oAlliskPlainsEastTrail2] = new System.Windows.Point(4, 4.5);

            Room oAlliskPlainsEastTrailDeadEnd = AddRoom("Dead End", "Dead End");
            AddBidirectionalExits(oAlliskPlainsEastTrail2, oAlliskPlainsEastTrailDeadEnd, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastTrailDeadEnd] = new System.Windows.Point(4.5, 4.5);

            Room oAlliskPlainsEastBend = AddRoom("Bend", "Bend in the Path");
            AddBidirectionalExits(oAlliskPlainsEastBend, oAlliskPlainsEastPath4Fork, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastBend] = new System.Windows.Point(6, 3);

            Room oAlliskPlainsNortheastPath1 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsNortheastPath1, oAlliskPlainsEastBend, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oAlliskPlainsNortheastPath1] = new System.Windows.Point(10, 2);

            Room oAlliskPlainsNortheastPath2 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsNortheastPath2, oAlliskPlainsNortheastPath1, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsNortheastPath2] = new System.Windows.Point(9, 2);

            Room oAlliskPlainsNortheastPath3 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsNortheastPath3, oAlliskPlainsNortheastPath2, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsNortheastPath3] = new System.Windows.Point(8, 2);

            Room oNorthPathAroundAlliskPlains1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains1, oAlliskPlainsEntrance, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains1] = new System.Windows.Point(2, 1);

            Room oNorthPathAroundAlliskPlains2 = AddRoom("Path", "Path through Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains1, oNorthPathAroundAlliskPlains2, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains2] = new System.Windows.Point(4, 1);

            Room oNorthPathAroundAlliskPlains3 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains2, oNorthPathAroundAlliskPlains3, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains3] = new System.Windows.Point(6, 1);

            Room oNorthPathAroundAlliskPlains4 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains3, oNorthPathAroundAlliskPlains4, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oNorthPathAroundAlliskPlains4, oAlliskPlainsNortheastPath3, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains4] = new System.Windows.Point(8, 1);

            Room oSoutheastPathAroundAlliskPlains1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oAlliskPlainsEntrance, oSoutheastPathAroundAlliskPlains1, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains1] = new System.Windows.Point(2, 5);

            Room oSoutheastPathAroundAlliskPlains2 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains1, oSoutheastPathAroundAlliskPlains2, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains2] = new System.Windows.Point(3, 6);

            Room oSoutheastPathAroundAlliskPlains3 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains2, oSoutheastPathAroundAlliskPlains3, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains3] = new System.Windows.Point(4, 6);

            Room oSoutheastPathAroundAlliskPlains4 = AddRoom("Allisk Plains", "Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains4, oSoutheastPathAroundAlliskPlains3, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oNorthPathAroundAlliskPlains4, oSoutheastPathAroundAlliskPlains4, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains4] = new System.Windows.Point(6, 3.5);
            //CSRTODO: sloping path

            Room oSouthPathAlliskPlains1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains2, oSouthPathAlliskPlains1, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains1] = new System.Windows.Point(4, 7);

            Room oSouthPathAlliskPlains2 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains1, oSouthPathAlliskPlains2, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains2] = new System.Windows.Point(4, 8);

            Room oSouthPathAlliskPlains3 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains2, oSouthPathAlliskPlains3, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains3] = new System.Windows.Point(4, 9);

            Room oSouthPathAlliskPlains4 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains3, oSouthPathAlliskPlains4, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains4] = new System.Windows.Point(3, 10);

            Room oSouthPathAlliskPlains5 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains4, oSouthPathAlliskPlains5, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains5] = new System.Windows.Point(4, 11);

            Room oSouthPathAlliskPlains6 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains5, oSouthPathAlliskPlains6, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains6] = new System.Windows.Point(5, 12);

            Room oSouthPathAlliskPlains7 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains7, oSouthPathAlliskPlains6, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains7] = new System.Windows.Point(5, 11);

            Room oSouthPathAlliskPlains8 = AddRoom("Allisk plains", "Allisk plains");
            AddExit(oSouthPathAlliskPlains7, oSouthPathAlliskPlains8, "northeast");
            Exit e = AddExit(oSoutheastPathAroundAlliskPlains4, oSouthPathAlliskPlains8, "southeast");
            e.Hidden = true;
            AddExit(oSouthPathAlliskPlains8, oSoutheastPathAroundAlliskPlains4, "northwest");
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains8] = new System.Windows.Point(7, 4.5);
            //CSRTODO: pit (hidden)

            Room oPathToOgres1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains6, oPathToOgres1, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oPathToOgres1] = new System.Windows.Point(5, 13);

            Room oTrakardOgreRanger = AddRoom("Ogre Rangers", "Path around Allisk Plains");
            oTrakardOgreRanger.AddPermanentMobs(MobTypeEnum.TrakardOgreRanger, MobTypeEnum.TrakardOgreRanger);
            AddBidirectionalExits(oPathToOgres1, oTrakardOgreRanger, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oTrakardOgreRanger] = new System.Windows.Point(4, 14);
        }

        private void AddWestOfTharbad(Room tharbadWestGateOutside)
        {
            RoomGraph tharbadWestGraph = new RoomGraph("West of Tharbad");
            tharbadWestGraph.ScalingFactor = 100;
            _graphs[MapType.WestOfTharbad] = tharbadWestGraph;

            tharbadWestGraph.Rooms[tharbadWestGateOutside] = new System.Windows.Point(6, 5);

            Room lelionBeachAndPark = AddRoom("Lelion Beach and Park", "Lelion Beach and Park");
            AddBidirectionalSameNameExit(tharbadWestGateOutside, lelionBeachAndPark, "ramp");
            tharbadWestGraph.Rooms[lelionBeachAndPark] = new System.Windows.Point(5, 5);

            Room beachPath = AddRoom("Beach Path", "Beach Path");
            AddBidirectionalExits(lelionBeachAndPark, beachPath, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[beachPath] = new System.Windows.Point(5, 6);

            Room marinersAnchor = AddRoom("Mariner's Anchor", "The Mariner's Anchor");
            AddBidirectionalExitsWithOut(beachPath, marinersAnchor, "west");
            tharbadWestGraph.Rooms[marinersAnchor] = new System.Windows.Point(4, 6);

            Room dalePurves = AddRoom("Dale Purves", "Dale's Beach");
            dalePurves.AddPermanentMobs(MobTypeEnum.DalePurves);
            AddExit(marinersAnchor, dalePurves, "back");
            AddExit(dalePurves, marinersAnchor, "east");
            tharbadWestGraph.Rooms[dalePurves] = new System.Windows.Point(3, 6);

            Room greyfloodRiver1 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver1.DamageType = RealmType.Water;
            AddExit(dalePurves, greyfloodRiver1, "river");
            AddExit(greyfloodRiver1, dalePurves, "beach");
            tharbadWestGraph.Rooms[greyfloodRiver1] = new System.Windows.Point(2, 6);

            Room greyfloodRiver2 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver2.DamageType = RealmType.Water;
            AddBidirectionalExits(greyfloodRiver1, greyfloodRiver2, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[greyfloodRiver2] = new System.Windows.Point(2, 7);

            Room greyfloodRiver3 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver3.DamageType = RealmType.Water;
            AddBidirectionalExits(greyfloodRiver2, greyfloodRiver3, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[greyfloodRiver3] = new System.Windows.Point(2, 8);

            Room riverMouth = AddRoom("River Mouth", "The Mouth of the Greyflood River");
            riverMouth.DamageType = RealmType.Water;
            AddExit(greyfloodRiver3, riverMouth, "southwest");
            AddExit(riverMouth, greyfloodRiver3, "river");
            tharbadWestGraph.Rooms[riverMouth] = new System.Windows.Point(1, 9);

            Room oWesternBeachPath1 = AddRoom("Western Beach Path", "Western Beach Path");
            AddBidirectionalExits(oWesternBeachPath1, riverMouth, BidirectionalExitType.WestEast);
            tharbadWestGraph.Rooms[oWesternBeachPath1] = new System.Windows.Point(0, 9);

            Room oWesternBeachPath2 = AddRoom("Western Beach Path", "Western Beach Path");
            AddBidirectionalExits(oWesternBeachPath2, oWesternBeachPath1, BidirectionalExitType.SouthwestNortheast);
            tharbadWestGraph.Rooms[oWesternBeachPath2] = new System.Windows.Point(1, 8);

            Room oBottomOfTheRocks = AddRoom("Bottom of the Rocks", "Bottom Of The Rocks");
            AddBidirectionalExits(oBottomOfTheRocks, oWesternBeachPath2, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oBottomOfTheRocks] = new System.Windows.Point(1, 7);

            Room oRockSlide = AddRoom("Rock Slide", "Rock Slide");
            AddBidirectionalExits(oRockSlide, oBottomOfTheRocks, BidirectionalExitType.UpDown);
            tharbadWestGraph.Rooms[oRockSlide] = new System.Windows.Point(1, 6);

            Room oDropOff = AddRoom("Drop Off", "Drop Off");
            AddBidirectionalExits(oDropOff, oRockSlide, BidirectionalExitType.UpDown);
            tharbadWestGraph.Rooms[oDropOff] = new System.Windows.Point(1, 5);

            Room oErynVornSouth = AddRoom("Eryn Vorn South", "South Edge of Eryn Vorn");
            AddBidirectionalExits(oErynVornSouth, oDropOff, BidirectionalExitType.SoutheastNorthwest);
            tharbadWestGraph.Rooms[oErynVornSouth] = new System.Windows.Point(0, 4);

            Room oLelionParkHillside = AddRoom("Lelion Park Hillside", "Lelion Park Hillside");
            AddBidirectionalExits(oLelionParkHillside, lelionBeachAndPark, BidirectionalExitType.SoutheastNorthwest);
            tharbadWestGraph.Rooms[oLelionParkHillside] = new System.Windows.Point(5, 4);

            Room oChildrensTidalPool = AddRoom("Children's Tidal Pool", "Children's Tidal Pool");
            oChildrensTidalPool.AddPermanentMobs(MobTypeEnum.SmallCrab, MobTypeEnum.UglyKid);
            AddBidirectionalExits(oChildrensTidalPool, oLelionParkHillside, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oChildrensTidalPool] = new System.Windows.Point(5, 3);

            Room oNorthShore = AddRoom("North Shore", "North Shore");
            AddBidirectionalExits(oNorthShore, oChildrensTidalPool, BidirectionalExitType.WestEast);
            tharbadWestGraph.Rooms[oNorthShore] = new System.Windows.Point(4, 3);

            Room oLelionPark = AddRoom("Lelion Park", "Lelion Park");
            AddBidirectionalExits(oLelionPark, lelionBeachAndPark, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oNorthShore, oLelionPark, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oLelionPark] = new System.Windows.Point(4, 5);

            Room oSouthCoveSandBar = AddRoom("South Cove Sand Bar", "South Cove Sand Bar");
            AddBidirectionalSameNameExit(oLelionPark, oSouthCoveSandBar, "drainage");
            tharbadWestGraph.Rooms[oSouthCoveSandBar] = new System.Windows.Point(3, 5);

            Room oMultiTurnPath = AddRoom("Multi-turn Path", "Multi-turn Path");
            AddBidirectionalExits(oMultiTurnPath, oSouthCoveSandBar, BidirectionalExitType.SoutheastNorthwest);
            tharbadWestGraph.Rooms[oMultiTurnPath] = new System.Windows.Point(2, 4);

            Room oCrookedPath = AddRoom("Crooked Path", "Crooked Path");
            AddExit(oMultiTurnPath, oCrookedPath, "west");
            AddExit(oMultiTurnPath, oCrookedPath, "north");
            AddExit(oCrookedPath, oMultiTurnPath, "west");
            tharbadWestGraph.Rooms[oCrookedPath] = new System.Windows.Point(1, 4);

            Room oNorthShoreGrotto = AddRoom("North Shore Grotto", "North Shore Grotto");
            Exit e = AddExit(oNorthShore, oNorthShoreGrotto, "west");
            e.IsTrapExit = true;
            AddExit(oNorthShoreGrotto, oCrookedPath, "southwest");
            tharbadWestGraph.Rooms[oNorthShoreGrotto] = new System.Windows.Point(3, 3);

            Room oNorthLookoutPoint = AddRoom("North Lookout Point", "North Lookout Point");
            AddExit(oNorthShoreGrotto, oNorthLookoutPoint, "west");
            AddExit(oNorthLookoutPoint, oCrookedPath, "south");
            tharbadWestGraph.Rooms[oNorthLookoutPoint] = new System.Windows.Point(0, 3);

            Room oNorthShoreShallowWaters = AddRoom("Shallow Waters", "North Shore Shallow Waters");
            AddBidirectionalExits(oNorthShoreShallowWaters, oNorthShore, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oNorthShoreShallowWaters] = new System.Windows.Point(4, 2);

            Room oNorthShoreWaters = AddRoom("Waters", "North Shore Waters");
            AddExit(oNorthShoreShallowWaters, oNorthShoreWaters, "tide");
            AddBidirectionalExits(oNorthShoreWaters, oNorthShoreGrotto, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oNorthShoreWaters] = new System.Windows.Point(4, 1);

            Room oOpenBay = AddRoom("Open Bay", "Open Bay");
            AddExit(oNorthShoreWaters, oOpenBay, "tide");
            tharbadWestGraph.Rooms[oOpenBay] = new System.Windows.Point(4, 0);

            Room oNorthLookoutTower = AddRoom("North Lookout Tower", "North Lookout Tower");
            AddExit(oOpenBay, oNorthLookoutTower, "south");
            AddBidirectionalExits(oNorthLookoutTower, oNorthLookoutPoint, BidirectionalExitType.NorthSouth);
            AddExit(oNorthShoreWaters, oNorthLookoutTower, "southwest");
            tharbadWestGraph.Rooms[oNorthLookoutTower] = new System.Windows.Point(0, 2);

            Room oNorthLookoutTowerCellar = AddRoom("North Lookout Tower Cellar", "North Lookout Tower Cellar");
            e = AddExit(oNorthLookoutTower, oNorthLookoutTowerCellar, "cellar");
            e.Hidden = true;
            AddExit(oNorthLookoutTowerCellar, oNorthLookoutTower, "door");
            tharbadWestGraph.Rooms[oNorthLookoutTowerCellar] = new System.Windows.Point(0, 1.5);

            Room oShroudedTunnel = AddRoom("Shrouded Tunnel", "Shrouded Tunnel");
            e = AddExit(oNorthLookoutTowerCellar, oShroudedTunnel, "shroud");
            e.Hidden = true;
            AddExit(oShroudedTunnel, oNorthLookoutTowerCellar, "out");
            tharbadWestGraph.Rooms[oShroudedTunnel] = new System.Windows.Point(0, 1);

            Room oShoreOfSeaOfTranquility1 = AddRoom("Sea Shore", "The Shore of the Sea of Tranquility");
            AddExit(riverMouth, oShoreOfSeaOfTranquility1, "shore");
            AddExit(oShoreOfSeaOfTranquility1, riverMouth, "north");
            tharbadWestGraph.Rooms[oShoreOfSeaOfTranquility1] = new System.Windows.Point(1, 10);

            Room oShoreOfSeaOfTranquility2 = AddRoom("Sea Shore", "The Shore of the Sea of Tranquility");
            AddBidirectionalExits(oShoreOfSeaOfTranquility1, oShoreOfSeaOfTranquility2, BidirectionalExitType.SouthwestNortheast);
            tharbadWestGraph.Rooms[oShoreOfSeaOfTranquility2] = new System.Windows.Point(0, 11);

            Room oShoreOfSeaOfTranquility3 = AddRoom("Sea Shore", "The Shore of the Sea of Tranquility");
            AddBidirectionalExits(oShoreOfSeaOfTranquility2, oShoreOfSeaOfTranquility3, BidirectionalExitType.SouthwestNortheast);
            tharbadWestGraph.Rooms[oShoreOfSeaOfTranquility3] = new System.Windows.Point(-1, 12);

            Room oEntranceToThunderCove = AddRoom("Thunder Cove Entrance", "Entrance to Thunder Cove");
            AddBidirectionalExits(oShoreOfSeaOfTranquility3, oEntranceToThunderCove, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oEntranceToThunderCove] = new System.Windows.Point(-1, 13);

            Room oDarkJungleEdge = AddRoom("Dark Jungle Edge", "Edge of a Dark Jungle");
            AddBidirectionalExits(oEntranceToThunderCove, oDarkJungleEdge, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oDarkJungleEdge] = new System.Windows.Point(-1, 14);

            Room oPrehistoricJungle = AddRoom("Prehistoric Jungle", "The Prehistoric Jungle");
            e = AddExit(oDarkJungleEdge, oPrehistoricJungle, "southwest");
            e.Hidden = true;
            AddExit(oPrehistoricJungle, oDarkJungleEdge, "northeast");
            tharbadWestGraph.Rooms[oPrehistoricJungle] = new System.Windows.Point(-2, 15);

            Room oWildmanVillage = AddRoom("Wildman Village", "Wildman Village");
            AddExit(oDarkJungleEdge, oWildmanVillage, "path");
            AddExit(oWildmanVillage, oDarkJungleEdge, "north");
            tharbadWestGraph.Rooms[oWildmanVillage] = new System.Windows.Point(-1, 15);
        }

        private void AddMithlond(Room boatswain, Room tharbadDocks, RoomGraph tharbadGraph, Room nindamosDocks, RoomGraph nindamosGraph)
        {
            RoomGraph mithlondGraph = new RoomGraph("Mithlond");
            _graphs[MapType.Mithlond] = mithlondGraph;
            mithlondGraph.ScalingFactor = 100;

            mithlondGraph.Rooms[boatswain] = new System.Windows.Point(1, 5);

            Room oCelduinExpressSlip = AddRoom("Celduin Express Slip", "Pier - Slip for the Celduin Express");
            oCelduinExpressSlip.AddPermanentMobs(MobTypeEnum.HarborMaster);
            oCelduinExpressSlip.BoatLocationType = BoatEmbarkOrDisembark.CelduinExpressMithlond;
            Exit e = AddExit(boatswain, oCelduinExpressSlip, "pier");
            e.PresenceType = ExitPresenceType.Periodic;
            e = AddExit(oCelduinExpressSlip, boatswain, "gangway");
            e.PresenceType = ExitPresenceType.Periodic;
            mithlondGraph.Rooms[oCelduinExpressSlip] = new System.Windows.Point(2, 5);

            Room oBullroarerSlip = AddRoom("Bullroarer Slip", "Pier - Slip for the Bullroarer");
            oBullroarerSlip.BoatLocationType = BoatEmbarkOrDisembark.BullroarerMithlond;
            AddBidirectionalExits(oCelduinExpressSlip, oBullroarerSlip, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oBullroarerSlip] = new System.Windows.Point(2, 6);

            Room oOmaniPrincessSlip = AddRoom("Omani Princess Slip", "Pier - Slip for the Omani Princess");
            AddBidirectionalExits(oBullroarerSlip, oOmaniPrincessSlip, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oOmaniPrincessSlip] = new System.Windows.Point(2, 7);

            Room oHarbringerSlip = AddRoom("Harbringer Slip", "Pier - Slip for the Harbringer");
            AddBidirectionalExits(oOmaniPrincessSlip, oHarbringerSlip, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oHarbringerSlip] = new System.Windows.Point(2, 8);

            Room oHarbringerGangplank = AddRoom("Gangplank", "Gangplank");
            oHarbringerGangplank.BoatLocationType = BoatEmbarkOrDisembark.HarbringerMithlond;
            AddExit(oHarbringerSlip, oHarbringerGangplank, "gangplank");
            AddExit(oHarbringerGangplank, oHarbringerSlip, "pier");
            mithlondGraph.Rooms[oHarbringerGangplank] = new System.Windows.Point(3, 8);

            Room oMithlondPort = AddRoom("Mithlond Port", "Mithlond Port");
            AddExit(oCelduinExpressSlip, oMithlondPort, "north");
            AddExit(oMithlondPort, oCelduinExpressSlip, "pier");
            mithlondGraph.Rooms[oMithlondPort] = new System.Windows.Point(2, 4);

            Room oEvendimTrailEnd = AddRoom("Evendim Trail", "End of the Evendim Trail");
            AddBidirectionalExits(oMithlondPort, oEvendimTrailEnd, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oEvendimTrailEnd] = new System.Windows.Point(3, 5);

            Room oMithlondPort2 = AddRoom("Mithlond Port", "Mithlond Port");
            AddBidirectionalExits(oMithlondPort2, oMithlondPort, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oMithlondPort2] = new System.Windows.Point(2, 3);

            Room oMusicianSchool = AddRoom("Musician School", "Mithlond Musician School");
            AddBidirectionalExits(oMithlondPort2, oMusicianSchool, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oMusicianSchool] = new System.Windows.Point(3, 3);

            Room oMithlondPort3 = AddRoom("Mithlond Port", "Mithlond Port");
            AddBidirectionalExits(oMithlondPort3, oMithlondPort2, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oMithlondPort3] = new System.Windows.Point(2, 2);

            Room oDarkAlley = AddRoom("Dark Alley", "Dark Alley");
            AddBidirectionalExits(oDarkAlley, oMithlondPort3, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oDarkAlley] = new System.Windows.Point(1, 2);

            Room oDeadEnd = AddRoom("Dead End", "Dead End");
            AddBidirectionalExits(oDeadEnd, oDarkAlley, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oDeadEnd] = new System.Windows.Point(1, 1.5);

            Room oSharkey = AddRoom("Sharkey", "Skarkey's Shippers");
            oSharkey.AddPermanentMobs(MobTypeEnum.Sharkey);
            e = AddExit(oDeadEnd, oSharkey, "west");
            e.Hidden = true;
            AddExit(oSharkey, oDeadEnd, "east");
            mithlondGraph.Rooms[oSharkey] = new System.Windows.Point(0, 1.5);

            Room oPicadilyAvenue = AddRoom("Picadily", "Picadily Avenue");
            AddBidirectionalExits(oMithlondPort3, oPicadilyAvenue, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oPicadilyAvenue] = new System.Windows.Point(3, 2);

            Room oHosuan = AddRoom("Ho-suan", "The Opium Den of Chon-Loc");
            oHosuan.AddPermanentMobs(MobTypeEnum.HoSuanThePenniless);
            AddBidirectionalExitsWithOut(oPicadilyAvenue, oHosuan, "north");
            mithlondGraph.Rooms[oHosuan] = new System.Windows.Point(3, 1);

            Room oMithlondGateInside = AddRoom("Gate Inside", "Mithlond Port");
            AddBidirectionalExits(oMithlondGateInside, oMithlondPort3, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oMithlondGateInside] = new System.Windows.Point(2, 1);

            Room oGrunkillCharters = AddRoom("Grunkill Charters", "Grunkill Charters");
            oGrunkillCharters.AddPermanentMobs(MobTypeEnum.Grunkill);
            AddBidirectionalExits(oGrunkillCharters, oMithlondGateInside, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oGrunkillCharters] = new System.Windows.Point(1, 1);

            Room oGrunkillQuarters = AddRoom("Grunkill Quarters", "Grunkill's Quarters");
            AddBidirectionalSameNameExit(oGrunkillCharters, oGrunkillQuarters, "curtain");
            mithlondGraph.Rooms[oGrunkillQuarters] = new System.Windows.Point(0, 1);

            Room oMithlondGateOutside = AddRoom("Gate Outside", "Ered Lune");
            AddBidirectionalSameNameExit(oMithlondGateInside, oMithlondGateOutside, "gate");
            mithlondGraph.Rooms[oMithlondGateOutside] = new System.Windows.Point(2, 0);

            AddHarbringer(mithlondGraph, oHarbringerGangplank, tharbadDocks, tharbadGraph);
            AddBullroarer(mithlondGraph, oBullroarerSlip, nindamosDocks, nindamosGraph);
            AddCelduinExpress(mithlondGraph, boatswain);
        }

        private void AddCelduinExpress(RoomGraph mithlondGraph, Room boatswain)
        {
            Room oCelduinExpressNW = AddRoom("Stern", "Stern of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressNW, boatswain, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oCelduinExpressNW] = new System.Windows.Point(0, 5);

            Room oCelduinExpressMainDeckW = AddRoom("Main Deck", "Main Deck of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressNW, oCelduinExpressMainDeckW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(boatswain, oCelduinExpressMainDeckW, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oCelduinExpressMainDeckW] = new System.Windows.Point(0, 5.5);

            Room oCelduinExpressMainDeckE = AddRoom("Main Deck", "Main Deck of the Celduin Express");
            AddBidirectionalExits(boatswain, oCelduinExpressMainDeckE, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCelduinExpressNW, oCelduinExpressMainDeckE, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oCelduinExpressMainDeckW, oCelduinExpressMainDeckE, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oCelduinExpressMainDeckE] = new System.Windows.Point(1, 5.5);

            Room oCelduinExpressBow = AddRoom("Bow", "Bow of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressMainDeckE, oCelduinExpressBow, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oCelduinExpressMainDeckW, oCelduinExpressBow, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oCelduinExpressBow] = new System.Windows.Point(0.5, 6);
        }

        /// <summary>
        /// harbringer allows travel from Tharbad to Mithlond (but not the reverse?)
        /// </summary>
        private void AddHarbringer(RoomGraph mithlondGraph, Room mithlondEntrance, Room tharbadDocks, RoomGraph tharbadGraph)
        {
            Room oHarbringerTop = AddRoom("Bluejacket", "Bow of the Harbringer.");
            oHarbringerTop.AddPermanentMobs(MobTypeEnum.Bluejacket, MobTypeEnum.Scallywag);
            mithlondGraph.Rooms[oHarbringerTop] = new System.Windows.Point(4.5, 5.5);

            Room oHarbringerWest1 = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            AddBidirectionalExits(oHarbringerTop, oHarbringerWest1, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oHarbringerWest1] = new System.Windows.Point(4, 6);

            Room oHarbringerEast1 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerTop, oHarbringerEast1, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oHarbringerWest1, oHarbringerEast1, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oHarbringerEast1] = new System.Windows.Point(5, 6);

            Room oHarbringerMithlondEntrance = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            oHarbringerMithlondEntrance.BoatLocationType = BoatEmbarkOrDisembark.Harbringer;
            AddBidirectionalExits(oHarbringerWest1, oHarbringerMithlondEntrance, BidirectionalExitType.NorthSouth);
            Exit e = AddExit(mithlondEntrance, oHarbringerMithlondEntrance, "ship");
            e.PresenceType = ExitPresenceType.Periodic;
            e = AddExit(oHarbringerMithlondEntrance, mithlondEntrance, "gangplank");
            e.PresenceType = ExitPresenceType.Periodic;
            e = AddExit(tharbadDocks, oHarbringerMithlondEntrance, "gangway");
            e.PresenceType = ExitPresenceType.Periodic;
            mithlondGraph.Rooms[oHarbringerMithlondEntrance] = new System.Windows.Point(4, 6.5);
            tharbadGraph.Rooms[oHarbringerMithlondEntrance] = new System.Windows.Point(0, 9);

            Room oHarbringerEast2 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerMithlondEntrance, oHarbringerEast2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oHarbringerEast1, oHarbringerEast2, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oHarbringerEast2] = new System.Windows.Point(5, 6.5);

            Room oHarbringerWest3 = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            AddBidirectionalExits(oHarbringerMithlondEntrance, oHarbringerWest3, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oHarbringerWest3] = new System.Windows.Point(4, 7);

            Room oHarbringerEast3 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerEast2, oHarbringerEast3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oHarbringerWest3, oHarbringerEast3, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oHarbringerEast3] = new System.Windows.Point(5, 7);

            Room oHarbringerWest4 = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            AddBidirectionalExits(oHarbringerWest3, oHarbringerWest4, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oHarbringerWest4] = new System.Windows.Point(4, 7.5);

            Room oHarbringerEast4 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerEast3, oHarbringerEast4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oHarbringerWest4, oHarbringerEast4, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oHarbringerEast4] = new System.Windows.Point(5, 7.5);

            Room oKralle = AddRoom("Kralle", "Stern of the Harbringer");
            oKralle.AddPermanentMobs(MobTypeEnum.Kralle);
            AddBidirectionalExits(oHarbringerWest4, oKralle, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oHarbringerEast4, oKralle, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oKralle] = new System.Windows.Point(4.5, 8);
        }

        private void AddBullroarer(RoomGraph mithlondGraph, Room mithlondEntrance, Room nindamosDocks, RoomGraph nindamosGraph)
        {
            Room bullroarerSE = AddRoom("Bullroarer", "Deck of the Bullroarer");
            bullroarerSE.BoatLocationType = BoatEmbarkOrDisembark.Bullroarer;
            Exit e = AddExit(mithlondEntrance, bullroarerSE, "gangway");
            e.PresenceType = ExitPresenceType.Periodic;
            e = AddExit(nindamosDocks, bullroarerSE, "gangway");
            e.PresenceType = ExitPresenceType.Periodic;
            e = AddExit(bullroarerSE, mithlondEntrance, "plank");
            e.WaitForMessage = InformationalMessageType.BullroarerInMithlond;
            e = AddExit(bullroarerSE, nindamosDocks, "plank");
            e.WaitForMessage = InformationalMessageType.BullroarerInNindamos;
            nindamosGraph.Rooms[bullroarerSE] = new System.Windows.Point(15, 6);
            mithlondGraph.Rooms[bullroarerSE] = new System.Windows.Point(5, 5);

            Room bullroarerSW = AddRoom("Bullroarer", "Covered Deck");
            AddBidirectionalExits(bullroarerSW, bullroarerSE, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[bullroarerSW] = new System.Windows.Point(4, 5);

            Room bullroarerNW = AddRoom("Bullroarer", "Fish Landing Deck");
            AddBidirectionalExits(bullroarerNW, bullroarerSW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(bullroarerNW, bullroarerSE, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[bullroarerNW] = new System.Windows.Point(4, 4.5);

            Room bullroarerNE = AddRoom("Bullroarer", "Fish Cleaning Deck");
            AddBidirectionalExits(bullroarerNW, bullroarerNE, BidirectionalExitType.WestEast);
            AddBidirectionalExits(bullroarerNE, bullroarerSE, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(bullroarerNE, bullroarerSW, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[bullroarerNE] = new System.Windows.Point(5, 4.5);

            Room wheelhouse = AddRoom("Wheelhouse", "The Bullroarers Wheelhouse");
            AddExit(bullroarerNE, wheelhouse, "wheelhouse");
            AddExit(bullroarerNW, wheelhouse, "wheelhouse");
            AddExit(wheelhouse, bullroarerNE, "out");
            mithlondGraph.Rooms[wheelhouse] = new System.Windows.Point(4.5, 4);

            Room cargoHold = AddRoom("Cargo Hold", "Cargo Hold");
            AddBidirectionalSameNameExit(wheelhouse, cargoHold, "stairs");
            mithlondGraph.Rooms[cargoHold] = new System.Windows.Point(4.5, 3.5);

            Room fishHold = AddRoom("Fish Hold", "Fish Hold");
            fishHold.DamageType = RealmType.Wind;
            Room brentDiehard = AddRoom("Brent Diehard", "Engine Room");
            brentDiehard.AddPermanentMobs(MobTypeEnum.BrentDiehard);
            mithlondGraph.Rooms[fishHold] = new System.Windows.Point(4.5, 3);
            mithlondGraph.Rooms[brentDiehard] = new System.Windows.Point(4.5, 2.5);
            AddBidirectionalSameNameExit(fishHold, brentDiehard, "hatchway", true);
            AddBidirectionalSameNameExit(cargoHold, fishHold, "hatch", true);
        }

        public AdjacencyGraph<Room, Exit> MapGraph
        {
            get
            {
                return _map;
            }
        }

        public List<Area> Areas
        {
            get
            {
                return _areas;
            }
        }

        public Dictionary<MapType, RoomGraph> Graphs
        {
            get
            {
                return _graphs;
            }
        }

        private void AddTharbadCity(Room oTharbadGateOutside, out Room tharbadWestGateOutside, out Room tharbadDocks, out RoomGraph tharbadGraph, out Room tharbadEastGate)
        {
            tharbadGraph = new RoomGraph("Tharbad");
            tharbadGraph.ScalingFactor = 100;
            _graphs[MapType.Tharbad] = tharbadGraph;

            tharbadGraph.Rooms[oTharbadGateOutside] = new System.Windows.Point(3, 0);

            Room balleNightingale = AddRoom("Balle/Nightingale", "Nightingale Ave./Balle St.");
            Room balle1 = AddRoom("Balle", "Balle Street");
            AddBidirectionalExits(balleNightingale, balle1, BidirectionalExitType.WestEast);
            AddBidirectionalSameNameExit(oTharbadGateOutside, balleNightingale, "gate");
            tharbadGraph.Rooms[balleNightingale] = new System.Windows.Point(3, 1);
            tharbadGraph.Rooms[balle1] = new System.Windows.Point(4, 1);

            Room balleIllusion = AddRoom("Balle/Illusion", "Balle Street/Illusion Road");
            AddBidirectionalExits(balle1, balleIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balleIllusion] = new System.Windows.Point(5, 1);

            Room balle2 = AddRoom("Balle", "Balle Street");
            AddBidirectionalExits(balleIllusion, balle2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balle2] = new System.Windows.Point(8, 1);

            Room balleEvard = AddRoom("Balle/Evard", "Balle St./Evard Ave.");
            AddBidirectionalExits(balle2, balleEvard, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balleEvard] = new System.Windows.Point(10, 1);

            Room evardMemorialTree = AddRoom("Evard Memorial Tree", "In the arms of the Evard Memorial Tree");
            Exit e = AddExit(balleEvard, evardMemorialTree, "tree");
            e.Hidden = true;
            AddExit(evardMemorialTree, balleEvard, "down");
            tharbadGraph.Rooms[evardMemorialTree] = new System.Windows.Point(10, 0);

            Room evard1 = AddRoom("Evard", "Evard Avenue");
            AddBidirectionalExits(balleEvard, evard1, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[evard1] = new System.Windows.Point(10, 3);

            Room evard2 = AddRoom("Evard", "Evard Avenue");
            AddBidirectionalExits(evard1, evard2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[evard2] = new System.Windows.Point(10, 7);

            Room alley = AddRoom("Alley", "Alley");
            AddBidirectionalExits(alley, evard2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[alley] = new System.Windows.Point(8, 7);

            Room sabreEvard = AddRoom("Sabre/Evard", "Sabre Street/Evard Avenue");
            AddBidirectionalExits(evard2, sabreEvard, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[sabreEvard] = new System.Windows.Point(10, 8);

            Room sabre1 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre1, sabreEvard, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre1] = new System.Windows.Point(8, 8);

            Room sabreIllusion = AddRoom("Sabre/Illusion", "Sabre Street/Illusion Road");
            AddBidirectionalExits(sabreIllusion, sabre1, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabreIllusion] = new System.Windows.Point(5, 8);

            Room sabre2 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre2, sabreIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre2] = new System.Windows.Point(4, 8);

            Room sabreNightingale = AddRoom("Sabre/Nightingale", "Nightingale Ave./Sabre Street");
            AddBidirectionalExits(sabreNightingale, sabre2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabreNightingale] = new System.Windows.Point(3, 8);

            Room nightingale1 = AddRoom("Nightingale", "Nightingale Avenue");
            AddBidirectionalExits(nightingale1, sabreNightingale, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale1] = new System.Windows.Point(3, 5);

            Room efrimsJuiceCafe = AddRoom("Efrim's Juice Cafe", "Efrim's Juice Cafe");
            AddBidirectionalSameNameExit(nightingale1, efrimsJuiceCafe, "door");
            tharbadGraph.Rooms[efrimsJuiceCafe] = new System.Windows.Point(4, 5);

            Room nightingale2 = AddRoom("Nightingale", "Nightingale Avenue");
            AddBidirectionalExits(nightingale2, nightingale1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(balleNightingale, nightingale2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale2] = new System.Windows.Point(3, 3);

            Room jewelryShop = AddRoom("Jewelry Shop", "Jewelry Shop");
            AddBidirectionalExits(nightingale2, jewelryShop, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[jewelryShop] = new System.Windows.Point(4, 3);

            Room nightingale3 = AddRoom("Nightingale", "Nightingale Avenue");
            AddBidirectionalExits(sabreNightingale, nightingale3, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale3] = new System.Windows.Point(3, 9);

            Room illusion2 = AddRoom("Illusion", "Illusion Road");
            AddBidirectionalExits(balleIllusion, illusion2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[illusion2] = new System.Windows.Point(5, 3);

            Room marketBeast = AddRoom("Market Beast", "Market District - Beast Sellers");
            AddBidirectionalExits(illusion2, marketBeast, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketBeast] = new System.Windows.Point(5, 5);

            Room bardicGuildhall = AddHealingRoom("Bardic Guildhall", "Bardic Guildhall", HealingRoom.Tharbad);
            AddBidirectionalExits(bardicGuildhall, nightingale3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[bardicGuildhall] = new System.Windows.Point(2, 9);

            Room oGuildmasterAnsette = AddRoom("Ansette", "Guildmaster's Office");
            oGuildmasterAnsette.AddPermanentMobs(MobTypeEnum.GuildmasterAnsette, MobTypeEnum.PrucillaTheGroupie);
            e = AddExit(bardicGuildhall, oGuildmasterAnsette, "door");
            e.Hidden = true;
            e.RequiresDay = true;
            AddExit(oGuildmasterAnsette, bardicGuildhall, "out");
            tharbadGraph.Rooms[oGuildmasterAnsette] = new System.Windows.Point(2, 8.5);

            Room sabre3 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre3, sabreNightingale, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre3] = new System.Windows.Point(2, 8);

            Room sabre4 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre4, sabre3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre4] = new System.Windows.Point(1, 8);

            Room tourismAndCustoms = AddRoom("Tourism and Customs", "Tourism and Customs");
            AddBidirectionalExits(tourismAndCustoms, sabre4, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[tourismAndCustoms] = new System.Windows.Point(1, 7);

            Room tharbadWestGateInside = AddRoom("West Gate Inside", "Sabre Street");
            AddBidirectionalExits(tharbadWestGateInside, sabre4, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[tharbadWestGateInside] = new System.Windows.Point(0, 8);

            tharbadWestGateOutside = AddRoom("West Gate Outside", "West Gate of Tharbad");
            AddBidirectionalSameNameExit(tharbadWestGateInside, tharbadWestGateOutside, "gate");
            tharbadGraph.Rooms[tharbadWestGateOutside] = new System.Windows.Point(-1, 8);

            tharbadDocks = AddRoom("Docks", "Tharbad Docks");
            tharbadDocks.BoatLocationType = BoatEmbarkOrDisembark.HarbringerTharbad;
            AddBidirectionalExits(tharbadWestGateOutside, tharbadDocks, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[tharbadDocks] = new System.Windows.Point(-1, 9);

            Room illusion1 = AddRoom("Illusion", "Illusion Road");
            AddBidirectionalExits(illusion1, sabreIllusion, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[illusion1] = new System.Windows.Point(5, 7);

            Room marketDistrictClothiers = AddRoom("Market Clothiers", "Market District - Clothiers");
            AddBidirectionalExits(marketDistrictClothiers, illusion1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(marketBeast, marketDistrictClothiers, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketDistrictClothiers] = new System.Windows.Point(5, 6);

            Room oMasterJeweler = AddRoom("Jeweler", "Market District - Precious Gems");
            oMasterJeweler.AddPermanentMobs(MobTypeEnum.MasterJeweler, MobTypeEnum.ElvenGuard);
            AddBidirectionalExits(marketDistrictClothiers, oMasterJeweler, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oMasterJeweler] = new System.Windows.Point(6, 6);

            Room marketTrinkets = AddRoom("Market Trinkets", "Market District - Trinkets and Baubles");
            AddBidirectionalExits(marketBeast, marketTrinkets, BidirectionalExitType.WestEast);
            AddBidirectionalExits(marketTrinkets, oMasterJeweler, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketTrinkets] = new System.Windows.Point(6, 5);

            Room oEntranceToGypsyEncampment = AddRoom("Gypsy Encampment", "Entrance to Gypsy Encampment");
            AddExit(oMasterJeweler, oEntranceToGypsyEncampment, "row");
            AddExit(oEntranceToGypsyEncampment, oMasterJeweler, "market");
            tharbadGraph.Rooms[oEntranceToGypsyEncampment] = new System.Windows.Point(7, 6);

            Room oGypsyRow1 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oEntranceToGypsyEncampment, oGypsyRow1, BidirectionalExitType.WestEast);
            e = AddExit(alley, oGypsyRow1, "north");
            e.Hidden = true;
            e = AddExit(oGypsyRow1, alley, "south");
            e.Hidden = true;
            tharbadGraph.Rooms[oGypsyRow1] = new System.Windows.Point(8, 6);

            Room oGypsyRow2 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oGypsyRow1, oGypsyRow2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oGypsyRow2] = new System.Windows.Point(9, 6);

            Room oGypsyRow3 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oGypsyRow3, oGypsyRow2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oGypsyRow3] = new System.Windows.Point(9, 5);

            Room oGypsyRow4 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oGypsyRow4, oGypsyRow3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oGypsyRow4] = new System.Windows.Point(8, 5);

            Room oKingBrundensWagon = AddRoom("King Wagon", "King's Wagon");
            AddBidirectionalExitsWithOut(oGypsyRow4, oKingBrundensWagon, "wagon");
            tharbadGraph.Rooms[oKingBrundensWagon] = new System.Windows.Point(8, 4);

            Room oKingBrunden = AddRoom("King Brunden", "Gypsy King's Lounge");
            oKingBrunden.AddPermanentMobs(MobTypeEnum.KingBrunden);
            AddBidirectionalExitsWithOut(oKingBrundensWagon, oKingBrunden, "back");
            tharbadGraph.Rooms[oKingBrunden] = new System.Windows.Point(8, 3);

            Room oGypsyBlademaster = AddRoom("Blademaster", "Fighters' Tent");
            oGypsyBlademaster.AddPermanentMobs(MobTypeEnum.GypsyBlademaster);
            AddBidirectionalExitsWithOut(oGypsyRow3, oGypsyBlademaster, "tent");
            tharbadGraph.Rooms[oGypsyBlademaster] = new System.Windows.Point(9, 4);

            Room oKingsMoneychanger = AddRoom("Moneychanger", "Gypsy Moneychanger");
            oKingsMoneychanger.AddPermanentMobs(MobTypeEnum.KingsMoneychanger);
            AddBidirectionalExitsWithOut(oGypsyRow2, oKingsMoneychanger, "tent");
            tharbadGraph.Rooms[oKingsMoneychanger] = new System.Windows.Point(9, 6.5);

            Room oMadameNicolov = AddRoom("Nicolov", "Madame Nicolov's Wagon");
            oMadameNicolov.AddPermanentMobs(MobTypeEnum.MadameNicolov);
            AddBidirectionalExitsWithOut(oGypsyRow1, oMadameNicolov, "wagon");
            tharbadGraph.Rooms[oMadameNicolov] = new System.Windows.Point(8, 5.5);

            Room gildedApple = AddRoom("Gilded Apple", "The Gilded Apple");
            AddBidirectionalSameNameExit(sabre3, gildedApple, "door");
            tharbadGraph.Rooms[gildedApple] = new System.Windows.Point(2, 7.5);

            Room zathriel = AddRoom("Zathriel the Minstrel", "Gilded Apple - Stage");
            zathriel.AddPermanentMobs(MobTypeEnum.ZathrielTheMinstrel);
            e = AddExit(gildedApple, zathriel, "stage");
            e.Hidden = true;
            AddExit(zathriel, gildedApple, "down");
            tharbadGraph.Rooms[zathriel] = new System.Windows.Point(2, 7);

            Room oOliphauntsTattoos = AddRoom("Oliphaunt's Tattoos", "Oliphaunt's Tattoos");
            AddBidirectionalExits(balle2, oOliphauntsTattoos, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oOliphauntsTattoos] = new System.Windows.Point(8, 1.5);

            Room oOliphaunt = AddRoom("Oliphaunt", "Oliphaunt's Workroom");
            oOliphaunt.AddPermanentMobs(MobTypeEnum.OliphauntTheTattooArtist);
            AddBidirectionalSameNameExit(oOliphauntsTattoos, oOliphaunt, "curtain");
            tharbadGraph.Rooms[oOliphaunt] = new System.Windows.Point(8, 2);

            Room oCrimsonBridge = AddRoom("Crimson Bridge", "Crimson Bridge");
            AddBidirectionalExits(sabre2, oCrimsonBridge, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oCrimsonBridge] = new System.Windows.Point(4, 8.25);

            Room oCrimsonTowerBase = AddRoom("Tower Base", "Tower Base");
            AddBidirectionalExits(oCrimsonBridge, oCrimsonTowerBase, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oCrimsonTowerBase] = new System.Windows.Point(4, 8.5);

            Room oClingingToIvy1 = AddRoom("Clinging to Ivy", "Clinging to Ivy");
            e = AddExit(oCrimsonTowerBase, oClingingToIvy1, "ivy");
            e.Hidden = true;
            AddExit(oClingingToIvy1, oCrimsonTowerBase, "down");
            tharbadGraph.Rooms[oClingingToIvy1] = new System.Windows.Point(4, 8.75);

            Room oClingingToIvy2 = AddRoom("Clinging to Ivy", "Clinging to Ivy");
            AddBidirectionalExits(oClingingToIvy2, oClingingToIvy1, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oClingingToIvy2] = new System.Windows.Point(4, 9);

            Room oTopOfTheTower = AddRoom("Tower Top", "Top of the Tower");
            AddBidirectionalExits(oTopOfTheTower, oClingingToIvy2, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oTopOfTheTower] = new System.Windows.Point(4, 9.25);

            Room oPalaceGates = AddRoom("Palace Gates", "Gates of the Palace of Illusion");
            AddBidirectionalExits(sabreIllusion, oPalaceGates, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oPalaceGates] = new System.Windows.Point(5, 9);

            Room oPalaceOfIllusion = AddRoom("Illusion Palace", "Palace of Illusion");
            AddBidirectionalSameNameExit(oPalaceGates, oPalaceOfIllusion, "gate");
            tharbadGraph.Rooms[oPalaceOfIllusion] = new System.Windows.Point(4.5, 12);

            Room oImperialKitchens = AddRoom("Imperial Kitchens", "Imperial Kitchens");
            AddBidirectionalExits(oImperialKitchens, oPalaceOfIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oImperialKitchens] = new System.Windows.Point(3.5, 12);

            Room oHallOfRainbows1 = AddRoom("Rainbow Hall", "Hall of Rainbows");
            AddBidirectionalExits(oHallOfRainbows1, oPalaceOfIllusion, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oHallOfRainbows1] = new System.Windows.Point(4.5, 11);

            Room oHallOfRainbows2 = AddRoom("Rainbow Hall", "Hall of Rainbows");
            AddBidirectionalExits(oHallOfRainbows1, oHallOfRainbows2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oHallOfRainbows2] = new System.Windows.Point(5.5, 11);

            Room oHallOfRainbows3 = AddRoom("Rainbow Hall", "Hall of Rainbows");
            AddBidirectionalExits(oHallOfRainbows2, oHallOfRainbows3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oHallOfRainbows3] = new System.Windows.Point(6.5, 11);

            Room oEmptyGuestRoom = AddRoom("Guest Room", "Empty Guest Room");
            AddBidirectionalSameNameExit(oHallOfRainbows3, oEmptyGuestRoom, "door");
            tharbadGraph.Rooms[oEmptyGuestRoom] = new System.Windows.Point(6.5, 12);

            Room oChancellorsDesk = AddRoom("Chancellor's Desk", "Chancellor's Desk");
            AddExit(oHallOfRainbows1, oChancellorsDesk, "arch");
            AddExit(oChancellorsDesk, oHallOfRainbows1, "east");
            tharbadGraph.Rooms[oChancellorsDesk] = new System.Windows.Point(3.5, 11);

            Room oMainAudienceChamber = AddRoom("Audience Chamber", "Main Audience Chamber");
            AddBidirectionalExits(oMainAudienceChamber, oChancellorsDesk, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oMainAudienceChamber] = new System.Windows.Point(3.5, 10);

            Room oCaptainRenton = AddRoom("Throne Room", "Throne Room");
            oCaptainRenton.AddPermanentMobs(MobTypeEnum.CaptainRenton);
            AddBidirectionalExits(oCaptainRenton, oMainAudienceChamber, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oCaptainRenton] = new System.Windows.Point(2.5, 10);

            Room oAdvisorsSecretChamber = AddRoom("Advisor's Chamber", "Advisor's Secret Chamber");
            e = AddExit(oCaptainRenton, oAdvisorsSecretChamber, "tapestry");
            e.Hidden = true;
            AddExit(oAdvisorsSecretChamber, oCaptainRenton, "tapestry");
            tharbadGraph.Rooms[oAdvisorsSecretChamber] = new System.Windows.Point(2.5, 11);

            Room oStepsToAzureTower = AddRoom("Azure Steps", "Steps to Azure Tower");
            e = AddExit(oAdvisorsSecretChamber, oStepsToAzureTower, "passage");
            e.Hidden = true;
            AddExit(oStepsToAzureTower, oHallOfRainbows2, "corridor");
            tharbadGraph.Rooms[oStepsToAzureTower] = new System.Windows.Point(1.5, 12);

            Room oAzureTowerStaircase1 = AddRoom("Azure Staircase", "Azure Tower Staircase");
            AddBidirectionalExits(oAzureTowerStaircase1, oStepsToAzureTower, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oAzureTowerStaircase1] = new System.Windows.Point(1.5, 11);

            Room oAzureTowerStaircase2 = AddRoom("Azure Tower Staircase", "Azure Tower Staircase");
            AddBidirectionalExits(oAzureTowerStaircase2, oAzureTowerStaircase1, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oAzureTowerStaircase2] = new System.Windows.Point(1.5, 10);

            Room oArenaPath = AddRoom("Arena Path", "Arena Path");
            AddBidirectionalExits(sabreEvard, oArenaPath, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oArenaPath] = new System.Windows.Point(9, 8.5);

            Room oArenaTunnel1 = AddRoom("Arena Tunnel", "Arena Tunnel");
            e = AddExit(oArenaPath, oArenaTunnel1, "arch");
            e.Hidden = true;
            AddExit(oArenaTunnel1, oArenaPath, "arch");
            tharbadGraph.Rooms[oArenaTunnel1] = new System.Windows.Point(9, 9);

            Room oArenaTunnel2 = AddRoom("Arena Tunnel", "Arena Tunnel");
            AddBidirectionalSameNameExit(oArenaTunnel1, oArenaTunnel2, "slope");
            tharbadGraph.Rooms[oArenaTunnel2] = new System.Windows.Point(9, 9.5);

            Room oTunnel1 = AddRoom("Tunnel 1", "Tunnel One");
            AddBidirectionalExits(oTunnel1, oArenaTunnel2, BidirectionalExitType.SoutheastNorthwest);
            tharbadGraph.Rooms[oTunnel1] = new System.Windows.Point(8, 9);

            Room oCenterRing = AddRoom("Center Ring", "Center Ring");
            AddBidirectionalExits(oCenterRing, oTunnel1, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oCenterRing] = new System.Windows.Point(7, 9);

            Room oTunnel2 = AddRoom("Tunnel 2", "Tunnel Two");
            AddBidirectionalExits(oArenaTunnel2, oTunnel2, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oTunnel2] = new System.Windows.Point(8, 10);

            Room oMiddleRing1 = AddRoom("Middle Ring", "Middle Ring");
            AddBidirectionalExits(oMiddleRing1, oTunnel2, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oMiddleRing1] = new System.Windows.Point(7, 10);

            Room oMiddleRing2 = AddRoom("Middle Ring", "Middle Ring");
            AddBidirectionalSameNameExit(oMiddleRing1, oMiddleRing2, "ring");
            tharbadGraph.Rooms[oMiddleRing2] = new System.Windows.Point(6, 10);

            Room oTunnel3 = AddRoom("Tunnel 3", "Tunnel Three");
            AddBidirectionalExits(oArenaTunnel2, oTunnel3, BidirectionalExitType.SoutheastNorthwest);
            tharbadGraph.Rooms[oTunnel3] = new System.Windows.Point(10, 10);

            Room oOuterRingEast = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingEast, oTunnel3, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oOuterRingEast] = new System.Windows.Point(10, 11);

            Room oOuterRingNorth = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingNorth, oOuterRingEast, BidirectionalExitType.SoutheastNorthwest);
            tharbadGraph.Rooms[oOuterRingNorth] = new System.Windows.Point(9, 10);

            Room oOuterRingWest = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingNorth, oOuterRingWest, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oOuterRingWest] = new System.Windows.Point(8, 11);

            Room oOuterRingSouth = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingWest, oOuterRingSouth, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oOuterRingEast, oOuterRingSouth, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oOuterRingSouth] = new System.Windows.Point(9, 12);

            tharbadEastGate = AddRoom("East Gate Outside", "Eastern Gate of Tharbad");
            AddBidirectionalSameNameExit(sabreEvard, tharbadEastGate, "gate");
            tharbadGraph.Rooms[tharbadEastGate] = new System.Windows.Point(11, 8);

            AddLocation(_aImladrisTharbadPerms, oGuildmasterAnsette);
            AddLocation(_aImladrisTharbadPerms, zathriel);
            AddLocation(_aImladrisTharbadPerms, oOliphaunt);
            AddLocation(_aImladrisTharbadPerms, oMasterJeweler);
            AddLocation(_aImladrisTharbadPerms, oMadameNicolov);
            AddLocation(_aImladrisTharbadPerms, oKingsMoneychanger);
            AddLocation(_aImladrisTharbadPerms, oGypsyBlademaster);
            AddLocation(_aImladrisTharbadPerms, oKingBrunden);
        }

        private void AddBreeCity(out Room oIxell, out Room oBreeTownSquare, out Room oWestGateInside, out Room oSmoulderingVillage, RoomGraph graphMillwoodMansion, out Room oDroolie, out Room oSewerPipeExit, out Room breeEastGateInside, out Room boatswain, out Room breeEastGateOutside, out Room oCemetery)
        {
            _breeStreetsGraph = new RoomGraph("Bree Streets");
            _breeStreetsGraph.ScalingFactor = 100;
            _graphs[MapType.BreeStreets] = _breeStreetsGraph;

            //Bree's road structure is a 15x11 grid
            Room[,] breeStreets = new Room[16, 11];
            Room[,] breeSewers = new Room[16, 11];
            breeStreets[0, 0] = AddRoom("Thalion/Wain", "Wain Road South/Thalion Road Intersection"); //1x1
            breeStreets[1, 0] = AddRoom("Thalion", "Thalion Road"); //2x1
            breeStreets[2, 0] = AddRoom("Thalion", "Thalion Road"); //3x1
            breeStreets[3, 0] = AddRoom("Thalion/High", "Thalion Road/South High Street Intersection"); //4x1
            breeStreets[4, 0] = AddRoom("Thalion", "Thalion Road"); //5x1
            breeStreets[5, 0] = AddRoom("Thalion", "Thalion Road"); //6x1
            breeStreets[6, 0] = AddRoom("Thalion", "Thalion Road"); //7x1
            breeStreets[7, 0] = AddRoom("Thalion/Main", "Main Street/Thalion Road Intersection"); //8x1
            Room breeDocks = breeStreets[9, 0] = AddRoom("Docks", "Bree Docks"); //10x1
            breeDocks.BoatLocationType = BoatEmbarkOrDisembark.CelduinExpressBree;
            oSewerPipeExit = breeStreets[10, 0] = AddRoom("Thalion/Crissaegrim", "Thalion Road/Crissaegrim Road"); //11x1
            breeStreets[11, 0] = AddRoom("Thalion", "Thalion Road"); //12x1
            breeStreets[12, 0] = AddRoom("Thalion", "Thalion Road"); //13x1
            breeStreets[13, 0] = AddRoom("Thalion", "Thalion Road"); //14x1
            breeStreets[14, 0] = AddRoom("Thalion/Brownhaven", "Brownhaven Road/Thalion Road Intersection"); //15x1
            breeStreets[0, 1] = AddRoom("Wain", "Wain Road South"); //1x2
            Room oToCampusFreeClinic = breeStreets[3, 1] = AddRoom("High", "South High Street"); //4x2
            breeStreets[7, 1] = AddRoom("Main", "Main Street"); //8x2
            breeStreets[10, 1] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x2
            breeStreets[14, 1] = AddRoom("Brownhaven", "Brownhaven Road"); //15x2
            breeStreets[0, 2] = AddRoom("Wain", "Wain Road South"); //1x3
            Room oToPawnShopWest = breeStreets[3, 2] = AddRoom("High", "South High Street"); //4x3
            Room oToBarracks = breeStreets[7, 2] = AddRoom("Main", "Main Street"); //8x3
            breeStreets[10, 2] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x3
            breeStreets[14, 2] = AddRoom("Brownhaven", "Brownhaven Road"); //15x3
            breeStreets[0, 3] = AddRoom("Periwinkle/Wain", "Wain Road South/Periwinkle Road Intersection"); //1x4
            breeSewers[0, 3] = AddRoom("Sewers Periwinkle/Wain", "Wain Road/Periwinkle Road Sewer Main"); //1x4
            AddExit(breeSewers[0, 3], breeStreets[0, 3], "up");
            breeStreets[1, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //2x4
            breeSewers[1, 3] = AddRoom("Sewers Periwinkle", "Periwinkle Road Sewer Main"); //2x4
            breeStreets[2, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //3x4
            breeSewers[2, 3] = AddRoom("Sewers Periwinkle", "Periwinkle Road Sewer Main"); //3x4
            breeStreets[3, 3] = AddRoom("Periwinkle/High", "South High Street/Periwinkle Road"); //4x4
            breeSewers[3, 3] = AddRoom("Sewers Periwinkle/High", "High Street/Periwinkle Road Sewer Main"); //4x4
            AddExit(breeSewers[3, 3], breeStreets[3, 3], "up");
            breeStreets[4, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //5x4
            breeSewers[4, 3] = AddRoom("Sewers Periwinkle", "Periwinkle Road Sewer Main"); //5x4
            breeStreets[5, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //6x4
            breeSewers[5, 3] = AddRoom("Sewers Periwinkle", "Periwinkle Road Sewer Main"); //6x4
            breeStreets[6, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //7x4
            breeSewers[6, 3] = AddRoom("Sewers Periwinkle", "Periwinkle Road Sewer Main"); //7x4
            breeStreets[7, 3] = AddRoom("Periwinkle/Main", "Main Street/Periwinkle Road Intersection"); //8x4
            breeSewers[7, 3] = AddRoom("Shirriffs", "Main Street/Periwinkle Road Sewer Main"); //Bree Sewers Periwinkle/Main 8x4
            AddExit(breeSewers[7, 3], breeStreets[7, 3], "up");
            breeStreets[8, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //9x4
            breeStreets[9, 3] = AddRoom("South Bridge", "South Bridge"); //10x4
            breeStreets[10, 3] = AddRoom("Periwinkle/Crissaegrim", "Periwinkle Road/Crissaegrim Road Intersection"); //11x4
            breeStreets[11, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //12x4
            Room oPeriwinklePoorAlley = breeStreets[12, 3] = AddRoom("Periwinkle/PoorAlley", "Periwinkle Road/Poor Alley Intersection"); //13x4
            breeStreets[13, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //14x4
            breeStreets[14, 3] = AddRoom("Periwinkle/Brownhaven", "Brownhaven Road/Periwinkle Road Intersection"); //15x4
            breeStreets[0, 4] = AddRoom("Wain", "Wain Road South"); //1x5
            breeSewers[0, 4] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x5
            Room oToBlindPigPubAndUniversity = breeStreets[3, 4] = AddRoom("High", "South High Street"); //4x5
            breeStreets[7, 4] = AddRoom("Main", "Main Street"); //8x5
            Room oToSnarSlystoneShoppe = breeStreets[10, 4] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x5
            breeStreets[14, 4] = AddRoom("Brownhaven", "Brownhaven Road"); //15x5
            breeStreets[0, 5] = AddRoom("Wain", "Wain Road South"); //1x6
            breeSewers[0, 5] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x6
            breeStreets[3, 5] = AddRoom("High", "South High Street"); //4x6
            breeStreets[7, 5] = AddRoom("Main", "Main Street"); //8x6
            Room oBigPapa = breeStreets[8, 5] = AddRoom("Big Papa", "Papa Joe's"); //9x6
            oBigPapa.AddPermanentMobs(MobTypeEnum.BigPapa);
            breeStreets[10, 5] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x6
            breeStreets[14, 5] = AddRoom("Brownhaven", "Brownhaven Road"); //15x6
            breeStreets[0, 6] = AddRoom("Wain", "Wain Road South"); //1x7
            breeSewers[0, 6] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x7
            breeStreets[3, 6] = AddRoom("High", "South High Street"); //4x7
            breeStreets[7, 6] = AddRoom("Main", "Main Street"); //8x7
            breeStreets[10, 6] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x7
            breeStreets[14, 6] = AddRoom("Brownhaven", "Brownhaven Road"); //15x7
            oWestGateInside = breeStreets[0, 7] = AddRoom("West Gate Inside", "West Gate of Bree"); //1x8
            breeSewers[0, 7] = AddRoom("Sewers West Gate", "Wain Road/Leviathan Way Sewer Main"); //1x8
            AddExit(breeSewers[0, 7], oWestGateInside, "up");
            breeStreets[1, 7] = AddRoom("Leviathan", "Leviathan Way"); //2x8
            Room oHauntedMansionEntrance = breeStreets[2, 7] = AddRoom("Leviathan", "Leviathan Way"); //3x8
            breeStreets[3, 7] = AddRoom("Leviathan/High", "Leviathan Way/High Street"); //4x8
            breeStreets[4, 7] = AddRoom("Leviathan", "Leviathan Way"); //5x8
            oBreeTownSquare = breeStreets[5, 7] = AddRoom("Town Square", "Bree Town Square"); //6x8
            oBreeTownSquare.AddPermanentMobs(MobTypeEnum.TheTownCrier, MobTypeEnum.Scribe, MobTypeEnum.SmallSpider, MobTypeEnum.Vagrant);
            breeStreets[6, 7] = AddRoom("Leviathan", "Leviathan Way"); //7x8
            breeStreets[7, 7] = AddRoom("Leviathan/Main", "Leviathan Way/Main Street"); //8x8
            breeStreets[8, 7] = AddRoom("Leviathan", "Leviathan Way"); //9x8
            Room oNorthBridge = breeStreets[9, 7] = AddRoom("North Bridge", "North Bridge"); //10x8
            breeStreets[10, 7] = AddRoom("Leviathan/Crissaegrim", "Leviathan Way/Crissaegrim Road"); //11x8
            breeStreets[11, 7] = AddRoom("Leviathan", "Leviathan Way"); //12x8
            Room oLeviathanPoorAlley = breeStreets[12, 7] = AddRoom("Leviathan", "Leviathan Way"); //13x8
            Room oToGrantsStables = breeStreets[13, 7] = AddRoom("Leviathan", "Leviathan Way"); //14x8
            breeEastGateInside = breeStreets[14, 7] = AddRoom("East Gate Inside", "Bree's East Gate"); //15x8
            breeStreets[0, 8] = AddRoom("Wain", "Wain Road North"); //1x9
            breeSewers[0, 8] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x9
            breeStreets[3, 8] = AddRoom("High", "North High Street"); //4x9
            breeStreets[7, 8] = AddRoom("Main", "Main Street"); //8x9
            breeStreets[10, 8] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x9
            breeStreets[14, 8] = AddRoom("Brownhaven", "Brownhaven Road"); //15x9
            _orderOfLove = breeStreets[15, 8] = AddHealingRoom("Order of Love", "Order of Love", HealingRoom.BreeNortheast); //16x9
            _orderOfLove.AddNonPermanentMobs(MobTypeEnum.Drunk, MobTypeEnum.HobbitishDoctor);
            breeStreets[0, 9] = AddRoom("Wain", "Wain Road North"); //1x10
            breeSewers[0, 9] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x10
            breeStreets[3, 9] = AddRoom("High", "North High Street"); //4x10
            breeStreets[7, 9] = AddRoom("Main", "Main Street"); //8x10
            Room oToLeonardosFoundry = breeStreets[10, 9] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x10
            Room oToGamblingPit = breeStreets[14, 9] = AddRoom("Brownhaven", "Brownhaven Road"); //15x10
            breeStreets[0, 10] = AddRoom("Ormenel/Wain", "Wain Road North/Ormenel Street Intersection"); //1x11
            breeSewers[0, 10] = AddRoom("Sewers Ormenel/Wain", "Wain Road Sewer Main"); //1x11
            Exit e = AddExit(breeStreets[0, 10], breeSewers[0, 10], "sewer");
            e.MustOpen = true;
            e.MinimumLevel = 4;
            breeStreets[1, 10] = AddRoom("Ormenel", "Ormenel Street"); //2x11
            Room oToZoo = breeStreets[2, 10] = AddRoom("Ormenel", "Ormenel Street"); //3x11
            breeStreets[3, 10] = AddRoom("Ormenel/High", "North High Street/Ormenel Street Intersection"); //4x11
            Room oToCasino = breeStreets[4, 10] = AddRoom("Ormenel", "Ormenel Street"); //5x11
            breeStreets[5, 10] = AddRoom("Ormenel", "Ormenel Street"); //6x11
            breeStreets[6, 10] = AddRoom("Ormenel", "Ormenel Street"); //7x11
            breeStreets[7, 10] = AddRoom("Ormenel/Main", "Main Street/Ormenel Street Intersection"); //8x11
            breeStreets[10, 10] = AddRoom("Ormenel/Crissaegrim", "Crissaegrim Road/Ormenel Street Intersection"); //11x11
            Room oToRealEstateOffice = breeStreets[11, 10] = AddRoom("Ormenel", "Ormenel Street"); //12x11
            graphMillwoodMansion.Rooms[oToRealEstateOffice] = new System.Windows.Point(3, 0);
            breeStreets[12, 10] = AddRoom("Ormenel", "Ormenel Street"); //13x11
            Room oStreetToFallon = breeStreets[13, 10] = AddRoom("Ormenel", "Ormenel Street"); //14x11
            breeStreets[14, 10] = AddRoom("Brownhaven/Ormenel", "Brownhaven Road/Ormenel Street Intersection"); //15x11

            AddBreeSewers(breeStreets, breeSewers, out oSmoulderingVillage, _breeStreetsGraph);

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    AddGridBidirectionalExits(breeStreets, x, y);
                }
            }

            Room oPoorAlley1 = AddRoom("Poor Alley", "Poor Alley");
            AddExit(oLeviathanPoorAlley, oPoorAlley1, "alley");
            AddExit(oPoorAlley1, oLeviathanPoorAlley, "north");
            _breeStreetsGraph.Rooms[oPoorAlley1] = new System.Windows.Point(12, 4);

            Room oPoorAlley2 = AddRoom("Poor Alley", "Poor Alley");
            AddBidirectionalExits(oPoorAlley1, oPoorAlley2, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oPoorAlley2] = new System.Windows.Point(12, 5);

            Room oPoorAlley3 = AddRoom("Poor Alley", "Poor Alley");
            AddBidirectionalExits(oPoorAlley2, oPoorAlley3, BidirectionalExitType.NorthSouth);
            AddExit(oPeriwinklePoorAlley, oPoorAlley3, "alley");
            AddExit(oPoorAlley3, oPeriwinklePoorAlley, "south");
            _breeStreetsGraph.Rooms[oPoorAlley3] = new System.Windows.Point(12, 6);

            Room oCampusFreeClinic = AddHealingRoom("Bree Campus Free Clinic", "Campus Free Clinic", HealingRoom.BreeSouthwest);
            oCampusFreeClinic.AddNonPermanentMobs(MobTypeEnum.Student);
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");
            _breeStreetsGraph.Rooms[oCampusFreeClinic] = new System.Windows.Point(4, 9);

            Room oBreeRealEstateOffice = AddRoom("Real Estate Office", "Bree Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oBreeRealEstateOffice] = new System.Windows.Point(11, -0.5);
            graphMillwoodMansion.Rooms[oBreeRealEstateOffice] = new System.Windows.Point(3, 1);

            oIxell = AddRoom("Ixell", "Kista Hills Show Home");
            oIxell.AddPermanentMobs(MobTypeEnum.IxellDeSantis);
            AddBidirectionalExitsWithOut(oBreeRealEstateOffice, oIxell, "door");
            _breeStreetsGraph.Rooms[oIxell] = new System.Windows.Point(11, -1);
            graphMillwoodMansion.Rooms[oIxell] = new System.Windows.Point(2, 1);

            Room oKistaHillsHousing = AddRoom("Kista Hills Housing", "Kista Hills Housing");
            AddBidirectionalExits(oStreetToFallon, oKistaHillsHousing, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oKistaHillsHousing] = new System.Windows.Point(13, -0.5);

            Room oChurchsEnglishGarden = AddRoom("Chuch's English Garden", "Church's English Garden");
            AddBidirectionalSameNameExit(oKistaHillsHousing, oChurchsEnglishGarden, "gate");
            Room oFallon = AddRoom("Fallon", "The Home of Church, the Cleric");
            oFallon.AddPermanentMobs(MobTypeEnum.Fallon);
            AddBidirectionalExitsWithOut(oChurchsEnglishGarden, oFallon, "door");
            _breeStreetsGraph.Rooms[oChurchsEnglishGarden] = new System.Windows.Point(13, -1);
            _breeStreetsGraph.Rooms[oFallon] = new System.Windows.Point(13, -1.5);

            Room oGrantsStables = AddRoom("Grant's stables", "Grant's stables");
            e = AddExit(oToGrantsStables, oGrantsStables, "stable");
            e.MaximumLevel = 10;
            AddExit(oGrantsStables, oToGrantsStables, "south");
            _breeStreetsGraph.Rooms[oGrantsStables] = new System.Windows.Point(13, 2.5);

            Room oGrant = AddRoom("Grant", "Grant's Office");
            oGrant.AddPermanentMobs(MobTypeEnum.Grant);
            Exit oToGrant = AddExit(oGrantsStables, oGrant, "gate");
            oToGrant.MustOpen = true;
            AddExit(oGrant, oGrantsStables, "out");
            _breeStreetsGraph.Rooms[oGrant] = new System.Windows.Point(13, 2);

            Room oDTansLeatherArmory = AddRoom("Leather Armory", "D'Tan's Leather Armory");
            AddExit(oToGrantsStables, oDTansLeatherArmory, "armory");
            AddExit(oDTansLeatherArmory, oToGrantsStables, "north");
            _breeStreetsGraph.Rooms[oDTansLeatherArmory] = new System.Windows.Point(13, 3.5);

            Room oPansy = AddRoom("Pansy Smallburrows", "Gambling Pit");
            oPansy.AddPermanentMobs(MobTypeEnum.PansySmallburrows);
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oPansy] = new System.Windows.Point(13, 1);

            oDroolie = AddRoom("Droolie", "Under North Bridge");
            oDroolie.AddPermanentMobs(MobTypeEnum.DroolieTheTroll);
            e = AddExit(oNorthBridge, oDroolie, "rope");
            e.Hidden = true;
            AddExit(oDroolie, oNorthBridge, "up");
            _breeStreetsGraph.Rooms[oDroolie] = new System.Windows.Point(9, 3.5);

            Room oIgor = AddRoom("Igor", "Blind Pig Pub");
            oIgor.AddPermanentMobs(MobTypeEnum.IgorTheBouncer);
            AddExit(oIgor, oToBlindPigPubAndUniversity, "east");
            AddExit(oToBlindPigPubAndUniversity, oIgor, "pub");
            _breeStreetsGraph.Rooms[oIgor] = new System.Windows.Point(2, 6);

            Room oSnarlingMutt = AddRoom("Snarling Mutt", "Snar Slystone's Apothecary and Curio Shoppe");
            oSnarlingMutt.AddPermanentMobs(MobTypeEnum.SnarlingMutt);
            AddBidirectionalExitsWithOut(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            _breeStreetsGraph.Rooms[oSnarlingMutt] = new System.Windows.Point(9, 6);

            Room oGuido = AddRoom("Guido", "Godfather's House of Games");
            oGuido.AddPermanentMobs(MobTypeEnum.Guido);
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");
            _breeStreetsGraph.Rooms[oGuido] = new System.Windows.Point(4, -0.5);

            Room oGodfather = AddRoom("Godfather", "Godfather's Office");
            oGodfather.AddPermanentMobs(MobTypeEnum.Godfather);
            e = AddExit(oGuido, oGodfather, "door");
            e.Hidden = true;
            e.MustOpen = true;
            e = AddExit(oGodfather, oGuido, "door");
            e.MustOpen = true;
            _breeStreetsGraph.Rooms[oGodfather] = new System.Windows.Point(4, -1);

            Room oSergeantGrimgall = AddRoom("Sergeant Grimgall", "Guard Headquarters");
            oSergeantGrimgall.AddPermanentMobs(MobTypeEnum.SergeantGrimgall);
            AddExit(oToBarracks, oSergeantGrimgall, "barracks");
            AddExit(oSergeantGrimgall, oToBarracks, "east");
            _breeStreetsGraph.Rooms[oSergeantGrimgall] = new System.Windows.Point(6, 8);

            Room oGuardsRecRoom = AddRoom("Guard's Rec Room", "Guard's Rec Room");
            AddBidirectionalExits(oSergeantGrimgall, oGuardsRecRoom, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oGuardsRecRoom] = new System.Windows.Point(6, 8.5);

            Room oBreePawnShopWest = AddRoom("Ixell's Antique Shop", "Ixell's Antique Shop");
            AddBidirectionalExits(oBreePawnShopWest, oToPawnShopWest, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBreePawnShopWest] = new System.Windows.Point(2, 8);

            Room oBreePawnShopEast = AddRoom("Pawn Shop", "Pawn Shop");
            AddBidirectionalExits(oPoorAlley1, oBreePawnShopEast, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBreePawnShopEast] = new System.Windows.Point(13, 4);

            Room oLeonardosFoundry = AddRoom("Leonardo's Foundry", "Leonardo's Foundry");
            AddExit(oToLeonardosFoundry, oLeonardosFoundry, "foundry");
            AddExit(oLeonardosFoundry, oToLeonardosFoundry, "east");
            _breeStreetsGraph.Rooms[oLeonardosFoundry] = new System.Windows.Point(9, 1);

            Room oLeonardosSwords = AddRoom("Leonardo's Swords", "Custom Swords");
            AddBidirectionalExits(oLeonardosSwords, oLeonardosFoundry, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oLeonardosSwords] = new System.Windows.Point(9, 0.5);

            Room oZooEntrance = AddRoom("Zoo Entrance", "Scranlin's Zoological Wonders");
            AddExit(oToZoo, oZooEntrance, "zoo");
            AddExit(oZooEntrance, oToZoo, "exit");
            _breeStreetsGraph.Rooms[oZooEntrance] = new System.Windows.Point(2, -0.5);

            Room oPathThroughScranlinsZoo = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo, oZooEntrance, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oPathThroughScranlinsZoo] = new System.Windows.Point(2, -1);

            Room oScranlinsPettingZoo = AddRoom("Petting Zoo", "Scranlin's Petting Zoo");
            AddExit(oPathThroughScranlinsZoo, oScranlinsPettingZoo, "north");
            AddExit(oScranlinsPettingZoo, oPathThroughScranlinsZoo, "south");
            _breeStreetsGraph.Rooms[oScranlinsPettingZoo] = new System.Windows.Point(2, -1.25);

            Room oScranlinsTrainingArea = AddRoom("Training Area", "Scranlin's Training Area");
            e = AddExit(oScranlinsPettingZoo, oScranlinsTrainingArea, "clearing");
            e.Hidden = true;
            AddExit(oScranlinsTrainingArea, oScranlinsPettingZoo, "gate");
            _breeStreetsGraph.Rooms[oScranlinsTrainingArea] = new System.Windows.Point(2, -1.5);

            Room oScranlin = AddRoom("Scranlin", "Scranlin's Outhouse");
            oScranlin.AddPermanentMobs(MobTypeEnum.Scranlin);
            e = AddExit(oScranlinsTrainingArea, oScranlin, "outhouse");
            e.Hidden = true;
            AddExit(oScranlin, oScranlinsTrainingArea, "out");
            _breeStreetsGraph.Rooms[oScranlin] = new System.Windows.Point(2, -1.75);

            Room oPathThroughScranlinsZoo2 = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo2, oPathThroughScranlinsZoo, BidirectionalExitType.SoutheastNorthwest);
            _breeStreetsGraph.Rooms[oPathThroughScranlinsZoo2] = new System.Windows.Point(1, -2);

            Room oPathThroughScranlinsZoo3 = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo3, oPathThroughScranlinsZoo2, BidirectionalExitType.SouthwestNortheast);
            _breeStreetsGraph.Rooms[oPathThroughScranlinsZoo3] = new System.Windows.Point(2, -3);

            Room oPathThroughScranlinsZoo4 = AddRoom("Path", "Path through Scranlin's Zoo");
            e = AddExit(oPathThroughScranlinsZoo3, oPathThroughScranlinsZoo4, "southeast");
            e.MaximumLevel = 10;
            AddExit(oPathThroughScranlinsZoo4, oPathThroughScranlinsZoo3, "northwest");
            e = AddExit(oPathThroughScranlinsZoo, oPathThroughScranlinsZoo4, "northeast");
            e.MaximumLevel = 10;
            AddExit(oPathThroughScranlinsZoo4, oPathThroughScranlinsZoo, "southwest");
            _breeStreetsGraph.Rooms[oPathThroughScranlinsZoo4] = new System.Windows.Point(3, -2);

            Room oDogHouse = AddRoom("Dog House", "The Dog House");
            oDogHouse.AddPermanentMobs(MobTypeEnum.Lathlorien);
            AddBidirectionalExitsWithOut(oPathThroughScranlinsZoo2, oDogHouse, "doghouse");
            _breeStreetsGraph.Rooms[oDogHouse] = new System.Windows.Point(1, -1);

            Room oMonkeyHouse = AddRoom("Monkey House", "Monkey House");
            AddBidirectionalExits(oMonkeyHouse, oPathThroughScranlinsZoo4, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oMonkeyHouse] = new System.Windows.Point(2.67, -2);

            Room oReptileHouse = AddRoom("Reptile House", "Reptile House");
            AddBidirectionalExits(oPathThroughScranlinsZoo4, oReptileHouse, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oReptileHouse] = new System.Windows.Point(4, -2);

            Room oCreaturesOfMyth = AddRoom("Creatures of Myth", "Creatures of Myth");
            e = AddExit(oPathThroughScranlinsZoo2, oCreaturesOfMyth, "west");
            e.MinimumLevel = 10;
            AddExit(oCreaturesOfMyth, oPathThroughScranlinsZoo2, "east");
            _breeStreetsGraph.Rooms[oCreaturesOfMyth] = new System.Windows.Point(0, -2);

            Room oGeneticBlunders = AddRoom("Genetic Blunders", "Genetic Blunders");
            e = AddExit(oPathThroughScranlinsZoo2, oGeneticBlunders, "east");
            e.MinimumLevel = 4;
            AddExit(oGeneticBlunders, oPathThroughScranlinsZoo2, "west");
            _breeStreetsGraph.Rooms[oGeneticBlunders] = new System.Windows.Point(1.67, -2);

            Room oBeastsOfFire = AddRoom("Beasts of Fire", "Beasts of Fire");
            e = AddExit(oPathThroughScranlinsZoo3, oBeastsOfFire, "north");
            e.MustOpen = true;
            e.MinimumLevel = 5;
            e = AddExit(oBeastsOfFire, oPathThroughScranlinsZoo3, "door");
            e.MustOpen = true;
            _breeStreetsGraph.Rooms[oBeastsOfFire] = new System.Windows.Point(2, -4);

            Room oOceania = AddRoom("Oceania", "Oceania");
            e = AddExit(oPathThroughScranlinsZoo3, oOceania, "south");
            e.MinimumLevel = 4;
            AddExit(oOceania, oPathThroughScranlinsZoo3, "north");
            _breeStreetsGraph.Rooms[oOceania] = new System.Windows.Point(2, -2.5);
            //CSRTODO: tank (fly)

            boatswain = AddRoom("Boatswain", "Stern of the Celduin Express");
            boatswain.AddPermanentMobs(MobTypeEnum.Boatswain);
            boatswain.BoatLocationType = BoatEmbarkOrDisembark.CelduinExpress;
            _breeStreetsGraph.Rooms[boatswain] = new System.Windows.Point(9, 9.5);
            e = AddExit(breeDocks, boatswain, "steamboat");
            e.PresenceType = ExitPresenceType.Periodic;
            e = AddExit(boatswain, breeDocks, "dock");
            e.PresenceType = ExitPresenceType.Periodic;

            Room oPearlAlley = AddRoom("Pearl Alley", "Pearl Alley");
            AddExit(oBreeTownSquare, oPearlAlley, "alley");
            AddExit(oPearlAlley, oBreeTownSquare, "north");
            _breeStreetsGraph.Rooms[oPearlAlley] = new System.Windows.Point(5, 3.5);

            Room oBartenderWaitress = AddRoom("Prancing Pony Bar/Wait", "Prancing Pony Tavern");
            oBartenderWaitress.AddPermanentMobs(MobTypeEnum.Bartender, MobTypeEnum.Bartender, MobTypeEnum.Waitress, MobTypeEnum.Waitress, MobTypeEnum.Waitress);
            AddBidirectionalExits(oPearlAlley, oBartenderWaitress, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBartenderWaitress] = new System.Windows.Point(6, 3.5);

            Room oHobbitsHideawayEntrance = AddRoom("Hideaway Entrance", "Entrance to the Hobbit's Hideaway");
            e = AddExit(_orderOfLove, oHobbitsHideawayEntrance, "cubbyhole");
            e.Hidden = true;
            e.MaximumLevel = 8;
            AddExit(oHobbitsHideawayEntrance, _orderOfLove, "west");
            _breeStreetsGraph.Rooms[oHobbitsHideawayEntrance] = new System.Windows.Point(16, 2);

            Room oHobbitClearing = AddRoom("Hobbit Clearing", "Hobbit Clearing");
            AddBidirectionalExits(oHobbitsHideawayEntrance, oHobbitClearing, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oHobbitClearing] = new System.Windows.Point(17, 2);

            Room oChiefsHole = AddRoom("Chief's Hole", "Chief's Hole");
            AddBidirectionalExitsWithOut(oHobbitClearing, oChiefsHole, "chief's");
            _breeStreetsGraph.Rooms[oChiefsHole] = new System.Windows.Point(16, 1);

            Room oBranco = AddRoom("Branco", "The Chief's Bedchambers");
            oBranco.AddPermanentMobs(MobTypeEnum.BrancoTheHobbitsChief);
            AddBidirectionalExitsWithOut(oChiefsHole, oBranco, "bedchambers");
            _breeStreetsGraph.Rooms[oBranco] = new System.Windows.Point(15, 1);

            Room oHobbitsTemple = AddRoom("Temple", "The Hobbit's Temple");
            AddBidirectionalExitsWithOut(oHobbitClearing, oHobbitsTemple, "temple");
            _breeStreetsGraph.Rooms[oHobbitsTemple] = new System.Windows.Point(16, 2.5);

            Room oBeneathTheHobbitsAltar = AddRoom("Under Altar", "Beneath the Hobbit's Altar");
            oBeneathTheHobbitsAltar.AddPermanentMobs(MobTypeEnum.LuthicTheHighPriestess);
            oBeneathTheHobbitsAltar.IsTrapRoom = true;
            e = AddExit(oHobbitsTemple, oBeneathTheHobbitsAltar, "altar");
            e.Hidden = true;
            AddExit(oBeneathTheHobbitsAltar, oHobbitsTemple, "up");
            _breeStreetsGraph.Rooms[oBeneathTheHobbitsAltar] = new System.Windows.Point(17, 2.5);

            breeEastGateOutside = AddRoom("East Gate Outside", "East Gate of Bree");
            _breeStreetsGraph.Rooms[breeEastGateOutside] = new System.Windows.Point(18, 3);

            oCemetery = AddRoom("Cemetery", "The Cemetery");
            e = AddExit(breeEastGateOutside, oCemetery, "path");
            e.RequiresDay = true;
            e = AddExit(oCemetery, oHobbitClearing, "west");
            e.MaximumLevel = 8;
            _breeStreetsGraph.Rooms[oCemetery] = new System.Windows.Point(18, 2);

            Room oCommonArea = AddRoom("Common Area", "The Common Area");
            AddBidirectionalExitsWithOut(oHobbitClearing, oCommonArea, "common");
            _breeStreetsGraph.Rooms[oCommonArea] = new System.Windows.Point(17, 1);

            Room oMainDiningHall = AddRoom("Dining Hall", "The Main Dining Hall");
            AddBidirectionalExitsWithOut(oCommonArea, oMainDiningHall, "dining");
            _breeStreetsGraph.Rooms[oMainDiningHall] = new System.Windows.Point(17, 0);

            Room oBigPapaSmallHallway = AddRoom("Small Hallway", "Small hallway");
            e = AddExit(oBigPapa, oBigPapaSmallHallway, "panel");
            e.Hidden = true;
            e = AddExit(oBigPapaSmallHallway, oBigPapa, "panel");
            e.MustOpen = true;
            _breeStreetsGraph.Rooms[oBigPapaSmallHallway] = new System.Windows.Point(8, 4.5);

            AddLocation(_aBreePerms, oGuido);
            AddLocation(_aBreePerms, oGodfather);
            AddLocation(_aBreePerms, oFallon);
            AddLocation(_aBreePerms, oBigPapa);
            AddLocation(_aBreePerms, oScranlin);

            AddHauntedMansion(oHauntedMansionEntrance, _breeStreetsGraph);
        }

        private void AddHauntedMansion(Room hauntedMansionEntrance, RoomGraph breeStreetsGraph)
        {
            RoomGraph hauntedMansionGraph = new RoomGraph("Bree Haunted Mansion");
            hauntedMansionGraph.ScalingFactor = 100;
            _graphs[MapType.BreeHauntedMansion] = hauntedMansionGraph;

            hauntedMansionGraph.Rooms[hauntedMansionEntrance] = new System.Windows.Point(2, 8);

            Room oOldGardener = AddRoom("Old Gardener", "Path to Mansion");
            oOldGardener.AddPermanentMobs(MobTypeEnum.OldGardener);
            Exit e = AddExit(hauntedMansionEntrance, oOldGardener, "gate");
            e.KeyType = KeyType.SilverKey;
            e.MustOpen = true;
            AddExit(oOldGardener, hauntedMansionEntrance, "gate");
            breeStreetsGraph.Rooms[oOldGardener] = new System.Windows.Point(2, 2.5);
            hauntedMansionGraph.Rooms[oOldGardener] = new System.Windows.Point(2, 7);

            Room oFoyer = AddRoom("Foyer", "Foyer of the Old Mansion");
            e = AddExit(oOldGardener, oFoyer, "door");
            e.KeyType = KeyType.SilverKey;
            e.MustOpen = true;
            AddExit(oFoyer, oOldGardener, "out");
            hauntedMansionGraph.Rooms[oFoyer] = new System.Windows.Point(2, 6);

            Room oDiningHall1 = AddRoom("Dining Hall", "The Mansion Dining Hall");
            AddBidirectionalExits(oDiningHall1, oFoyer, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oDiningHall1] = new System.Windows.Point(1, 6);

            Room oDiningHall2 = AddRoom("Dining Hall", "North end of the Dining Hall");
            AddBidirectionalExits(oDiningHall2, oDiningHall1, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oDiningHall2] = new System.Windows.Point(1, 5);

            Room oKitchen = AddRoom("Kitchen", "Kitchen");
            AddBidirectionalExits(oDiningHall2, oKitchen, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oKitchen] = new System.Windows.Point(1.5, 5);

            Room oDarkHallway = AddRoom("Dark Hallway", "Dark Hallway");
            AddBidirectionalExits(oDarkHallway, oDiningHall2, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oDarkHallway] = new System.Windows.Point(1, 4);

            Room oStudy = AddRoom("Damaged Skeleton", "Study");
            oStudy.AddPermanentMobs(MobTypeEnum.DamagedSkeleton);
            e = AddExit(oDarkHallway, oStudy, "door");
            e.MustOpen = true;
            AddExit(oStudy, oDarkHallway, "door");
            hauntedMansionGraph.Rooms[oStudy] = new System.Windows.Point(1, 3);

            Room oLivingRoom = AddRoom("Living Room", "Living Room");
            AddBidirectionalExits(oFoyer, oLivingRoom, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oLivingRoom] = new System.Windows.Point(3, 6);

            Room oHallway = AddRoom("Hallway", "Hallway");
            AddBidirectionalExits(oHallway, oLivingRoom, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oHallway] = new System.Windows.Point(3, 5);

            Room oBedroom = AddRoom("Bedroom", "Bedroom");
            e = AddExit(oHallway, oBedroom, "door");
            e.MustOpen = true;
            AddExit(oBedroom, oHallway, "door");
            hauntedMansionGraph.Rooms[oBedroom] = new System.Windows.Point(3, 4);

            Room oStairwellTop = AddRoom("Stairwell Top", "Top of the Stairwell");
            AddBidirectionalExits(oStairwellTop, oFoyer, BidirectionalExitType.UpDown);
            hauntedMansionGraph.Rooms[oStairwellTop] = new System.Windows.Point(2, 2);

            Room oSoutheasternHallwayCorner = AddRoom("Hallway", "Southeastern Hallway Corner");
            AddBidirectionalExits(oStairwellTop, oSoutheasternHallwayCorner, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oSoutheasternHallwayCorner] = new System.Windows.Point(3, 2);

            Room oEasternHallway = AddRoom("Hallway", "Eastern Hallway");
            AddBidirectionalExits(oEasternHallway, oSoutheasternHallwayCorner, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oEasternHallway] = new System.Windows.Point(3, 1);

            Room oChildsBedroom = AddRoom("Child's Bedroom", "Child's Bedroom");
            e = AddExit(oEasternHallway, oChildsBedroom, "door");
            e.MustOpen = true;
            AddExit(oChildsBedroom, oEasternHallway, "door");
            hauntedMansionGraph.Rooms[oChildsBedroom] = new System.Windows.Point(2, 1);

            Room oGhostlyFencer = AddRoom("Ghostly Fencer", "Decrepit Training Room");
            AddExit(oEasternHallway, oGhostlyFencer, "north");
            AddExit(oGhostlyFencer, oEasternHallway, "southeast");
            hauntedMansionGraph.Rooms[oGhostlyFencer] = new System.Windows.Point(2, 0);

            Room oWesternHallway = AddRoom("Hallway", "Western Hallway");
            AddExit(oWesternHallway, oGhostlyFencer, "north");
            AddExit(oGhostlyFencer, oWesternHallway, "southwest");
            hauntedMansionGraph.Rooms[oWesternHallway] = new System.Windows.Point(0, 1);

            Room oSouthwesternHallwayCorner = AddRoom("Hallway", "Southwestern Hallway Corner");
            AddBidirectionalExits(oWesternHallway, oSouthwesternHallwayCorner, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oSouthwesternHallwayCorner] = new System.Windows.Point(0, 2);

            Room oWesternHallway3 = AddRoom("Hallway", "Hallway");
            AddExit(oSouthwesternHallwayCorner, oWesternHallway3, "east");
            AddBidirectionalExits(oWesternHallway3, oStairwellTop, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oWesternHallway3] = new System.Windows.Point(1, 2);

            Room oDen = AddRoom("Den", "Den");
            e = AddExit(oWesternHallway3, oDen, "door");
            e.MustOpen = true;
            AddExit(oDen, oWesternHallway3, "door");
            hauntedMansionGraph.Rooms[oDen] = new System.Windows.Point(1, 1);
        }

        private void AddUnderBree(Room droolie, Room oOuthouse, Room oSewerPipeExit)
        {
            RoomGraph underBreeGraph = new RoomGraph("Under Bree");
            underBreeGraph.ScalingFactor = 100;
            _graphs[MapType.UnderBree] = underBreeGraph;

            underBreeGraph.Rooms[droolie] = new System.Windows.Point(0, 0);
            underBreeGraph.Rooms[oOuthouse] = new System.Windows.Point(8, 12);
            underBreeGraph.Rooms[oSewerPipeExit] = new System.Windows.Point(7, 2);

            Room oCatchBasin = AddRoom("Catch Basin", "Catch Basin");
            AddBidirectionalExitsWithOut(oOuthouse, oCatchBasin, "hole");
            underBreeGraph.Rooms[oCatchBasin] = new System.Windows.Point(8, 11);

            Room oSepticTank = AddRoom("Septic Tank", "Septic Tank");
            AddBidirectionalSameNameExit(oCatchBasin, oSepticTank, "grate");
            underBreeGraph.Rooms[oSepticTank] = new System.Windows.Point(8, 10);

            Room oDrainPipe1 = AddRoom("Drain Pipe", "Drain Pipe");
            AddBidirectionalSameNameExit(oSepticTank, oDrainPipe1, "pipe");
            underBreeGraph.Rooms[oDrainPipe1] = new System.Windows.Point(8, 9);

            Room oDrainPipe2 = AddRoom("Drain Pipe", "Drain Pipe");
            AddBidirectionalSameNameExit(oDrainPipe1, oDrainPipe2, "culvert");
            underBreeGraph.Rooms[oDrainPipe2] = new System.Windows.Point(8, 8);

            Room oBrandywineRiverShore = AddRoom("Brandywine Shore", "Southeastern Shore of the Brandywine River");
            AddBidirectionalExitsWithOut(oBrandywineRiverShore, oDrainPipe2, "grate");
            underBreeGraph.Rooms[oBrandywineRiverShore] = new System.Windows.Point(8, 7);

            Room oSewerDitch = AddRoom("Sewer Ditch", "Sewer Ditch");
            AddBidirectionalExitsWithOut(oBrandywineRiverShore, oSewerDitch, "ditch");
            underBreeGraph.Rooms[oSewerDitch] = new System.Windows.Point(8, 6);

            Room oSewerTunnel1 = AddRoom("Sewer Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel1, oSewerDitch, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTunnel1] = new System.Windows.Point(8, 5);

            Room oSewerTConnection = AddRoom("Sewer T-Connection", "Sewer T-Connection");
            AddBidirectionalExits(oSewerTConnection, oSewerTunnel1, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTConnection] = new System.Windows.Point(8, 4);

            Room oSewerTunnel2 = AddRoom("Sewer Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel2, oSewerTConnection, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerTunnel2] = new System.Windows.Point(7, 4);

            Room oSewerPipe = AddRoom("Sewer Pipe", "Sewer Pipe");
            AddExit(oSewerTunnel2, oSewerPipe, "pipe");
            AddExit(oSewerPipe, oSewerTunnel2, "down");
            AddExit(oSewerPipe, oSewerPipeExit, "up");
            underBreeGraph.Rooms[oSewerPipe] = new System.Windows.Point(7, 3);

            Room oSalamander = AddRoom("Salamander", "The Brandywine River");
            oSalamander.AddPermanentMobs(MobTypeEnum.Salamander);
            AddExit(oBrandywineRiverShore, oSalamander, "reeds");
            AddExit(oSalamander, oBrandywineRiverShore, "shore");
            underBreeGraph.Rooms[oSalamander] = new System.Windows.Point(9, 7);

            Room oBrandywineRiver1 = AddRoom("Brandywine River", "The Brandywine River");
            oBrandywineRiver1.DamageType = RealmType.Water;
            AddExit(droolie, oBrandywineRiver1, "down");
            Exit e = AddExit(oBrandywineRiver1, droolie, "rope");
            e.FloatRequirement = FloatRequirement.Fly;
            underBreeGraph.Rooms[oBrandywineRiver1] = new System.Windows.Point(0, 1);
            //CSRTODO: north

            Room oBrandywineRiver2 = AddRoom("Brandywine River", "The Brandywine River");
            oBrandywineRiver2.DamageType = RealmType.Water;
            AddBidirectionalExits(oBrandywineRiver1, oBrandywineRiver2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver2] = new System.Windows.Point(1, 1);

            Room oOohlgrist = AddRoom("Oohlgrist", "Small Boat");
            oOohlgrist.AddPermanentMobs(MobTypeEnum.Oohlgrist);
            AddExit(oBrandywineRiver2, oOohlgrist, "boat");
            AddExit(oOohlgrist, oBrandywineRiver2, "river");
            underBreeGraph.Rooms[oOohlgrist] = new System.Windows.Point(2, 1);

            Room oBrandywineRiverBoathouse = AddRoom("Brandywine Boathouse", "Brandywine River Boathouse");
            AddExit(oOohlgrist, oBrandywineRiverBoathouse, "shore");
            AddExit(oBrandywineRiverBoathouse, oOohlgrist, "boat");
            underBreeGraph.Rooms[oBrandywineRiverBoathouse] = new System.Windows.Point(3, 1);

            Room oRockyBeach1 = AddRoom("Rocky Beach", "Rocky Beach");
            AddBidirectionalExits(oBrandywineRiverBoathouse, oRockyBeach1, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oRockyBeach1] = new System.Windows.Point(4, 1);

            Room oRockyBeach2 = AddRoom("Rocky Beach", "Rocky Beach");
            AddBidirectionalExits(oRockyBeach1, oRockyBeach2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oRockyBeach2] = new System.Windows.Point(5, 1);

            Room oHermitsCave = AddRoom("Hermit Fisher", "Hermit's Cave");
            oHermitsCave.AddPermanentMobs(MobTypeEnum.HermitFisher);
            e = AddExit(oRockyBeach2, oHermitsCave, "cave");
            e.Hidden = true;
            AddExit(oHermitsCave, oRockyBeach2, "out");
            underBreeGraph.Rooms[oHermitsCave] = new System.Windows.Point(6, 1);

            Room oRockyAlcove = AddRoom("Rocky Alcove", "Rocky Alcove");
            AddExit(oRockyBeach1, oRockyAlcove, "alcove");
            AddExit(oRockyAlcove, oRockyBeach1, "north");
            underBreeGraph.Rooms[oRockyAlcove] = new System.Windows.Point(5, 0);

            Room oSewerDrain = AddRoom("Sewer Drain", "Sewer Drain");
            AddBidirectionalSameNameExit(oRockyAlcove, oSewerDrain, "grate");
            underBreeGraph.Rooms[oSewerDrain] = new System.Windows.Point(7, 0);

            Room oDrainTunnel1 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddExit(oSewerDrain, oDrainTunnel1, "south");
            underBreeGraph.Rooms[oDrainTunnel1] = new System.Windows.Point(7, 1);

            Room oDrainTunnel2 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddExit(oDrainTunnel1, oDrainTunnel2, "north");
            underBreeGraph.Rooms[oDrainTunnel2] = new System.Windows.Point(8, 0);

            Room oDrainTunnel3 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddExit(oDrainTunnel2, oDrainTunnel3, "south");
            underBreeGraph.Rooms[oDrainTunnel3] = new System.Windows.Point(8, 1);

            Room oDrainTunnel4 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddExit(oDrainTunnel3, oDrainTunnel4, "south");
            underBreeGraph.Rooms[oDrainTunnel4] = new System.Windows.Point(8, 2);

            Room sewerTunnelToTConnection = AddRoom("Sewer Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oDrainTunnel4, sewerTunnelToTConnection, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(sewerTunnelToTConnection, oSewerTConnection, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[sewerTunnelToTConnection] = new System.Windows.Point(8, 3);

            Room oBoardedSewerTunnel = AddRoom("Boarded Tunnel", "Boarded Sewer Tunnel");
            AddBidirectionalExits(sewerTunnelToTConnection, oBoardedSewerTunnel, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBoardedSewerTunnel] = new System.Windows.Point(9, 3);

            Room oSewerOrcChamber = AddRoom("Sewer Orc Guards", "Sewer Orc Chamber");
            oSewerOrcChamber.AddPermanentMobs(MobTypeEnum.SewerOrcGuard, MobTypeEnum.SewerOrcGuard);
            e = AddExit(oBoardedSewerTunnel, oSewerOrcChamber, "busted board");
            e.Hidden = true;
            e = AddExit(oSewerOrcChamber, oBoardedSewerTunnel, "busted board");
            e.Hidden = true;
            underBreeGraph.Rooms[oSewerOrcChamber] = new System.Windows.Point(10, 3);

            Room oSewerOrcLair = AddRoom("Sewer Orc Lair");
            AddBidirectionalExits(oSewerOrcLair, oSewerOrcChamber, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerOrcLair] = new System.Windows.Point(10, 2);

            Room oSewerPassage = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSewerOrcLair, oSewerPassage, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerPassage] = new System.Windows.Point(11, 2);

            Room oSewerOrcStorageRoom = AddRoom("Storage Room");
            AddBidirectionalExits(oSewerPassage, oSewerOrcStorageRoom, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerOrcStorageRoom] = new System.Windows.Point(12, 2);

            Room oSlopingSewerPassage = AddRoom("Sloping Sewer Passage");
            AddBidirectionalExits(oSewerOrcStorageRoom, oSlopingSewerPassage, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSlopingSewerPassage] = new System.Windows.Point(12, 3);

            Room oSewerPassageInFrontOfGate = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSlopingSewerPassage, oSewerPassageInFrontOfGate, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerPassageInFrontOfGate] = new System.Windows.Point(12, 4);

            AddLocation(_aBreePerms, oSalamander);
        }

        private void AddBreeSewers(Room[,] breeStreets, Room[,] breeSewers, out Room oSmoulderingVillage, RoomGraph breeStreetsGraph)
        {
            RoomGraph breeSewersGraph = new RoomGraph("Bree Sewers");
            breeSewersGraph.ScalingFactor = 100;
            _graphs[MapType.BreeSewers] = breeSewersGraph;

            //add exits for the sewers. due to screwiness on periwinkle this can't be done automatically.
            AddBidirectionalExits(breeSewers[0, 10], breeSewers[0, 9], BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[breeSewers[0, 10]] = new System.Windows.Point(-1, -1);
            AddBidirectionalExits(breeSewers[0, 9], breeSewers[0, 8], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 8], breeSewers[0, 7], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 7], breeSewers[0, 6], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 6], breeSewers[0, 5], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 5], breeSewers[0, 4], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 4], breeSewers[0, 3], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 3], breeSewers[1, 3], BidirectionalExitType.WestEast);
            AddExit(breeSewers[1, 3], breeSewers[3, 3], "east");
            AddExit(breeSewers[3, 3], breeSewers[2, 3], "west");
            AddExit(breeSewers[2, 3], breeSewers[1, 3], "west");
            AddBidirectionalExits(breeSewers[3, 3], breeSewers[4, 3], BidirectionalExitType.WestEast);
            AddBidirectionalExits(breeSewers[4, 3], breeSewers[5, 3], BidirectionalExitType.WestEast);
            AddBidirectionalExits(breeSewers[5, 3], breeSewers[6, 3], BidirectionalExitType.WestEast);
            AddBidirectionalExits(breeSewers[6, 3], breeSewers[7, 3], BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[breeStreets[0, 10]] = new System.Windows.Point(5, 1);
            breeSewersGraph.Rooms[breeSewers[0, 10]] = new System.Windows.Point(5, 2);
            breeSewersGraph.Rooms[breeSewers[0, 9]] = new System.Windows.Point(5, 3);
            breeSewersGraph.Rooms[breeSewers[0, 8]] = new System.Windows.Point(5, 4);
            breeSewersGraph.Rooms[breeSewers[0, 7]] = new System.Windows.Point(5, 5);
            breeSewersGraph.Rooms[breeSewers[0, 6]] = new System.Windows.Point(5, 6);
            breeSewersGraph.Rooms[breeSewers[0, 5]] = new System.Windows.Point(5, 7);
            breeSewersGraph.Rooms[breeSewers[0, 4]] = new System.Windows.Point(5, 8);
            breeSewersGraph.Rooms[breeSewers[0, 3]] = new System.Windows.Point(5, 9);
            breeSewersGraph.Rooms[breeSewers[1, 3]] = new System.Windows.Point(6, 9);
            breeSewersGraph.Rooms[breeSewers[2, 3]] = new System.Windows.Point(7, 8);
            breeSewersGraph.Rooms[breeSewers[3, 3]] = new System.Windows.Point(8, 9);
            breeSewersGraph.Rooms[breeSewers[4, 3]] = new System.Windows.Point(9, 9);
            breeSewersGraph.Rooms[breeSewers[5, 3]] = new System.Windows.Point(10, 9);
            breeSewersGraph.Rooms[breeSewers[6, 3]] = new System.Windows.Point(11, 9);
            breeSewersGraph.Rooms[breeSewers[7, 3]] = new System.Windows.Point(12, 9);

            Room oTunnel = AddRoom("Tunnel", "Tunnel");
            Exit e = AddExit(breeSewers[0, 10], oTunnel, "tunnel");
            e.Hidden = true;
            AddExit(oTunnel, breeSewers[0, 10], "tunnel");
            breeSewersGraph.Rooms[oTunnel] = new System.Windows.Point(4, 2);

            Room oLatrine = AddRoom("Latrine", "Latrine");
            oLatrine.DamageType = RealmType.Wind;
            AddExit(oTunnel, oLatrine, "south");
            e = AddExit(oLatrine, oTunnel, "north");
            e.Hidden = true;
            breeSewersGraph.Rooms[oLatrine] = new System.Windows.Point(4, 3);

            Room oEugenesDungeon = AddRoom("Eugene's Dungeon", "Eugene's Dungeon");
            AddBidirectionalExits(oEugenesDungeon, oLatrine, BidirectionalExitType.SouthwestNortheast);
            breeSewersGraph.Rooms[oEugenesDungeon] = new System.Windows.Point(3, 2);

            Room oShadowOfIncendius = AddRoom("Shadow of Incendius", "Honorary Holding");
            oShadowOfIncendius.AddPermanentMobs(MobTypeEnum.ShadowOfIncendius);
            AddBidirectionalExits(oShadowOfIncendius, oEugenesDungeon, BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[oShadowOfIncendius] = new System.Windows.Point(2, 2);

            Room oEugeneTheExecutioner = AddRoom("Eugene the Executioner", "Torture Room");
            oEugeneTheExecutioner.AddPermanentMobs(MobTypeEnum.EugeneTheExecutioner);
            oEugeneTheExecutioner.DamageType = RealmType.Fire;
            AddExit(oEugenesDungeon, oEugeneTheExecutioner, "up");
            oEugeneTheExecutioner.IsTrapRoom = true;
            breeSewersGraph.Rooms[oEugeneTheExecutioner] = new System.Windows.Point(3, 1);

            Room oBurnedRemainsOfNimrodel = AddRoom("Nimrodel", "Cellar");
            oBurnedRemainsOfNimrodel.AddPermanentMobs(MobTypeEnum.BurnedRemainsOfNimrodel);
            AddBidirectionalExitsWithOut(oBurnedRemainsOfNimrodel, oEugeneTheExecutioner, "door");
            breeSewersGraph.Rooms[oBurnedRemainsOfNimrodel] = new System.Windows.Point(2, 1);

            Room aqueduct = AddRoom("Aqueduct", "Aqueduct");
            AddBidirectionalExitsWithOut(oBurnedRemainsOfNimrodel, aqueduct, "pipe");
            breeSewersGraph.Rooms[aqueduct] = new System.Windows.Point(1, 2);

            Room oShirriff = breeSewers[7, 3];
            oShirriff.AddPermanentMobs(MobTypeEnum.Shirriff, MobTypeEnum.Shirriff);

            Room oValveChamber = AddRoom("Valve Chamber", "Valve chamber");
            e = AddExit(breeSewers[7, 3], oValveChamber, "valve");
            e.Hidden = true;
            AddExit(oValveChamber, breeSewers[7, 3], "south");
            breeSewersGraph.Rooms[oValveChamber] = new System.Windows.Point(12, 8);

            Room oSewerPassageFromValveChamber = AddRoom("Sewer Passage", "Sewer Passage");
            AddBidirectionalExits(oSewerPassageFromValveChamber, oValveChamber, BidirectionalExitType.NorthSouth);
            breeSewersGraph.Rooms[oSewerPassageFromValveChamber] = new System.Windows.Point(12, 7);

            Room oCentralSewerChannels = AddRoom("Central Sewer Channels", "Central Sewer Channels");
            //CSRTODO
            //oCentralSewerChannels.Mob1 = "demon";
            AddBidirectionalExits(oCentralSewerChannels, oSewerPassageFromValveChamber, BidirectionalExitType.SoutheastNorthwest);
            breeSewersGraph.Rooms[oCentralSewerChannels] = new System.Windows.Point(11, 6);

            Room oSewerPassageToSewerDemon = AddRoom("Passage", "Sewer Passage");
            oSewerPassageToSewerDemon.DamageType = RealmType.Earth;
            e = AddExit(oCentralSewerChannels, oSewerPassageToSewerDemon, "northwest");
            e.Hidden = true;
            AddExit(oSewerPassageToSewerDemon, oCentralSewerChannels, "southeast");
            breeSewersGraph.Rooms[oSewerPassageToSewerDemon] = new System.Windows.Point(10, 5);

            Room oSewerPassageFromCentChannel = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oCentralSewerChannels, oSewerPassageFromCentChannel, BidirectionalExitType.SouthwestNortheast);
            breeSewersGraph.Rooms[oSewerPassageFromCentChannel] = new System.Windows.Point(10, 7);

            Room oSewerTIntersection = AddRoom("T-Intersection", "Sewer T-Intersection");
            AddBidirectionalExits(oSewerTIntersection, oSewerPassageFromCentChannel, BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[oSewerTIntersection] = new System.Windows.Point(9, 7);

            Room oSewerValveChamber2 = AddRoom("Valve Chamber", "Sewer Valve Chamber");
            AddBidirectionalExits(oSewerValveChamber2, oSewerTIntersection, BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[oSewerValveChamber2] = new System.Windows.Point(8, 7);

            Room oSewerPassageToDrainageChamber = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oSewerTIntersection, oSewerPassageToDrainageChamber, BidirectionalExitType.NorthSouth);
            breeSewersGraph.Rooms[oSewerPassageToDrainageChamber] = new System.Windows.Point(8, 7.5);

            Room oDrainageChamber = AddRoom("Drainage Chamber", "Drainage Chamber");
            e = AddExit(oSewerPassageToDrainageChamber, oDrainageChamber, "door");
            e.MustOpen = true;
            AddExit(oDrainageChamber, oSewerPassageToDrainageChamber, "door");
            breeSewersGraph.Rooms[oDrainageChamber] = new System.Windows.Point(8, 8);

            oSmoulderingVillage = AddRoom("Smoldering Village", "Smoldering village");
            breeSewersGraph.Rooms[oSmoulderingVillage] = new System.Windows.Point(0, 0);

            Room oWell = AddRoom("Well", "Well");
            AddExit(oSmoulderingVillage, oWell, "well");
            AddExit(oWell, oSmoulderingVillage, "ladder");
            breeSewersGraph.Rooms[oWell] = new System.Windows.Point(1, 0);

            Room oKasnarTheGuard = AddRoom("Kasnar", "Water Pipe");
            oKasnarTheGuard.AddPermanentMobs(MobTypeEnum.KasnarTheGuard);
            AddExit(oWell, oKasnarTheGuard, "pipe");
            AddExit(oKasnarTheGuard, oWell, "north");
            breeSewersGraph.Rooms[oKasnarTheGuard] = new System.Windows.Point(1, 1);

            AddExit(aqueduct, oKasnarTheGuard, "north");
            e = AddExit(oKasnarTheGuard, aqueduct, "south");
            e.KeyType = KeyType.KasnarsRedKey;
            e.MustOpen = true;

            AddLocation(_aBreePerms, oShirriff);
            AddLocation(_aBreePerms, oBurnedRemainsOfNimrodel);
            AddLocation(_aBreePerms, oKasnarTheGuard);
        }

        private void AddGridBidirectionalExits(Room[,] grid, int x, int y)
        {
            Room r = grid[x, y];
            if (r != null)
            {
                _breeStreetsGraph.Rooms[r] = new System.Windows.Point(x, 10 - y);

                //look for a square to the west and add the east/west exits
                if (x > 0)
                {
                    Room roomToWest = grid[x - 1, y];
                    if (roomToWest != null)
                    {
                        AddBidirectionalExits(roomToWest, r, BidirectionalExitType.WestEast);
                    }
                }
                //look for a square to the south and add the north/south exits
                if (y > 0)
                {
                    Room roomToSouth = grid[x, y - 1];
                    if (roomToSouth != null)
                    {
                        AddBidirectionalExits(r, roomToSouth, BidirectionalExitType.NorthSouth);
                    }
                }
            }
        }

        /// <summary>
        /// adds rooms for mayor millwood's mansion
        /// </summary>
        /// <param name="oIxell">Ixell (entrance to mansion)</param>
        private void AddMayorMillwoodMansion(Room oIxell)
        {
            RoomGraph graphMillwoodMansion = _graphs[MapType.MillwoodMansion];

            Room oPathToMansion1 = AddRoom("Construction Site", "Construction Site");
            AddExit(oIxell, oPathToMansion1, "back");
            AddExit(oPathToMansion1, oIxell, "hoist");
            graphMillwoodMansion.Rooms[oPathToMansion1] = new System.Windows.Point(1, 1);

            Room oPathToMansion2 = AddRoom("Southern View", "Southern View");
            AddBidirectionalExits(oPathToMansion1, oPathToMansion2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion2] = new System.Windows.Point(1, 2);

            Room oPathToMansion3 = AddRoom("The South Wall", "The South Wall");
            AddBidirectionalExits(oPathToMansion2, oPathToMansion3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion3] = new System.Windows.Point(1, 3);

            Room oPathToMansion4WarriorBardsx2 = AddRoom("Warrior Bards (Path)", "Stone Path");
            oPathToMansion4WarriorBardsx2.AddPermanentMobs(MobTypeEnum.WarriorBard, MobTypeEnum.WarriorBard);
            AddExit(oPathToMansion3, oPathToMansion4WarriorBardsx2, "stone");
            AddExit(oPathToMansion4WarriorBardsx2, oPathToMansion3, "north");
            graphMillwoodMansion.Rooms[oPathToMansion4WarriorBardsx2] = new System.Windows.Point(1, 4);

            Room oPathToMansion5 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion4WarriorBardsx2, oPathToMansion5, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oPathToMansion5] = new System.Windows.Point(0, 5);

            Room oPathToMansion6 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion5, oPathToMansion6, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion6] = new System.Windows.Point(0, 6);

            Room oPathToMansion7 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion6, oPathToMansion7, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oPathToMansion7] = new System.Windows.Point(1, 7);

            Room oPathToMansion8 = AddRoom("Red Oak Tree", "Red Oak Tree");
            AddBidirectionalExits(oPathToMansion7, oPathToMansion8, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion8] = new System.Windows.Point(1, 8);

            Room oPathToMansion9 = AddRoom("Red Oak Tree", "Red Oak Tree");
            AddBidirectionalExits(oPathToMansion8, oPathToMansion9, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oPathToMansion9] = new System.Windows.Point(2, 9);

            Room oPathToMansion10 = AddRoom("Red Oak Tree", "Red Oak Tree");
            AddBidirectionalExits(oPathToMansion9, oPathToMansion10, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oPathToMansion10] = new System.Windows.Point(1, 10);

            Room oPathToMansion11 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion10, oPathToMansion11, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion11] = new System.Windows.Point(1, 11);

            Room oPathToMansion12 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion11, oPathToMansion12, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oPathToMansion12] = new System.Windows.Point(2, 11);

            Room oGrandPorch = AddRoom("Warrior Bard (Porch)", "Grand Porch");
            oGrandPorch.AddPermanentMobs(MobTypeEnum.WarriorBard);
            AddExit(oPathToMansion12, oGrandPorch, "porch");
            AddExit(oGrandPorch, oPathToMansion12, "path");
            graphMillwoodMansion.Rooms[oGrandPorch] = new System.Windows.Point(3, 11);

            Room oMansionInside1 = AddRoom("Mansion Inside", "Main Hallway");
            AddBidirectionalSameNameExit(oGrandPorch, oMansionInside1, "door", true);
            graphMillwoodMansion.Rooms[oMansionInside1] = new System.Windows.Point(4, 11);

            Room oMansionInside2 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionInside1, oMansionInside2, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionInside2] = new System.Windows.Point(5, 11);

            Room oMansionFirstFloorToNorthStairwell1 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell1, oMansionInside2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell1] = new System.Windows.Point(5, 10);

            Room oMansionFirstFloorToNorthStairwell2 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell2, oMansionFirstFloorToNorthStairwell1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell2] = new System.Windows.Point(5, 9);

            Room oMansionFirstFloorToNorthStairwell3 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell3, oMansionFirstFloorToNorthStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell3] = new System.Windows.Point(5, 8);

            Room oMansionFirstFloorToNorthStairwell4 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell4] = new System.Windows.Point(5, 7);

            Room oMansionFirstFloorToNorthStairwell5 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell5] = new System.Windows.Point(6, 7);

            Room oWarriorBardMansionNorth = AddRoom("Warrior Bard Mansion N", "Northern Stairwell");
            oWarriorBardMansionNorth.AddPermanentMobs(MobTypeEnum.WarriorBard);
            AddBidirectionalExits(oWarriorBardMansionNorth, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oWarriorBardMansionNorth] = new System.Windows.Point(6, 6);

            Room oMansionFirstFloorToSouthStairwell1 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToSouthStairwell1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell1] = new System.Windows.Point(5, 12);

            Room oMansionFirstFloorToSouthStairwell2 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell1, oMansionFirstFloorToSouthStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell2] = new System.Windows.Point(5, 13);

            Room oMansionFirstFloorToSouthStairwell3 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell2, oMansionFirstFloorToSouthStairwell3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell3] = new System.Windows.Point(5, 14);

            Room oMansionFirstFloorToSouthStairwell4 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell3, oMansionFirstFloorToSouthStairwell4, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell4] = new System.Windows.Point(5, 15);

            Room oMansionFirstFloorToSouthStairwell5 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell4, oMansionFirstFloorToSouthStairwell5, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell5] = new System.Windows.Point(6, 15);

            Room oWarriorBardMansionSouth = AddRoom("Warrior Bard Mansion S", "Southern Stairwell");
            oWarriorBardMansionSouth.AddPermanentMobs(MobTypeEnum.WarriorBard);
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell5, oWarriorBardMansionSouth, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oWarriorBardMansionSouth] = new System.Windows.Point(6, 16);

            Room oMansionFirstFloorToEastStairwell1 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToEastStairwell1, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell1] = new System.Windows.Point(6, 11);

            Room oMansionFirstFloorToEastStairwell2 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell1, oMansionFirstFloorToEastStairwell2, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell2] = new System.Windows.Point(7, 11);

            Room oMansionFirstFloorToEastStairwell3 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell2, oMansionFirstFloorToEastStairwell3, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell3] = new System.Windows.Point(8, 11);

            Room oMansionFirstFloorToEastStairwell4 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell3, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell4] = new System.Windows.Point(9, 11);

            Room oMansionFirstFloorToEastStairwell5 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell5] = new System.Windows.Point(10, 10);

            Room oMansionFirstFloorToEastStairwell6 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell6] = new System.Windows.Point(11, 11);

            Room oMansionFirstFloorToEastStairwell7 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell4, oMansionFirstFloorToEastStairwell7, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell6, oMansionFirstFloorToEastStairwell7, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell7] = new System.Windows.Point(10, 12);

            Room oWarriorBardMansionEast = AddRoom("Warrior Bard Mansion E", "Grand Staircase");
            oWarriorBardMansionEast.AddPermanentMobs(MobTypeEnum.WarriorBard);
            AddBidirectionalExits(oWarriorBardMansionEast, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oWarriorBardMansionEast] = new System.Windows.Point(10, 11);

            Room oNorthHallway1 = AddRoom("North Hallway", "North Hallway");
            AddBidirectionalExits(oNorthHallway1, oMansionFirstFloorToEastStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway1] = new System.Windows.Point(7, 10);

            Room oNorthHallway2 = AddRoom("North Hallway", "North Hallway");
            AddBidirectionalExits(oNorthHallway2, oNorthHallway1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway2] = new System.Windows.Point(7, 9);

            Room oNorthHallway3 = AddRoom("North Hallway", "North Hallway");
            AddBidirectionalExits(oNorthHallway3, oNorthHallway2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway3] = new System.Windows.Point(7, 8);

            Room oDungeonGuardNorth = AddRoom("Dungeon Guard", "North Hallway");
            oDungeonGuardNorth.AddPermanentMobs(MobTypeEnum.DungeonGuard);
            AddBidirectionalExits(oNorthHallway3, oDungeonGuardNorth, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oDungeonGuardNorth] = new System.Windows.Point(8, 8);

            Room oSouthHallway1 = AddRoom("South Hallway", "South Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell2, oSouthHallway1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway1] = new System.Windows.Point(7, 12);

            Room oSouthHallway2 = AddRoom("South Hallway", "South Hallway");
            AddBidirectionalExits(oSouthHallway1, oSouthHallway2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway2] = new System.Windows.Point(7, 13);

            Room oSouthHallway3 = AddRoom("South Hallway", "South Hallway");
            AddBidirectionalExits(oSouthHallway2, oSouthHallway3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway3] = new System.Windows.Point(7, 14);

            Room oDungeonGuardSouth = AddRoom("Dungeon Guard", "South Hallway");
            oDungeonGuardSouth.AddPermanentMobs(MobTypeEnum.DungeonGuard);
            AddBidirectionalExits(oSouthHallway3, oDungeonGuardSouth, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oDungeonGuardSouth] = new System.Windows.Point(8, 14);

            AddMillwoodMansionUpstairs(oWarriorBardMansionNorth, oWarriorBardMansionSouth, oWarriorBardMansionEast);
        }

        private void AddMillwoodMansionUpstairs(Room northStairwell, Room southStairwell, Room eastStairwell)
        {
            RoomGraph millwoodMansionUpstairsGraph = new RoomGraph("Millwood Mansion Upstairs");
            millwoodMansionUpstairsGraph.ScalingFactor = 100;
            _graphs[MapType.MillwoodMansionUpstairs] = millwoodMansionUpstairsGraph;
            millwoodMansionUpstairsGraph.Rooms[northStairwell] = new System.Windows.Point(1, 0);
            millwoodMansionUpstairsGraph.Rooms[southStairwell] = new System.Windows.Point(1, 12);
            millwoodMansionUpstairsGraph.Rooms[eastStairwell] = new System.Windows.Point(5, 5);

            Room oGrandStaircaseUpstairs = AddRoom("Grand Staircase", "Grand Staircase");
            AddBidirectionalExits(oGrandStaircaseUpstairs, eastStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oGrandStaircaseUpstairs] = new System.Windows.Point(5, 6);

            Room oRoyalHallwayUpstairs = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oGrandStaircaseUpstairs, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayUpstairs] = new System.Windows.Point(4, 6);

            Room oRoyalHallwayToMayor = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oRoyalHallwayToMayor, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayToMayor] = new System.Windows.Point(4, 7);

            Room oRoyalHallwayToChancellor = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayToChancellor, oRoyalHallwayUpstairs, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayToChancellor] = new System.Windows.Point(4, 5);

            Room oRoyalHallway1 = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallway1, oRoyalHallwayUpstairs, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway1] = new System.Windows.Point(3, 6);

            Room oRoyalHallway2 = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallway2, oRoyalHallway1, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway2] = new System.Windows.Point(2, 6);

            Room oRoyalHallway3 = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallway3, oRoyalHallway2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway3] = new System.Windows.Point(1, 6);

            Room oNorthCorridor1 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor1, oRoyalHallway3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor1] = new System.Windows.Point(1, 5);

            Room oNorthCorridor2 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor2, oNorthCorridor1, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor2] = new System.Windows.Point(1, 4);

            Room oDiningArea = AddRoom("Dining Area", "Dining Area");
            AddBidirectionalExits(oDiningArea, oNorthCorridor2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oDiningArea] = new System.Windows.Point(0, 4);

            Room oNorthCorridor3 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor3, oNorthCorridor2, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor3] = new System.Windows.Point(1, 3);

            Room oNorthCorridor4 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor4, oNorthCorridor3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor4] = new System.Windows.Point(1, 2);

            Room oMeditationChamber = AddHealingRoom("Meditation Chamber", "Meditation Chamber", HealingRoom.MillwoodMansion);
            Exit e = AddExit(oNorthCorridor4, oMeditationChamber, "door");
            e.MustOpen = true;
            AddExit(oMeditationChamber, oNorthCorridor4, "out");
            millwoodMansionUpstairsGraph.Rooms[oMeditationChamber] = new System.Windows.Point(0, 2);

            Room oNorthernStairwell = AddRoom("Northern Stairwell", "Northern Stairwell");
            AddBidirectionalExits(oNorthernStairwell, oNorthCorridor4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthernStairwell, northStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oNorthernStairwell] = new System.Windows.Point(1, 1);

            Room oSouthCorridor1 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oRoyalHallway3, oSouthCorridor1, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor1] = new System.Windows.Point(1, 7);

            Room oSouthCorridor2 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oSouthCorridor1, oSouthCorridor2, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor2] = new System.Windows.Point(1, 8);

            Room oKnightsQuarters = AddRoom("Knights' Quarters", "Knights' Quarters");
            AddBidirectionalExits(oKnightsQuarters, oSouthCorridor2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oKnightsQuarters] = new System.Windows.Point(0, 8);

            Room oSouthCorridor3 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oSouthCorridor2, oSouthCorridor3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor3] = new System.Windows.Point(1, 9);

            Room oSouthCorridor4 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oSouthCorridor3, oSouthCorridor4, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor4] = new System.Windows.Point(1, 10);

            Room oStorageRoom = AddRoom("Storage Room", "Storage Room");
            e = AddExit(oSouthCorridor4, oStorageRoom, "door");
            e.MustOpen = true;
            AddExit(oStorageRoom, oSouthCorridor4, "out");
            millwoodMansionUpstairsGraph.Rooms[oStorageRoom] = new System.Windows.Point(0, 10);

            Room oSouthernStairwell = AddRoom("Southern Stairwell", "Southern Stairwell");
            AddBidirectionalExits(oSouthCorridor4, oSouthernStairwell, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSouthernStairwell, southStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oSouthernStairwell] = new System.Windows.Point(1, 11);

            Room oMayorMillwood = AddRoom("Mayor Millwood", "Royal Chamber");
            oMayorMillwood.AddPermanentMobs(MobTypeEnum.MayorMillwood);
            e = AddExit(oRoyalHallwayToMayor, oMayorMillwood, "chamber");
            e.MustOpen = true;
            AddExit(oMayorMillwood, oRoyalHallwayToMayor, "out");
            millwoodMansionUpstairsGraph.Rooms[oMayorMillwood] = new System.Windows.Point(4, 8);

            Room oChancellorOfProtection = AddRoom("Chancellor of Protection", "The Chancellor of Protection's Chambers");
            oChancellorOfProtection.AddPermanentMobs(MobTypeEnum.ChancellorOfProtection);
            e = AddExit(oRoyalHallwayToChancellor, oChancellorOfProtection, "chamber");
            e.MustOpen = true;
            AddExit(oChancellorOfProtection, oRoyalHallwayToChancellor, "out");
            millwoodMansionUpstairsGraph.Rooms[oChancellorOfProtection] = new System.Windows.Point(4, 4);

            AddLocation(_aBreePerms, oChancellorOfProtection);
            AddLocation(_aBreePerms, oMayorMillwood);
        }

        private void AddBreeToImladris(out Room oOuthouse, Room breeEastGateInside, Room breeEastGateOutside, out Room imladrisWestGateOutside, Room oCemetery)
        {
            RoomGraph breeToImladrisGraph = new RoomGraph("Bree/Imladris");
            breeToImladrisGraph.ScalingFactor = 100;
            _graphs[MapType.BreeToImladris] = breeToImladrisGraph;

            breeToImladrisGraph.Rooms[breeEastGateInside] = new System.Windows.Point(2, 4);
            breeToImladrisGraph.Rooms[oCemetery] = new System.Windows.Point(2, 3);

            AddExit(breeEastGateInside, breeEastGateOutside, "gate");
            breeToImladrisGraph.Rooms[breeEastGateOutside] = new System.Windows.Point(3, 4);
            Exit e = AddExit(breeEastGateOutside, breeEastGateInside, "gate");
            e.RequiresDay = true;

            Room oGreatEastRoad1 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(breeEastGateOutside, oGreatEastRoad1, BidirectionalExitType.WestEast);
            AddToFarmHouseAndUglies(oGreatEastRoad1, out oOuthouse, breeToImladrisGraph);
            breeToImladrisGraph.Rooms[oGreatEastRoad1] = new System.Windows.Point(4, 4);

            Room oGreatEastRoad2 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad1, oGreatEastRoad2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad2] = new System.Windows.Point(5, 4);

            Room oGreatEastRoad3 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad2, oGreatEastRoad3, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad3] = new System.Windows.Point(6, 4);

            AddGalbasiDowns(oGreatEastRoad2, oGreatEastRoad3, breeToImladrisGraph);

            Room oGreatEastRoad4 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad3, oGreatEastRoad4, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad4] = new System.Windows.Point(7, 4);

            Room oGreatEastRoad5 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad4, oGreatEastRoad5, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad5] = new System.Windows.Point(8, 4);

            Room oGreatEastRoad6 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad5, oGreatEastRoad6, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad6] = new System.Windows.Point(9, 4);

            Room oGreatEastRoadGoblinAmbushGobLrgLrg = AddRoom("Gob Ambush #1", "Great East Road");
            oGreatEastRoadGoblinAmbushGobLrgLrg.AddPermanentMobs(MobTypeEnum.Goblin, MobTypeEnum.LargeGoblin, MobTypeEnum.LargeGoblin);
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad6, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oGreatEastRoadGoblinAmbushGobLrgLrg] = new System.Windows.Point(10, 3);

            Room oGreatEastRoad8 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad8, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oGreatEastRoad8] = new System.Windows.Point(11, 4);

            Room oGreatEastRoad9 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad8, oGreatEastRoad9, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad9] = new System.Windows.Point(12, 4);

            Room oGreatEastRoad10 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad9, oGreatEastRoad10, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad10] = new System.Windows.Point(13, 4);

            Room oGreatEastRoad11 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad10, oGreatEastRoad11, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad11] = new System.Windows.Point(14, 4);

            Room oGreatEastRoad12 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad11, oGreatEastRoad12, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad12] = new System.Windows.Point(15, 4);

            Room oGreatEastRoad13 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad12, oGreatEastRoad13, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad13] = new System.Windows.Point(16, 4);

            Room oGreatEastRoad14 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad13, oGreatEastRoad14, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad14] = new System.Windows.Point(17, 4);

            imladrisWestGateOutside = AddRoom("West Gate Outside", "West Gate of Imladris");
            AddBidirectionalExits(oGreatEastRoad14, imladrisWestGateOutside, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[imladrisWestGateOutside] = new System.Windows.Point(18, 4);

            Room oNorthBrethilForest1 = AddRoom("North Brethil Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oGreatEastRoadGoblinAmbushGobLrgLrg, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest1] = new System.Windows.Point(10, 2);

            Room oNorthBrethilForest2 = AddRoom("North Brethil Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oNorthBrethilForest2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest2] = new System.Windows.Point(11, 2);

            Room oDarkFootpath = AddRoom("Dark Footpath", "Dark Footpath");
            AddBidirectionalExits(oDarkFootpath, oGreatEastRoad10, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthBrethilForest2, oDarkFootpath, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDarkFootpath] = new System.Windows.Point(12, 2);

            Room oNorthBrethilForest3 = AddRoom("Brethil Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest3, oDarkFootpath, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest3] = new System.Windows.Point(12, 1);

            Room oNorthBrethilForest4 = AddRoom("Brethil Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest4, oNorthBrethilForest3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest4] = new System.Windows.Point(12, 0);

            Room oNorthBrethilForest5GobAmbush = AddRoom("Gob Ambush #2", "North Brethil Forest");
            oNorthBrethilForest5GobAmbush.AddPermanentMobs(MobTypeEnum.GoblinWarrior, MobTypeEnum.GoblinWarrior, MobTypeEnum.LargeGoblin);
            AddBidirectionalExits(oNorthBrethilForest4, oNorthBrethilForest5GobAmbush, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest5GobAmbush] = new System.Windows.Point(13, 0);

            //South Brethil Forest
            Room oDeepForest = AddRoom("Deep Forest", "Deep Forest");
            AddBidirectionalExits(oGreatEastRoad9, oDeepForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oDeepForest] = new System.Windows.Point(12, 5);

            Room oBrethilForest = AddRoom("Brethil Forest", "Brethil Forest");
            AddBidirectionalExits(oDeepForest, oBrethilForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oBrethilForest] = new System.Windows.Point(12, 6);

            Room oSpriteGuards = AddRoom("Sprite Guards", "Brethil Forest");
            oSpriteGuards.AddPermanentMobs(MobTypeEnum.SpriteGuard, MobTypeEnum.SpriteGuard);
            e = AddExit(oBrethilForest, oSpriteGuards, "brush");
            e.Hidden = true;
            AddExit(oSpriteGuards, oBrethilForest, "east");
            breeToImladrisGraph.Rooms[oSpriteGuards] = new System.Windows.Point(11, 6);

            AddLocation(_aBreePerms, oGreatEastRoadGoblinAmbushGobLrgLrg);
            AddLocation(_aBreePerms, oNorthBrethilForest5GobAmbush);
            AddLocation(_aBreePerms, oSpriteGuards);
        }

        private void AddToFarmHouseAndUglies(Room oGreatEastRoad1, out Room oOuthouse, RoomGraph breeToImladrisGraph)
        {
            Room oRoadToFarm1 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oGreatEastRoad1, oRoadToFarm1, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm1] = new System.Windows.Point(4, 5);

            Room oRoadToFarm2 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oRoadToFarm1, oRoadToFarm2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm2] = new System.Windows.Point(4, 6);

            Room oRoadToFarm3 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oRoadToFarm2, oRoadToFarm3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm3] = new System.Windows.Point(4, 7);

            Room oRoadToFarm4 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oRoadToFarm3, oRoadToFarm4, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm4] = new System.Windows.Point(4, 8);

            Room oRoadToFarm5 = AddRoom("Ranch House Path", "Path to Ranch House");
            AddBidirectionalExits(oRoadToFarm5, oRoadToFarm4, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oRoadToFarm5] = new System.Windows.Point(3, 8);

            Room oRoadToFarm6 = AddRoom("Front Steps", "Ranch House Front Steps");
            AddBidirectionalExits(oRoadToFarm6, oRoadToFarm5, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oRoadToFarm6] = new System.Windows.Point(2, 8);

            oOuthouse = AddRoom("Outhouse", "Outhouse");
            AddBidirectionalExits(oRoadToFarm4, oOuthouse, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oOuthouse] = new System.Windows.Point(5, 8);

            Room oSwimmingPond = AddRoom("Swimming Pond", "Swimming Pond");
            AddExit(oOuthouse, oSwimmingPond, "pond");
            AddExit(oSwimmingPond, oOuthouse, "west");
            breeToImladrisGraph.Rooms[oSwimmingPond] = new System.Windows.Point(6, 8);

            Room oPond = AddRoom("Pond", "Pond");
            oPond.AddPermanentMobs(MobTypeEnum.WaterTurtle);
            AddBidirectionalExitsWithOut(oSwimmingPond, oPond, "pond");
            breeToImladrisGraph.Rooms[oPond] = new System.Windows.Point(6, 7.5);

            Room oMuddyPath = AddRoom("Muddy Path", "Muddy Path");
            Exit e = AddExit(oSwimmingPond, oMuddyPath, "path");
            e.Hidden = true;
            AddExit(oMuddyPath, oSwimmingPond, "pond");
            breeToImladrisGraph.Rooms[oMuddyPath] = new System.Windows.Point(7, 8);

            Room oSmallPlayground = AddRoom("Playground", "Small Playground");
            AddBidirectionalExits(oSmallPlayground, oMuddyPath, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oSmallPlayground] = new System.Windows.Point(7.5, 7.5);

            Room oUglyKidSchoolEntrance = AddRoom("School Entrance", "Ugly Kid School Entrance");
            AddBidirectionalSameNameExit(oSmallPlayground, oUglyKidSchoolEntrance, "gate");
            breeToImladrisGraph.Rooms[oUglyKidSchoolEntrance] = new System.Windows.Point(8.5, 7.5);

            Room oMuddyFoyer = AddRoom("Muddy Foyer", "Muddy Foyer");
            e = AddExit(oUglyKidSchoolEntrance, oMuddyFoyer, "front");
            e.MaximumLevel = 10;
            AddExit(oMuddyFoyer, oUglyKidSchoolEntrance, "out");
            breeToImladrisGraph.Rooms[oMuddyFoyer] = new System.Windows.Point(8.5, 7.25);

            Room oUglyKidClassroomK7 = AddRoom("Classroom K-7", "Ugly Kid Classroom K-7");
            AddExit(oMuddyFoyer, oUglyKidClassroomK7, "classroom");
            AddExit(oUglyKidClassroomK7, oMuddyFoyer, "foyer");
            breeToImladrisGraph.Rooms[oUglyKidClassroomK7] = new System.Windows.Point(8.5, 7);

            Room oHallway = AddRoom("Hallway", "Hallway");
            AddExit(oUglyKidClassroomK7, oHallway, "hallway");
            AddExit(oHallway, oUglyKidClassroomK7, "classroom");
            breeToImladrisGraph.Rooms[oHallway] = new System.Windows.Point(8.5, 6.75);

            Room oHallwayEnd = AddRoom("Hallway End", "Hallway End");
            AddBidirectionalExits(oHallwayEnd, oHallway, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oHallwayEnd] = new System.Windows.Point(8.5, 6.5);

            Room oRoadToFarm7HoundDog = AddRoom("Hound Dog", "Front Porch");
            oRoadToFarm7HoundDog.AddPermanentMobs(MobTypeEnum.HoundDog);
            AddBidirectionalExitsWithOut(oRoadToFarm6, oRoadToFarm7HoundDog, "porch");
            breeToImladrisGraph.Rooms[oRoadToFarm7HoundDog] = new System.Windows.Point(2, 7);

            Room oFarmParlorManagerMulloyThreshold = AddRoom("Farm Parlor", "Parlor");
            AddBidirectionalSameNameExit(oFarmParlorManagerMulloyThreshold, oRoadToFarm7HoundDog, "door", true);
            breeToImladrisGraph.Rooms[oFarmParlorManagerMulloyThreshold] = new System.Windows.Point(2, 6);

            Room oManagerMulloy = AddRoom("Manager Mulloy", "Study");
            oManagerMulloy.AddPermanentMobs(MobTypeEnum.ManagerMulloy);
            AddBidirectionalExitsWithOut(oFarmParlorManagerMulloyThreshold, oManagerMulloy, "study");
            breeToImladrisGraph.Rooms[oManagerMulloy] = new System.Windows.Point(2, 5);

            Room oFarmKitchen = AddRoom("Kitchen", "Study");
            AddExit(oFarmParlorManagerMulloyThreshold, oFarmKitchen, "kitchen");
            AddExit(oFarmKitchen, oFarmParlorManagerMulloyThreshold, "parlor");
            breeToImladrisGraph.Rooms[oFarmKitchen] = new System.Windows.Point(1, 5);

            Room oFarmBackPorch = AddRoom("Back Porch", "Back Porch");
            AddExit(oFarmKitchen, oFarmBackPorch, "backdoor");
            AddExit(oFarmBackPorch, oFarmKitchen, "kitchen");
            breeToImladrisGraph.Rooms[oFarmBackPorch] = new System.Windows.Point(1, 6);

            Room oFarmCat = AddRoom("Farm Cat", "The Woodshed");
            oFarmCat.AddPermanentMobs(MobTypeEnum.FarmCat);
            AddExit(oFarmBackPorch, oFarmCat, "woodshed");
            e = AddExit(oFarmCat, oFarmBackPorch, "out");
            e.NoFlee = true;
            breeToImladrisGraph.Rooms[oFarmCat] = new System.Windows.Point(1, 7);

            Room oCrabbe = AddRoom("Crabbe", "Detention Room");
            oCrabbe.AddPermanentMobs(MobTypeEnum.CrabbeTheClassBully);
            AddBidirectionalExitsWithOut(oHallway, oCrabbe, "detention");
            breeToImladrisGraph.Rooms[oCrabbe] = new System.Windows.Point(7.5, 6.75);

            Room oMrWartnose = AddRoom("Mr. Wartnose", "Mr. Wartnose's Office");
            oMrWartnose.AddPermanentMobs(MobTypeEnum.MrWartnose);
            AddBidirectionalExitsWithOut(oUglyKidClassroomK7, oMrWartnose, "office");
            breeToImladrisGraph.Rooms[oMrWartnose] = new System.Windows.Point(7.5, 7);

            AddLocation(_aBreePerms, oRoadToFarm7HoundDog);
            AddLocation(_aBreePerms, oManagerMulloy);
            AddLocation(_aBreePerms, oFarmCat);
        }

        private void AddGalbasiDowns(Room oGreatEastRoad2, Room oGreatEastRoad3, RoomGraph breeToImladrisGraph)
        {
            Room oGalbasiDownsEntrance = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsEntrance, oGreatEastRoad2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGalbasiDownsEntrance] = new System.Windows.Point(5, 3.5);

            Room oGalbasiDownsSoutheast = AddRoom("Galbasi Downs", "Galbasi Downs");
            Exit e = AddExit(oGreatEastRoad3, oGalbasiDownsSoutheast, "north");
            e.Hidden = true;
            AddExit(oGalbasiDownsSoutheast, oGreatEastRoad3, "south");
            breeToImladrisGraph.Rooms[oGalbasiDownsSoutheast] = new System.Windows.Point(6, 3.5);

            Room oGalbasiDownsSouthwest = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsSouthwest, oGalbasiDownsEntrance, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGalbasiDownsSouthwest] = new System.Windows.Point(4, 3.5);

            Room oGalbasiDownsNorth = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsEntrance, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsSoutheast, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsSouthwest, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oGalbasiDownsNorth] = new System.Windows.Point(5, 3);

            Room oGalbasiDownsNorthwest = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorthwest, oGalbasiDownsSouthwest, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGalbasiDownsNorthwest, oGalbasiDownsNorth, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGalbasiDownsNorthwest] = new System.Windows.Point(4, 3);

            Room oGalbasiDownsNortheast = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsNortheast, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGalbasiDownsNortheast] = new System.Windows.Point(6, 3);

            Room oGalbasiDownsFurthestNorth = AddRoom("Galbasi Downs End", "Galbasi Downs");
            AddExit(oGalbasiDownsFurthestNorth, oGalbasiDownsNortheast, "southeast");
            e = AddExit(oGalbasiDownsNortheast, oGalbasiDownsFurthestNorth, "northwest");
            e.Hidden = true;
            AddExit(oGalbasiDownsFurthestNorth, oGalbasiDownsNorthwest, "southwest");
            e = AddExit(oGalbasiDownsNorthwest, oGalbasiDownsFurthestNorth, "northeast");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oGalbasiDownsFurthestNorth] = new System.Windows.Point(5, 2.5);

            Room oTopOfHill = AddRoom("Hilltop", "Top of Hill");
            e = AddExit(oGalbasiDownsFurthestNorth, oTopOfHill, "hill");
            e.Hidden = true;
            AddExit(oTopOfHill, oGalbasiDownsFurthestNorth, "north");
            breeToImladrisGraph.Rooms[oTopOfHill] = new System.Windows.Point(5, 2.25);

            Room oBarrow = AddRoom("Barrow", "Barrow");
            e = AddExit(oTopOfHill, oBarrow, "niche");
            e.Hidden = true;
            //This always fails is this always the case or am I just using a too low dexterity/level character?
            //e = AddExit(oBarrow, oTopOfHill, "up");
            //e.IsTrapExit = true;
            breeToImladrisGraph.Rooms[oBarrow] = new System.Windows.Point(5, 2);

            Room oAntechamber = AddRoom("Antechamber DMG", "Antechamber");
            AddExit(oBarrow, oAntechamber, "altar");
            AddExit(oAntechamber, oBarrow, "up");
            oAntechamber.DamageType = RealmType.Fire;
            breeToImladrisGraph.Rooms[oAntechamber] = new System.Windows.Point(5, 1.75);

            Room oGalbasiHalls = AddRoom("Galbasi Halls", "Galbasi Halls");
            e = AddExit(oAntechamber, oGalbasiHalls, "stairway");
            e.Hidden = true;
            AddExit(oGalbasiHalls, oAntechamber, "stairway");
            breeToImladrisGraph.Rooms[oGalbasiHalls] = new System.Windows.Point(5, 1.5);

            Room oUnderHallsCorridorsGreenSlime = AddRoom("Green Slime", "Underhalls Corridors");
            //CSRTODO
            //oUnderHallsCorridorsGreenSlime.Mob1 = "slime";
            AddBidirectionalExits(oUnderHallsCorridorsGreenSlime, oGalbasiHalls, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderHallsCorridorsGreenSlime] = new System.Windows.Point(5, 1.25);

            Room oUnderHallsCorridorsBaseJunction = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderHallsCorridorsBaseJunction, oUnderHallsCorridorsGreenSlime, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderHallsCorridorsBaseJunction] = new System.Windows.Point(5, 1);

            Room oUnderhallsCorridorsWest = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsWest, oUnderHallsCorridorsBaseJunction, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsWest] = new System.Windows.Point(4, 1);

            Room oDarkCorner = AddRoom("Skeleton", "Dark Corner");
            AddBidirectionalExits(oDarkCorner, oUnderhallsCorridorsWest, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDarkCorner] = new System.Windows.Point(3, 1);

            Room oToCave = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderHallsCorridorsBaseJunction, oToCave, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oToCave] = new System.Windows.Point(6, 1);

            Room oGlowingCave = AddHealingRoom("Glowing Cave", "Glowing Cave", HealingRoom.Underhalls);
            AddBidirectionalExitsWithOut(oToCave, oGlowingCave, "cave");
            breeToImladrisGraph.Rooms[oGlowingCave] = new System.Windows.Point(6, 2);

            Room oUnderhallsCorridorsEast = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oToCave, oUnderhallsCorridorsEast, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsEast] = new System.Windows.Point(7, 1);

            Room oUnderhallsCorridorsToQuarry = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsEast, oUnderhallsCorridorsToQuarry, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToQuarry] = new System.Windows.Point(8, 1);

            Room oUnderhallsCorridorsSoutheastCorner = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToQuarry, oUnderhallsCorridorsSoutheastCorner, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsSoutheastCorner] = new System.Windows.Point(9, 1);

            Room oUnderhallsCorridorsEast2 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsEast2, oUnderhallsCorridorsSoutheastCorner, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsEast2] = new System.Windows.Point(9, 0.5);

            Room oUnderhallsIronDoor = AddRoom("Iron Door", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsIronDoor, oUnderhallsCorridorsEast2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsIronDoor] = new System.Windows.Point(9, 0);

            Room oBlackEyeOrcDwelling = AddRoom("Sewer Orcs", "Black Eye Orc Dwelling");
            e = AddExit(oUnderhallsIronDoor, oBlackEyeOrcDwelling, "iron");
            e.MustOpen = true;
            e.KeyType = KeyType.UnknownKnockable;
            e.IsTrapExit = true;
            AddExit(oBlackEyeOrcDwelling, oUnderhallsIronDoor, "out");
            breeToImladrisGraph.Rooms[oBlackEyeOrcDwelling] = new System.Windows.Point(9, -0.5);

            Room oUnderhallsCorridorsNE = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsNE, oUnderHallsCorridorsBaseJunction, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsNE] = new System.Windows.Point(6, 0);

            Room oUnderhallsCorridorsN = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsN, oUnderhallsCorridorsNE, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsN] = new System.Windows.Point(6, -1);

            Room oOrcsQuarry = AddRoom("Orcs' Quarry", "Orcs' Quarry");
            AddBidirectionalExits(oUnderhallsCorridorsNE, oOrcsQuarry, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oOrcsQuarry] = new System.Windows.Point(8, 0);

            Room oOrcsQuarry2 = AddRoom("Orc Guard", "Orcs' Quarry");
            AddBidirectionalExits(oOrcsQuarry, oOrcsQuarry2, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oUnderhallsCorridorsToQuarry, oOrcsQuarry2, BidirectionalExitType.UpDown);
            breeToImladrisGraph.Rooms[oOrcsQuarry2] = new System.Windows.Point(8, 0.5);
        }

        private void AddImladrisCity(out Room oImladrisSouthGateInside, out Room oEastGateOfImladrisOutside, Room imladrisWestGateOutside, out Room healingHand)
        {
            RoomGraph imladrisGraph = new RoomGraph("Imladris");
            imladrisGraph.ScalingFactor = 100;
            _graphs[MapType.Imladris] = imladrisGraph;

            Room imladrisWestGateInside = AddRoom("West Gate Inside", "West Gate of Imladris");
            AddExit(imladrisWestGateInside, imladrisWestGateOutside, "gate");
            Exit e = AddExit(imladrisWestGateOutside, imladrisWestGateInside, "gate");
            e.RequiresDay = true;
            imladrisGraph.Rooms[imladrisWestGateOutside] = new System.Windows.Point(-1, 5);
            imladrisGraph.Rooms[imladrisWestGateInside] = new System.Windows.Point(0, 5);

            Room oImladrisCircle1 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle1, imladrisWestGateInside, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle1] = new System.Windows.Point(5D / 3, 5 - (4D / 3));

            Room oImladrisCircle2 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle2, oImladrisCircle1, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle2] = new System.Windows.Point(10D / 3, 5 - (8D / 3));

            Room oImladrisCircle3 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle2, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle3] = new System.Windows.Point(5, 1);

            Room oImladrisCircle4 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle4, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle4] = new System.Windows.Point(5 + (4D / 3), 1 + (4D / 3));

            Room oImladrisCircle5 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle4, oImladrisCircle5, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle5] = new System.Windows.Point(5 + (8D / 3), 1 + (8D / 3));

            Room oImladrisMainStreet1 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(imladrisWestGateInside, oImladrisMainStreet1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet1] = new System.Windows.Point(1, 5);

            Room oImladrisMainStreet2 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet1, oImladrisMainStreet2, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet2] = new System.Windows.Point(2, 5);

            Room oImladrisMainStreet3 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet2, oImladrisMainStreet3, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet3] = new System.Windows.Point(3, 5);

            Room oImladrisMainStreet4 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet3, oImladrisMainStreet4, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet4] = new System.Windows.Point(4, 5);

            Room oImladrisMainStreet5 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet4, oImladrisMainStreet5, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet5] = new System.Windows.Point(5, 5);

            Room oImladrisAlley3 = AddRoom("Side Alley", "Side Alley North");
            AddBidirectionalExits(oImladrisMainStreet5, oImladrisAlley3, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisAlley3] = new System.Windows.Point(6, 5);

            Room oImladrisAlley4 = AddRoom("Side Alley", "Side Alley North");
            AddBidirectionalExits(oImladrisAlley3, oImladrisAlley4, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisAlley4] = new System.Windows.Point(6, 6);

            Room oImladrisAlley5 = AddRoom("Side Alley", "Side Alley South");
            AddBidirectionalExits(oImladrisAlley4, oImladrisAlley5, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisAlley5] = new System.Windows.Point(6, 7);

            Room oImladrisSmallAlley1 = AddRoom("Small Alley", "Small Alley");
            AddExit(oImladrisAlley3, oImladrisSmallAlley1, "alley");
            AddExit(oImladrisSmallAlley1, oImladrisAlley3, "south");
            imladrisGraph.Rooms[oImladrisSmallAlley1] = new System.Windows.Point(6, 4);

            Room oImladrisSmallAlley2 = AddRoom("Small Alley", "Small Alley");
            AddBidirectionalExits(oImladrisSmallAlley2, oImladrisSmallAlley1, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisSmallAlley2] = new System.Windows.Point(6, 3);

            Room oImladrisPawnShop = AddRoom("Sharkey's Pawn Shop", "Sharkey's Pawn Shoppe");
            AddBidirectionalSameNameExit(oImladrisPawnShop, oImladrisSmallAlley2, "door");
            imladrisGraph.Rooms[oImladrisPawnShop] = new System.Windows.Point(5, 3);

            Room oImladrisTownCircle = AddRoom("Town Circle", "Imladris Town Circle");
            AddBidirectionalExits(oImladrisAlley3, oImladrisTownCircle, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisTownCircle] = new System.Windows.Point(7, 5);

            Room oImladrisMainStreet6 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisTownCircle, oImladrisMainStreet6, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet6] = new System.Windows.Point(8, 5);

            Room oEastGateOfImladrisInside = AddRoom("East Gate Inside", "East Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle5, oEastGateOfImladrisInside, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisMainStreet6, oEastGateOfImladrisInside, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oEastGateOfImladrisInside] = new System.Windows.Point(9, 5);

            oEastGateOfImladrisOutside = AddRoom("East Gate Outside", "Gates of Imladris");
            e = AddExit(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, "gate");
            e.MinimumLevel = 3;
            AddExit(oEastGateOfImladrisOutside, oEastGateOfImladrisInside, "gate");
            imladrisGraph.Rooms[oEastGateOfImladrisOutside] = new System.Windows.Point(10, 5);

            Room oImladrisCircle6 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oEastGateOfImladrisInside, oImladrisCircle6, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle6] = new System.Windows.Point(9 - (4D / 3), 5 + (4D / 3));

            Room oImladrisCircle7 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle6, oImladrisCircle7, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle7] = new System.Windows.Point(9 - (8D / 3), 5 + (8D / 3));

            Room oImladrisCircle10 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(imladrisWestGateInside, oImladrisCircle10, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle10] = new System.Windows.Point(5 - (10D / 3), 9 - (8D / 3));

            Room oImladrisCircle9 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle10, oImladrisCircle9, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle9] = new System.Windows.Point(5 - (5D / 3), 9 - (4D / 3));

            Room oImladrisCircle8 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle9, oImladrisCircle8, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisCircle7, oImladrisCircle8, BidirectionalExitType.SouthwestNortheast);
            AddExit(oImladrisAlley5, oImladrisCircle8, "south");
            imladrisGraph.Rooms[oImladrisCircle8] = new System.Windows.Point(5, 9);

            Room oRearAlley = AddRoom("Rear Alley", "Rear Alley");
            AddBidirectionalExits(oImladrisCircle10, oRearAlley, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oRearAlley, oImladrisAlley5, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oRearAlley] = new System.Windows.Point(5, 7);

            Room oPoisonedDagger = AddRoom("Master Assassins", "The Poisoned Dagger");
            oPoisonedDagger.AddPermanentMobs(MobTypeEnum.MasterAssassin, MobTypeEnum.MasterAssassin);
            AddBidirectionalSameNameExit(oRearAlley, oPoisonedDagger, "door");
            imladrisGraph.Rooms[oPoisonedDagger] = new System.Windows.Point(5, 6.5);

            oImladrisSouthGateInside = AddRoom("South Gate Inside", "Southern Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle8, oImladrisSouthGateInside, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisSouthGateInside] = new System.Windows.Point(5, 10);

            Room oImladrisCityDump = AddRoom("City Dump", "The Imladris City Dump");
            AddBidirectionalExits(oImladrisCityDump, oImladrisCircle8, BidirectionalExitType.NorthSouth);
            e = AddExit(oImladrisCityDump, oRearAlley, "north");
            e.Hidden = true;
            imladrisGraph.Rooms[oImladrisCityDump] = new System.Windows.Point(5, 8);

            healingHand = AddHealingRoom("Healing Hand", "The Healing Hand", HealingRoom.Imladris);
            AddBidirectionalExits(healingHand, oImladrisMainStreet5, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[healingHand] = new System.Windows.Point(5, 4.5);

            Room oTyriesPriestSupplies = AddRoom("Tyrie's Priest Supplies", "Tyrie's Priest Supplies");
            AddBidirectionalExits(oImladrisMainStreet5, oTyriesPriestSupplies, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oTyriesPriestSupplies] = new System.Windows.Point(5, 5.5);

            AddLocation(_aImladrisTharbadPerms, oPoisonedDagger);
        }

        private void AddEastOfImladris(Room oEastGateOfImladrisOutside, out Room westGateOfEsgaroth)
        {
            RoomGraph eastOfImladrisGraph = new RoomGraph("East of Imladris");
            eastOfImladrisGraph.ScalingFactor = 100;
            _graphs[MapType.EastOfImladris] = eastOfImladrisGraph;

            eastOfImladrisGraph.Rooms[oEastGateOfImladrisOutside] = new System.Windows.Point(0, 6);

            Room oMountainPath1 = AddRoom("Mountain Path", "Mountain Path");
            AddBidirectionalExits(oEastGateOfImladrisOutside, oMountainPath1, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainPath1] = new System.Windows.Point(1, 6);

            Room oMountainPath2 = AddRoom("Mountain Path", "Mountain Path");
            AddBidirectionalExits(oMountainPath2, oMountainPath1, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPath2] = new System.Windows.Point(2, 5);

            Room oSouthernFootpath1 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oMountainPath2, oSouthernFootpath1, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oSouthernFootpath1] = new System.Windows.Point(3, 6);

            Room oSouthernFootpath2 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oSouthernFootpath1, oSouthernFootpath2, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oSouthernFootpath2] = new System.Windows.Point(3, 7);

            Room oSouthernFootpath3 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oSouthernFootpath2, oSouthernFootpath3, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oSouthernFootpath3] = new System.Windows.Point(4, 8);

            Room oSouthernFootpath4 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oSouthernFootpath3, oSouthernFootpath4, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oSouthernFootpath4] = new System.Windows.Point(5, 9);

            Room oGiantRockslide = AddRoom("Giant Rockslide", "Giant Rockslide");
            AddBidirectionalExits(oSouthernFootpath4, oGiantRockslide, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oGiantRockslide] = new System.Windows.Point(6, 9);
            //CSRTODO: trail (hidden

            Room oMountainTrail1 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail1, oMountainPath2, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oMountainTrail1] = new System.Windows.Point(2, 4);

            Room oIorlasThreshold = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oIorlasThreshold, oMountainTrail1, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oIorlasThreshold] = new System.Windows.Point(3, 3);

            Room oMountainTrailEastOfIorlas1 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oIorlasThreshold, oMountainTrailEastOfIorlas1, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas1] = new System.Windows.Point(4, 3);

            Room oMountainTrailEastOfIorlas2 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrailEastOfIorlas1, oMountainTrailEastOfIorlas2, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas2] = new System.Windows.Point(5, 3);

            Room oMountainTrailEastOfIorlas3 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrailEastOfIorlas2, oMountainTrailEastOfIorlas3, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas3] = new System.Windows.Point(6, 3);

            Room oMountainTrailEastOfIorlas4 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrailEastOfIorlas3, oMountainTrailEastOfIorlas4, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas4] = new System.Windows.Point(7, 4);

            Room oCarrockPlains = AddRoom("Carrock Plains", "Carrock Plains");
            AddBidirectionalExits(oMountainTrailEastOfIorlas4, oCarrockPlains, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oCarrockPlains] = new System.Windows.Point(8, 4);

            westGateOfEsgaroth = AddRoom("West Gate Outside", "West Entrance to Esgaroth");
            AddBidirectionalExits(oCarrockPlains, westGateOfEsgaroth, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[westGateOfEsgaroth] = new System.Windows.Point(9, 4);

            Room oMountainTrail3 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail3, oIorlasThreshold, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainTrail3] = new System.Windows.Point(4, 2);

            Room oMountainPass1 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass1, oMountainTrail3, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPass1] = new System.Windows.Point(5, 1);

            Room oMountainPass2 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass2, oMountainPass1, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPass2] = new System.Windows.Point(6, 0);

            Room oMountainPass3 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass1, oMountainPass3, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oMountainPass3] = new System.Windows.Point(6, 2);

            Room oMountainPass4 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass2, oMountainPass4, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oMountainPass4, oMountainPass3, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPass4] = new System.Windows.Point(7, 1);
            //CSRTODO: down to ituk glacer (hidden)

            Room oLoftyTrail1 = AddRoom("Lofty Trail", "A Lofty Trail");
            AddBidirectionalExits(oLoftyTrail1, oMountainPass2, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oLoftyTrail1] = new System.Windows.Point(6, -0.5);

            Room oLoftyTrail2 = AddRoom("Lofty Trail", "A Lofty Trail");
            AddBidirectionalExits(oLoftyTrail2, oLoftyTrail1, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oLoftyTrail2] = new System.Windows.Point(6, -1);

            Room oLoftyTrail3 = AddRoom("Lofty Trail", "Lofty Trail");
            AddExit(oLoftyTrail2, oLoftyTrail3, "up");
            Exit e = AddExit(oLoftyTrail3, oLoftyTrail2, "down");
            e.IsTrapExit = true;
            eastOfImladrisGraph.Rooms[oLoftyTrail3] = new System.Windows.Point(6, -1.5);

            Room oLoftyTrail4 = AddRoom("Lofty Trail", "Lofty Trail");
            AddExit(oLoftyTrail3, oLoftyTrail4, "up");
            e = AddExit(oLoftyTrail4, oLoftyTrail3, "down");
            e.IsTrapExit = true;
            eastOfImladrisGraph.Rooms[oLoftyTrail4] = new System.Windows.Point(6, -2);

            Room oMountainDragon = AddRoom("Mountain Dragon", "Lofty Trail");
            oMountainDragon.AddPermanentMobs(MobTypeEnum.MountainDragon);
            AddExit(oLoftyTrail4, oMountainDragon, "up");
            AddExit(oMountainDragon, oLoftyTrail4, "down");
            eastOfImladrisGraph.Rooms[oMountainDragon] = new System.Windows.Point(6, -2.5);
            //CSRTODO: up

            Room oMountainTrail4 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail4, oMountainTrail3, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oMountainTrail4] = new System.Windows.Point(3, 1);

            Room oMistyMountainsCave = AddRoom("Cave", "Cave in the Misty Mountains");
            AddBidirectionalExitsWithOut(oMountainTrail4, oMistyMountainsCave, "cave");
            eastOfImladrisGraph.Rooms[oMistyMountainsCave] = new System.Windows.Point(2, 1);

            Room oMountainTrail5 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail5, oMountainTrail4, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oMountainTrail5] = new System.Windows.Point(3, 0);

            Room oMountainTrail6 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail6, oMountainTrail5, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oMountainTrail6] = new System.Windows.Point(3, -1);

            Room oLarsMagnusGrunwald = AddRoom("Lars Magnus Grunwald", "The Greywold Goldmine");
            oLarsMagnusGrunwald.AddPermanentMobs(MobTypeEnum.LarsMagnusGrunwald);
            AddBidirectionalSameNameExit(oMountainTrail6, oLarsMagnusGrunwald, "gate");
            eastOfImladrisGraph.Rooms[oLarsMagnusGrunwald] = new System.Windows.Point(3, -2);

            Room oIorlas = AddRoom("Iorlas", "Hermit's Shack");
            oIorlas.AddPermanentMobs(MobTypeEnum.IorlasTheHermit);
            AddExit(oIorlasThreshold, oIorlas, "shack");
            AddExit(oIorlas, oIorlasThreshold, "door");
            eastOfImladrisGraph.Rooms[oIorlas] = new System.Windows.Point(2, 3);

            AddLocation(_aImladrisTharbadPerms, oIorlas);
        }

        private void AddBreeToHobbiton(Room oBreeWestGateInside, Room oSmoulderingVillage)
        {
            RoomGraph westOfBreeMap = new RoomGraph("West of Bree");
            westOfBreeMap.ScalingFactor = 100;
            _graphs[MapType.WestOfBree] = westOfBreeMap;

            westOfBreeMap.Rooms[oBreeWestGateInside] = new System.Windows.Point(15, 0);

            Room oBreeWestGateOutside = AddRoom("West Gate Outside", "West Gate of Bree");
            AddBidirectionalSameNameExit(oBreeWestGateInside, oBreeWestGateOutside, "gate");
            _breeStreetsGraph.Rooms[oBreeWestGateOutside] = new System.Windows.Point(-1, 3);
            westOfBreeMap.Rooms[oBreeWestGateOutside] = new System.Windows.Point(14, 0);

            Room oGrandIntersection = AddRoom("Grand Intersection", "The Grand Intersection - Leviathan Way/North Fork Road/Western Road");
            AddBidirectionalExits(oGrandIntersection, oBreeWestGateOutside, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oGrandIntersection] = new System.Windows.Point(13, 0);

            Room oNorthFork1 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork1, oGrandIntersection, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oNorthFork1] = new System.Windows.Point(12, -1);

            Room oWesternRoad1 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad1, oGrandIntersection, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad1] = new System.Windows.Point(12, 0);

            Room oWesternRoad2 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad2, oWesternRoad1, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad2] = new System.Windows.Point(11, 0);

            Room oWesternRoad3 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad3, oWesternRoad2, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad3] = new System.Windows.Point(10, 0);

            Room oWesternRoad4 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad4, oWesternRoad3, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad4] = new System.Windows.Point(9, 0);

            Room oWesternRoad5 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad5, oWesternRoad4, BidirectionalExitType.WestEast);
            //CSRTODO: north
            westOfBreeMap.Rooms[oWesternRoad5] = new System.Windows.Point(8, 0);

            Room oWesternRoad6 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad6, oWesternRoad5, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad6] = new System.Windows.Point(7, 0);

            Room oWesternRoad7 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad7, oWesternRoad6, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad7] = new System.Windows.Point(6, 0);

            Room oWesternRoad8 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad8, oWesternRoad7, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad8] = new System.Windows.Point(5, 0);

            Room oWesternRoad9 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad9, oWesternRoad8, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad9] = new System.Windows.Point(4, 0);

            Room oVillageOfHobbiton1 = AddRoom("Village of Hobbiton", "The Village of Hobbiton");
            AddBidirectionalExits(oVillageOfHobbiton1, oWesternRoad9, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oVillageOfHobbiton1] = new System.Windows.Point(3, 0);

            Room oMainSquareOfHobbiton = AddRoom("Main Square of Hobbiton", "Main Square of Hobbiton");
            AddBidirectionalExits(oMainSquareOfHobbiton, oVillageOfHobbiton1, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oMainSquareOfHobbiton] = new System.Windows.Point(2, 0);

            Room oVillageOfHobbiton2 = AddRoom("Village of Hobbiton", "The Village of Hobbiton");
            Exit e = AddExit(oMainSquareOfHobbiton, oVillageOfHobbiton2, "south");
            e.MinimumLevel = 4;
            AddExit(oVillageOfHobbiton2, oMainSquareOfHobbiton, "north");
            westOfBreeMap.Rooms[oVillageOfHobbiton2] = new System.Windows.Point(2, 1);

            Room oValleyRoad = AddRoom("Valley Road", "Valley Road");
            AddBidirectionalExits(oVillageOfHobbiton2, oValleyRoad, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oValleyRoad] = new System.Windows.Point(2, 2);

            Room oHillAtBagEnd = AddRoom("Bag End Hill", "The Hill at Bag End");
            AddBidirectionalExits(oValleyRoad, oHillAtBagEnd, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oHillAtBagEnd] = new System.Windows.Point(3, 2);

            Room oBilboFrodoHobbitHoleCondo = AddRoom("Hobbit Hole Condo", "Bilbo and Frodo's Hobbit Hole Condo");
            AddBidirectionalExitsWithOut(oHillAtBagEnd, oBilboFrodoHobbitHoleCondo, "down");
            westOfBreeMap.Rooms[oBilboFrodoHobbitHoleCondo] = new System.Windows.Point(3, 2.25);

            Room oBilboFrodoCommonArea = AddRoom("Common Area", "Common Area");
            AddBidirectionalSameNameExit(oBilboFrodoHobbitHoleCondo, oBilboFrodoCommonArea, "curtain");
            westOfBreeMap.Rooms[oBilboFrodoCommonArea] = new System.Windows.Point(3, 2.5);

            Room oEastwingHallway = AddRoom("Eastwing Hallway", "Eastwing Hallway");
            AddExit(oBilboFrodoCommonArea, oEastwingHallway, "eastwing");
            AddExit(oEastwingHallway, oBilboFrodoCommonArea, "common");
            westOfBreeMap.Rooms[oEastwingHallway] = new System.Windows.Point(4, 2.5);

            Room oSouthwingHallway = AddRoom("Southwing Hallway", "Southwing Hallway");
            AddExit(oEastwingHallway, oSouthwingHallway, "southwing");
            AddExit(oSouthwingHallway, oEastwingHallway, "eastwing");
            westOfBreeMap.Rooms[oSouthwingHallway] = new System.Windows.Point(3, 2.75);

            Room oBilboBaggins = AddRoom("Bilbo Baggins", "Bilbo's Living Quarters");
            oBilboBaggins.AddPermanentMobs(MobTypeEnum.BilboBaggins);
            AddBidirectionalSameNameExit(oSouthwingHallway, oBilboBaggins, "oakdoor", true);
            westOfBreeMap.Rooms[oBilboBaggins] = new System.Windows.Point(3, 3);

            Room oFrodoBaggins = AddRoom("Frodo Baggins", "Frodo's Living Quarters");
            oFrodoBaggins.AddPermanentMobs(MobTypeEnum.FrodoBaggins);
            AddBidirectionalSameNameExit(oSouthwingHallway, oFrodoBaggins, "curtain");
            westOfBreeMap.Rooms[oFrodoBaggins] = new System.Windows.Point(4, 2.75);

            Room oGreatHallOfHeroes = AddRoom("Great Hall of Heroes", "The Great Hall of Heroes.");
            AddBidirectionalExitsWithOut(oGrandIntersection, oGreatHallOfHeroes, "hall");
            westOfBreeMap.Rooms[oGreatHallOfHeroes] = new System.Windows.Point(13, 0.5);

            //something is hasted
            Room oSomething = AddRoom("Something", "Kitchen");
            //CSRTODO
            //oSomething.Mob1 = "Something";
            //oSomething.Experience1 = 140;
            e = AddExit(oGreatHallOfHeroes, oSomething, "curtain");
            e.MaximumLevel = 10;
            e.Hidden = true;
            AddExit(oSomething, oGreatHallOfHeroes, "curtain");
            westOfBreeMap.Rooms[oSomething] = new System.Windows.Point(13, 1);

            Room oShepherd = AddRoom("Shepherd", "Pasture");
            oShepherd.AddPermanentMobs(MobTypeEnum.Shepherd);
            AddExit(oNorthFork1, oShepherd, "pasture");
            AddExit(oShepherd, oNorthFork1, "south");
            westOfBreeMap.Rooms[oShepherd] = new System.Windows.Point(13, -2);

            //Gate is locked (and knocking doesn't work) so not treating as an exit. This is only accessible from the other way around.
            //AddExit(oShepherd, oSmoulderingVillage, "gate");
            AddExit(oSmoulderingVillage, oShepherd, "gate");
            westOfBreeMap.Rooms[oSmoulderingVillage] = new System.Windows.Point(13, -2.5);

            AddLocation(_aBreePerms, oBilboBaggins);
            AddLocation(_aBreePerms, oFrodoBaggins);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside, out Room oTharbadGateOutside)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];

            RoomGraph imladrisToTharbadGraph = new RoomGraph("Imladris/Tharbad");
            imladrisToTharbadGraph.ScalingFactor = 100;
            _graphs[MapType.ImladrisToTharbad] = imladrisToTharbadGraph;

            Room oMistyTrail1 = AddRoom("South Gate Outside", "Misty Trail");
            AddBidirectionalSameNameExit(oImladrisSouthGateInside, oMistyTrail1, "gate");
            imladrisGraph.Rooms[oMistyTrail1] = new System.Windows.Point(5, 11);
            imladrisToTharbadGraph.Rooms[oMistyTrail1] = new System.Windows.Point(5, 0);

            Room oBrunskidTradersGuild1 = AddRoom("Brunskid Guild", "Brunskid Trader's Guild Store Front");
            AddBidirectionalExits(oBrunskidTradersGuild1, oMistyTrail1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oBrunskidTradersGuild1] = new System.Windows.Point(4, 11);
            imladrisToTharbadGraph.Rooms[oBrunskidTradersGuild1] = new System.Windows.Point(4, 0);

            Room oGuildmaster = AddRoom("Guildmaster", "Brunskid Trader's Guild Office");
            oGuildmaster.AddPermanentMobs(MobTypeEnum.Guildmaster);
            AddBidirectionalExits(oGuildmaster, oBrunskidTradersGuild1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oGuildmaster] = new System.Windows.Point(3, 11);
            imladrisToTharbadGraph.Rooms[oGuildmaster] = new System.Windows.Point(3, 0);

            Room oCutthroatAssassin = AddRoom("Hiester", "Brunskid Trader's Guild Acquisitions Room");
            AddBidirectionalExits(oCutthroatAssassin, oGuildmaster, BidirectionalExitType.WestEast);
            oCutthroatAssassin.AddPermanentMobs(MobTypeEnum.GregoryHiester, MobTypeEnum.MasterAssassin, MobTypeEnum.Cutthroat);
            imladrisGraph.Rooms[oCutthroatAssassin] = new System.Windows.Point(2, 11);
            imladrisToTharbadGraph.Rooms[oCutthroatAssassin] = new System.Windows.Point(2, 0);

            Room oMistyTrail2 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail1, oMistyTrail2, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail2] = new System.Windows.Point(5, 1);

            Room oMistyTrail3 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail2, oMistyTrail3, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail3] = new System.Windows.Point(5, 2);

            Room oMistyTrail4 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail3, oMistyTrail4, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail4] = new System.Windows.Point(4, 3);

            Room oPotionFactoryReception = AddRoom("Potion Factory Guard", "Reception Area of Potion Factory");
            oPotionFactoryReception.AddPermanentMobs(MobTypeEnum.Guard);
            AddBidirectionalExits(oPotionFactoryReception, oMistyTrail4, BidirectionalExitType.WestEast);
            imladrisToTharbadGraph.Rooms[oPotionFactoryReception] = new System.Windows.Point(3, 3);

            Room oPotionFactoryAdministrativeOffices = AddRoom("Potion Factory Administrative Offices", "Administrative Offices");
            AddBidirectionalExits(oPotionFactoryReception, oPotionFactoryAdministrativeOffices, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oPotionFactoryAdministrativeOffices] = new System.Windows.Point(3, 4);

            Room oMarkFrey = AddRoom("Mark Frey", "Potent Potions, Inc.");
            oMarkFrey.AddPermanentMobs(MobTypeEnum.MarkFrey);
            AddBidirectionalExitsWithOut(oPotionFactoryAdministrativeOffices, oMarkFrey, "door");
            imladrisToTharbadGraph.Rooms[oMarkFrey] = new System.Windows.Point(3, 5);

            Room oMistyTrail5 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail4, oMistyTrail5, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail5] = new System.Windows.Point(4, 4);

            Room oMistyTrail6 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail5, oMistyTrail6, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail6] = new System.Windows.Point(4, 5);

            Room oMistyTrail7 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail6, oMistyTrail7, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail7] = new System.Windows.Point(4, 6);

            Room oMistyTrail8 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail7, oMistyTrail8, BidirectionalExitType.NorthSouth);
            AddShantyTown(oMistyTrail8);
            imladrisToTharbadGraph.Rooms[oMistyTrail8] = new System.Windows.Point(4, 7);

            Room oMistyTrail9 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail8, oMistyTrail9, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail9] = new System.Windows.Point(4, 8);

            Room oMistyTrail10 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail9, oMistyTrail10, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail10] = new System.Windows.Point(3, 9);

            Room oMistyTrail11 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail10, oMistyTrail11, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail11] = new System.Windows.Point(2, 10);

            Room oMistyTrail12 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail11, oMistyTrail12, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail12] = new System.Windows.Point(2, 11);

            Room oMistyTrail13 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail12, oMistyTrail13, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail13] = new System.Windows.Point(1, 12);

            Room oMistyTrail14 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail13, oMistyTrail14, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail14] = new System.Windows.Point(0, 13);

            Room oGrassyField = AddRoom("Grassy Field", "Grassy Field");
            oGrassyField.AddNonPermanentMobs(MobTypeEnum.Griffon);
            AddBidirectionalExits(oGrassyField, oMistyTrail14, BidirectionalExitType.SoutheastNorthwest);
            imladrisToTharbadGraph.Rooms[oGrassyField] = new System.Windows.Point(-1, 12);

            Room spindrilsCastleOutside = AddRoom("Dark Clouds", "Dark Clouds");
            Exit e = AddExit(oGrassyField,spindrilsCastleOutside, "up");
            e.Hidden = true;
            e.PresenceType = ExitPresenceType.RequiresSearch;
            AddExit(spindrilsCastleOutside, oGrassyField, "down");
            imladrisToTharbadGraph.Rooms[spindrilsCastleOutside] = new System.Windows.Point(-1, 11);

            AddSpindrilsCastle(spindrilsCastleOutside);

            oTharbadGateOutside = AddRoom("North Gate", "North Gate of Tharbad");
            AddBidirectionalExits(oMistyTrail14, oTharbadGateOutside, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oTharbadGateOutside] = new System.Windows.Point(0, 14);

            AddLocation(_aImladrisTharbadPerms, oCutthroatAssassin);
            AddLocation(_aImladrisTharbadPerms, oMarkFrey);
        }

        private void AddEsgaroth(Room westGateOfEsgaroth)
        {
            RoomGraph esgarothGraph = new RoomGraph("Esgaroth");
            esgarothGraph.ScalingFactor = 100;
            _graphs[MapType.Esgaroth] = esgarothGraph;

            esgarothGraph.Rooms[westGateOfEsgaroth] = new System.Windows.Point(0, 6);
        }

        private void AddSpindrilsCastle(Room spindrilsCastleOutside)
        {
            RoomGraph spindrilsCastleLevel1Graph = new RoomGraph("Spindril's Castle Level 1");
            spindrilsCastleLevel1Graph.ScalingFactor = 100;
            _graphs[MapType.SpindrilsCastleLevel1] = spindrilsCastleLevel1Graph;

            spindrilsCastleLevel1Graph.Rooms[spindrilsCastleOutside] = new System.Windows.Point(11, 10);

            Room spindrilsCastleInside = AddRoom("Dark/Heavy Clouds", "Dark and Heavy Clouds");
            Exit e = AddExit(spindrilsCastleOutside, spindrilsCastleInside, "up");
            e.FloatRequirement = FloatRequirement.Fly;
            e = AddExit(spindrilsCastleInside, spindrilsCastleOutside, "down");
            e.FloatRequirement = FloatRequirement.Fly;
            spindrilsCastleLevel1Graph.Rooms[spindrilsCastleInside] = new System.Windows.Point(10, 9);

            Room oCloudEdge = AddHealingRoom("Cloud Edge", "Cloud Edge", HealingRoom.SpindrilsCastle);
            AddBidirectionalExits(spindrilsCastleInside, oCloudEdge, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCloudEdge] = new System.Windows.Point(10, 10);

            Room oBrokenCastleWall = AddRoom("Broken Castle Wall", "Broken Castle Wall");
            AddBidirectionalExits(oBrokenCastleWall, spindrilsCastleInside, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oBrokenCastleWall] = new System.Windows.Point(10, 8);
            //CSRTODO: rubble

            Room oEastCastleWall = AddRoom("East Castle Wall", "East Castle Wall");
            AddBidirectionalExits(oEastCastleWall, oBrokenCastleWall, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oEastCastleWall] = new System.Windows.Point(10, 7);

            Room oEastCastleWall2 = AddRoom("East Castle Wall", "East Castle Wall");
            AddBidirectionalExits(oEastCastleWall2, oEastCastleWall, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oEastCastleWall2] = new System.Windows.Point(10, 6);

            Room oSewageVault = AddRoom("Sewage Vault", "Sewage Vault");
            e = AddExit(oEastCastleWall2, oSewageVault, "grate");
            e.Hidden = true;
            AddExit(oSewageVault, oEastCastleWall2, "grate");
            spindrilsCastleLevel1Graph.Rooms[oSewageVault] = new System.Windows.Point(10, 5);

            Room oSewageShaft1 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddExit(oSewageVault, oSewageShaft1, "shaft");
            AddExit(oSewageShaft1, oSewageVault, "east");
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft1] = new System.Windows.Point(9, 5);

            Room oSewageShaft2 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddBidirectionalExits(oSewageShaft2, oSewageShaft1, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft2] = new System.Windows.Point(8, 5);

            Room oSewageShaft3 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddBidirectionalExits(oSewageShaft3, oSewageShaft2, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft3] = new System.Windows.Point(7, 5);

            Room oSewageShaft4 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddBidirectionalExits(oSewageShaft4, oSewageShaft3, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft4] = new System.Windows.Point(6, 5);

            Room oKitchenCorridor = AddRoom("Kitchen Corridor", "Kitchen Corridor");
            AddBidirectionalSameNameExit(oSewageShaft4, oKitchenCorridor, "grate");
            spindrilsCastleLevel1Graph.Rooms[oKitchenCorridor] = new System.Windows.Point(5, 5);

            Room oServiceCorridor = AddRoom("Service Corridor", "Service Corridor");
            AddBidirectionalExits(oKitchenCorridor, oServiceCorridor, BidirectionalExitType.SoutheastNorthwest);
            spindrilsCastleLevel1Graph.Rooms[oServiceCorridor] = new System.Windows.Point(5.5, 5.5);

            Room oArieCorridor = AddRoom("Arie Corridor", "Arie Corridor");
            AddBidirectionalExits(oServiceCorridor, oArieCorridor, BidirectionalExitType.SoutheastNorthwest);
            spindrilsCastleLevel1Graph.Rooms[oArieCorridor] = new System.Windows.Point(6, 6);
            //CSRTODO: ladder

            Room oSpindrilsAerie = AddRoom("Spindril's Aerie", "Spindril's Aerie");
            oSpindrilsAerie.AddPermanentMobs(MobTypeEnum.Roc);
            AddExit(oArieCorridor, oSpindrilsAerie, "aerie");
            AddExit(oSpindrilsAerie, oArieCorridor, "entry");
            spindrilsCastleLevel1Graph.Rooms[oSpindrilsAerie] = new System.Windows.Point(6.5, 5.5);

            Room oTuraksAlcove = AddRoom("Turak's Alcove", "Turak's Alcove");
            AddBidirectionalExits(oTuraksAlcove, oKitchenCorridor, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oTuraksAlcove] = new System.Windows.Point(5, 4.5);
            //CSRTODO: up

            Room oKitchen = AddRoom("Kitchen", "Kitchen");
            AddBidirectionalExits(oKitchen, oKitchenCorridor, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oKitchen] = new System.Windows.Point(4, 5);

            Room oCastleSpindrilCourtyardNE = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExitsWithOut(oCastleSpindrilCourtyardNE, oArieCorridor, "corridor");
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardNE] = new System.Windows.Point(6, 7);

            Room oCastleSpindrilCourtyardE = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardNE, oCastleSpindrilCourtyardE, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardE] = new System.Windows.Point(6, 8);

            Room oCastleSpindrilCourtyardSE = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardE, oCastleSpindrilCourtyardSE, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardSE] = new System.Windows.Point(6, 9);

            Room oCastleSpindrilCourtyardN = AddRoom("Castle Courtyard", "North Court of Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardN, oCastleSpindrilCourtyardNE, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardN] = new System.Windows.Point(5, 7);

            Room oCastleSpindrilCourtyardMiddle = AddRoom("Castle Courtyard", "Center Court for Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardN, oCastleSpindrilCourtyardMiddle, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCastleSpindrilCourtyardMiddle, oCastleSpindrilCourtyardE, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardMiddle] = new System.Windows.Point(5, 8);

            Room oCastleSpindrilCourtyardS = AddRoom("Castle Courtyard", "South Court of Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardMiddle, oCastleSpindrilCourtyardS, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCastleSpindrilCourtyardS, oCastleSpindrilCourtyardSE, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardS] = new System.Windows.Point(5, 9);

            Room oCastleSpindrilCourtyardSW = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardSW, oCastleSpindrilCourtyardS, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardSW] = new System.Windows.Point(4, 9);

            Room oWestsideTunnelEntry = AddRoom("Westside Tunnel Entry", "Westside Tunnel Entry");
            AddBidirectionalExitsWithOut(oCastleSpindrilCourtyardSW, oWestsideTunnelEntry, "tunnel");
            spindrilsCastleLevel1Graph.Rooms[oWestsideTunnelEntry] = new System.Windows.Point(3, 9);

            Room oWestsideHallway1 = AddRoom("Westside Hallway", "Westside Hallway");
            AddBidirectionalExits(oWestsideHallway1, oWestsideTunnelEntry, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oWestsideHallway1] = new System.Windows.Point(3, 8);

            Room oWesternHallway2 = AddRoom("Western Hallway", "Western Hallway");
            AddBidirectionalExits(oWesternHallway2, oWestsideHallway1, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oWesternHallway2] = new System.Windows.Point(3, 7);

            Room oBaseOfBroadStairs = AddRoom("Stairs Base", "Base of the Broad Stairs");
            AddBidirectionalExits(oBaseOfBroadStairs, oWesternHallway2, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oBaseOfBroadStairs] = new System.Windows.Point(3, 6);

            Room oBroadStairs = AddRoom("Broad Stairs", "Broad Stairs");
            AddBidirectionalExits(oBroadStairs, oBaseOfBroadStairs, BidirectionalExitType.UpDown);
            spindrilsCastleLevel1Graph.Rooms[oBroadStairs] = new System.Windows.Point(2, 5);
            //CSRTODO: up

            Room oRedVelvetRoom = AddRoom("Tellia", "Red Velvet Room");
            oRedVelvetRoom.AddPermanentMobs(MobTypeEnum.TelliaTheWitch);
            AddBidirectionalExitsWithOut(oBroadStairs, oRedVelvetRoom, "oak");
            spindrilsCastleLevel1Graph.Rooms[oRedVelvetRoom] = new System.Windows.Point(1, 6);

            Room oBlueVelvetRoom = AddRoom("Blue Velvet", "Blue Velvet Room");
            AddBidirectionalExitsWithOut(oBroadStairs, oBlueVelvetRoom, "ash");
            spindrilsCastleLevel1Graph.Rooms[oBlueVelvetRoom] = new System.Windows.Point(1, 5);

            Room oGreenVelvetRoom = AddRoom("Lord De'Arnse", "Green Velvet Room");
            oGreenVelvetRoom.AddPermanentMobs(MobTypeEnum.LordDeArnse);
            AddBidirectionalExitsWithOut(oBroadStairs, oGreenVelvetRoom, "hickory");
            spindrilsCastleLevel1Graph.Rooms[oGreenVelvetRoom] = new System.Windows.Point(1, 4);

            Room oLowerWesternCorridor = AddRoom("Corridor", "Lower Western Corridor");
            AddBidirectionalSameNameExit(oBaseOfBroadStairs, oLowerWesternCorridor, "door");
            spindrilsCastleLevel1Graph.Rooms[oLowerWesternCorridor] = new System.Windows.Point(3, 5);

            Room oLowerWesternCorridor2 = AddRoom("Corridor", "Lower Western Corridor");
            AddBidirectionalExits(oLowerWesternCorridor2, oLowerWesternCorridor, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oLowerWesternCorridor2] = new System.Windows.Point(3, 4);

            Room oLowerWesternCorridor3 = AddRoom("Corridor", "Lower Western Corridor");
            AddBidirectionalExits(oLowerWesternCorridor3, oLowerWesternCorridor2, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oLowerWesternCorridor3] = new System.Windows.Point(3, 3);

            Room oWesternCorridorStairs = AddRoom("Stairs", "Western Corridor Stairs");
            AddBidirectionalExits(oWesternCorridorStairs, oLowerWesternCorridor3, BidirectionalExitType.UpDown);
            spindrilsCastleLevel1Graph.Rooms[oWesternCorridorStairs] = new System.Windows.Point(3, 2);

            Room oWesternCorridor = AddRoom("Western Corridor", "Western Corridor");
            AddExit(oWesternCorridorStairs, oWesternCorridor, "up");
            AddExit(oWesternCorridor, oWesternCorridorStairs, "stairs");
            spindrilsCastleLevel1Graph.Rooms[oWesternCorridor] = new System.Windows.Point(3, 1);
            //CSRTODO: north, door

            Room oCastleSpindrilCourtyardW = AddRoom("Castle Courtyard", "Western Courtyard for Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardW, oCastleSpindrilCourtyardSW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCastleSpindrilCourtyardW, oCastleSpindrilCourtyardMiddle, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardW] = new System.Windows.Point(4, 8);

            Room oCastleSpindrilCourtyardNW = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardNW, oCastleSpindrilCourtyardN, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCastleSpindrilCourtyardNW, oCastleSpindrilCourtyardW, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardNW] = new System.Windows.Point(4, 7);
            //CSRTODO: steps

            Room oWeaponsmithShop = AddRoom("Weaponsmith's Shop", "Weaponsmith's Shop");
            oWeaponsmithShop.AddPermanentMobs(MobTypeEnum.Gnimbelle);
            AddBidirectionalExitsWithOut(oCastleSpindrilCourtyardS, oWeaponsmithShop, "door");
            spindrilsCastleLevel1Graph.Rooms[oWeaponsmithShop] = new System.Windows.Point(5, 9.5);

            Room oGnimbelleGninbalArmory = AddRoom("Gni Armory", "Gnimbelle and Gninbal's Armory");
            oGnimbelleGninbalArmory.AddPermanentMobs(MobTypeEnum.Gnibal);
            AddBidirectionalSameNameExit(oWeaponsmithShop, oGnimbelleGninbalArmory, "door");
            spindrilsCastleLevel1Graph.Rooms[oGnimbelleGninbalArmory] = new System.Windows.Point(5, 10);

            Room oGniPawnShop = AddRoom("Gni Pawn Shop", "Gnimbelle and Gnarbolla's Pawn Shoppe");
            oGniPawnShop.AddPermanentMobs(MobTypeEnum.Gnarbolla);
            e = AddExit(oGnimbelleGninbalArmory, oGniPawnShop, "passage");
            e.Hidden = true;
            AddExit(oGniPawnShop, oGnimbelleGninbalArmory, "out");
            spindrilsCastleLevel1Graph.Rooms[oGniPawnShop] = new System.Windows.Point(5, 10.5);

            Room oSouthernStairwellAlcove = AddRoom("South Stairwell Alcove", "Southern Tower's Stairwell Alcove");
            AddExit(oCastleSpindrilCourtyardSE, oSouthernStairwellAlcove, "alcove");
            AddExit(oSouthernStairwellAlcove, oCastleSpindrilCourtyardSE, "north");
            spindrilsCastleLevel1Graph.Rooms[oSouthernStairwellAlcove] = new System.Windows.Point(7, 9);
            //CSRTODO: up

            Room oBarracksHallway = AddRoom("Barracks Hallway", "Barracks Hallway");
            AddBidirectionalExitsWithOut(oSouthernStairwellAlcove, oBarracksHallway, "door");
            spindrilsCastleLevel1Graph.Rooms[oBarracksHallway] = new System.Windows.Point(8, 9);

            Room oCastleBarracks = AddRoom("Castle Barracks", "Castle Barracks");
            AddBidirectionalExitsWithOut(oBarracksHallway, oCastleBarracks, "barracks");
            spindrilsCastleLevel1Graph.Rooms[oCastleBarracks] = new System.Windows.Point(8, 8);

            Room oCastleArmory = AddRoom("Castle Armory", "Castle Spindril Armory");
            AddBidirectionalExitsWithOut(oBarracksHallway, oCastleArmory, "armory");
            spindrilsCastleLevel1Graph.Rooms[oCastleArmory] = new System.Windows.Point(8, 10);
        }

        private void AddShantyTown(Room oMistyTrail8)
        {
            RoomGraph imladrisToTharbadGraph = _graphs[MapType.ImladrisToTharbad];

            RoomGraph oShantyTownGraph = new RoomGraph("Shanty Town");
            oShantyTownGraph.ScalingFactor = 100;
            _graphs[MapType.ShantyTown] = oShantyTownGraph;

            oShantyTownGraph.Rooms[oMistyTrail8] = new System.Windows.Point(5, 0);

            Room oRuttedDirtRoad = AddRoom("Dirt Road", "Rutted Dirt Road");
            AddBidirectionalExits(oRuttedDirtRoad, oMistyTrail8, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oRuttedDirtRoad] = new System.Windows.Point(4, 0);
            imladrisToTharbadGraph.Rooms[oRuttedDirtRoad] = new System.Windows.Point(3, 7);

            Room oHouseOfPleasure = AddRoom("mistress", "House of Pleasure");
            oHouseOfPleasure.AddPermanentMobs(MobTypeEnum.Mistress);
            AddBidirectionalSameNameExit(oRuttedDirtRoad, oHouseOfPleasure, "door");
            oShantyTownGraph.Rooms[oHouseOfPleasure] = new System.Windows.Point(4, -1);

            Room oMadameDespana = AddRoom("Madame Despana", "Private Bedchamber");
            oMadameDespana.AddPermanentMobs(MobTypeEnum.MadameDespana);
            Exit e = AddExit(oHouseOfPleasure, oMadameDespana, "crimson");
            e.MustOpen = true;
            e = AddExit(oMadameDespana, oHouseOfPleasure, "crimson");
            e.MustOpen = true;
            oShantyTownGraph.Rooms[oMadameDespana] = new System.Windows.Point(3, -1);

            Room oNorthEdgeOfShantyTown = AddRoom("Shanty Town", "North Edge of Shanty Town");
            AddBidirectionalExits(oRuttedDirtRoad, oNorthEdgeOfShantyTown, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oNorthEdgeOfShantyTown] = new System.Windows.Point(4, 1);

            Room oRedFoxLane = AddRoom("Red Fox", "Red Fox Lane");
            AddBidirectionalExits(oRedFoxLane, oNorthEdgeOfShantyTown, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oRedFoxLane] = new System.Windows.Point(3, 1);

            Room oGypsyCamp = AddRoom("Gypsy Camp", "Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp, oRedFoxLane, BidirectionalExitType.SoutheastNorthwest);
            oShantyTownGraph.Rooms[oGypsyCamp] = new System.Windows.Point(2, 0);

            Room oGypsyCamp2 = AddRoom("Gypsy Camp", "Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp2, oGypsyCamp, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oGypsyCamp2] = new System.Windows.Point(1, 0);

            Room oMadameProkawskiPalmReadingService = AddRoom("Palm Reading Service", "Madame Prokawski's Palm Reading Service");
            AddBidirectionalExitsWithOut(oGypsyCamp2, oMadameProkawskiPalmReadingService, "wagon");
            oShantyTownGraph.Rooms[oMadameProkawskiPalmReadingService] = new System.Windows.Point(1, -1);

            Room oGypsyAnimalKeep = AddRoom("Gypsy Animal Keep", "Gypsy Animal Keep");
            AddBidirectionalSameNameExit(oGypsyCamp2, oGypsyAnimalKeep, "gate");
            oShantyTownGraph.Rooms[oGypsyAnimalKeep] = new System.Windows.Point(0, 0);

            Room oExoticAnimalKeep = AddRoom("Exotic Animal Wagon", "Exotic Animal Wagon");
            AddBidirectionalExitsWithOut(oGypsyAnimalKeep, oExoticAnimalKeep, "wagon");
            oShantyTownGraph.Rooms[oExoticAnimalKeep] = new System.Windows.Point(-1, 0);

            Room oNorthShantyTown = AddRoom("Shanty Town", "North Shanty Town");
            AddBidirectionalExits(oRedFoxLane, oNorthShantyTown, BidirectionalExitType.SouthwestNortheast);
            oShantyTownGraph.Rooms[oNorthShantyTown] = new System.Windows.Point(2, 2);

            Room oShantyTownDump = AddRoom("Town Dump", "Shanty Town Dump");
            AddBidirectionalExits(oNorthShantyTown, oShantyTownDump, BidirectionalExitType.SouthwestNortheast);
            oShantyTownGraph.Rooms[oShantyTownDump] = new System.Windows.Point(1, 3);

            Room oShantyTownWest = AddRoom("Shanty Town", "Shanty Town West");
            AddBidirectionalExits(oShantyTownDump, oShantyTownWest, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTownWest] = new System.Windows.Point(1, 4);

            Room oCopseOfTrees = AddRoom("Copse of Trees", "Copse of Trees");
            AddBidirectionalExits(oShantyTownWest, oCopseOfTrees, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oCopseOfTrees] = new System.Windows.Point(1, 5);

            Room oBluff = AddRoom("Bluff", "Shanty Town Bluff");
            AddBidirectionalExits(oCopseOfTrees, oBluff, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oBluff] = new System.Windows.Point(1, 6);

            Room oShantyTown1 = AddRoom("Shanty Town", "Shanty Town");
            AddBidirectionalExits(oNorthEdgeOfShantyTown, oShantyTown1, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTown1] = new System.Windows.Point(4, 2);

            Room oShantyTown2 = AddRoom("Shanty Town", "Shanty Town");
            AddBidirectionalExits(oShantyTown1, oShantyTown2, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTown2] = new System.Windows.Point(4, 3);

            Room oShantyTown3 = AddRoom("Shanty Town", "Shanty Town");
            AddBidirectionalExits(oShantyTown2, oShantyTown3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBluff, oShantyTown3, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oShantyTown3] = new System.Windows.Point(3, 6);

            Room oPentampurisPalace = AddRoom("Pentampuri's Palace", "Pentampuri's Palace");
            AddBidirectionalExitsWithOut(oShantyTown3, oPentampurisPalace, "shack");
            oShantyTownGraph.Rooms[oPentampurisPalace] = new System.Windows.Point(4, 6);

            Room oPrinceBrunden = AddRoom("Prince Brunden", "King of the Gypsies Wagon");
            oPrinceBrunden.AddPermanentMobs(MobTypeEnum.PrinceBrunden);
            AddBidirectionalExitsWithOut(oGypsyCamp, oPrinceBrunden, "wagon");
            oShantyTownGraph.Rooms[oPrinceBrunden] = new System.Windows.Point(2, -1);

            Room oNaugrim = AddRoom("Naugrim", "Naugrim's Wine Cask Home");
            oNaugrim.AddPermanentMobs(MobTypeEnum.Naugrim);
            AddBidirectionalExitsWithOut(oNorthShantyTown, oNaugrim, "cask");
            oShantyTownGraph.Rooms[oNaugrim] = new System.Windows.Point(1, 1);

            Room oHogoth = AddRoom("Hogoth", "Hogoth's Home");
            oHogoth.AddPermanentMobs(MobTypeEnum.Hogoth);
            AddBidirectionalExitsWithOut(oShantyTownWest, oHogoth, "shack");
            oShantyTownGraph.Rooms[oHogoth] = new System.Windows.Point(0, 4);

            Room oFaornil = AddRoom("Faornil", "Seer's Tent");
            oFaornil.AddPermanentMobs(MobTypeEnum.FaornilTheSeer);
            AddBidirectionalExitsWithOut(oShantyTown1, oFaornil, "tent");
            oShantyTownGraph.Rooms[oFaornil] = new System.Windows.Point(5, 2);

            Room oGraddy = AddRoom("Graddy", "Graddy the Dwarf's Wagon");
            oGraddy.AddPermanentMobs(MobTypeEnum.Graddy);
            AddBidirectionalExitsWithOut(oShantyTown2, oGraddy, "wagon");
            oShantyTownGraph.Rooms[oGraddy] = new System.Windows.Point(5, 3);

            Room oGraddyOgre = AddRoom("Ogre", "Graddy's Ogre Pen");
            oGraddyOgre.AddPermanentMobs(MobTypeEnum.Ogre);
            e = AddExit(oGraddy, oGraddyOgre, "gate");
            e.MustOpen = true;
            e = AddExit(oGraddyOgre, oGraddy, "gate");
            e.MustOpen = true;
            oShantyTownGraph.Rooms[oGraddyOgre] = new System.Windows.Point(5, 4);

            AddLocation(_aImladrisTharbadPerms, oPrinceBrunden);
            AddLocation(_aImladrisTharbadPerms, oNaugrim);
            AddLocation(_aImladrisTharbadPerms, oHogoth);
            AddLocation(_aImladrisTharbadPerms, oFaornil);
            AddLocation(_aImladrisTharbadPerms, oGraddy);
        }

        private void AddIntangible(Room oBreeTownSquare, Room healingHand, Room nindamosVillageCenter)
        {
            RoomGraph intangibleGraph = new RoomGraph("Intangible");
            intangibleGraph.ScalingFactor = 100;
            _graphs[MapType.Intangible] = intangibleGraph;

            intangibleGraph.Rooms[oBreeTownSquare] = new System.Windows.Point(0, 0);
            intangibleGraph.Rooms[healingHand] = new System.Windows.Point(1, 0);
            intangibleGraph.Rooms[nindamosVillageCenter] = new System.Windows.Point(2, 0);

            Room treeOfLife = AddRoom("Tree of Life", "The Tree of Life");
            treeOfLife.Intangible = true;
            AddExit(treeOfLife, oBreeTownSquare, "down");
            intangibleGraph.Rooms[treeOfLife] = new System.Windows.Point(0, 1);

            Room oLimbo = AddRoom("Limbo", "Limbo");
            oLimbo.Intangible = true;
            Exit e = AddExit(oLimbo, treeOfLife, "green");
            e.MustOpen = true;
            intangibleGraph.Rooms[oLimbo] = new System.Windows.Point(1, 2);

            Room oDarkTunnel = AddRoom("Dark Tunnel", "Dark Tunnel");
            oDarkTunnel.Intangible = true;
            e = AddExit(oLimbo, oDarkTunnel, "blue");
            e.MustOpen = true;
            e.MinimumLevel = 4;
            AddExit(oDarkTunnel, healingHand, "light");
            intangibleGraph.Rooms[oDarkTunnel] = new System.Windows.Point(1, 1);

            Room oFluffyCloudsAboveNindamos = AddRoom("Fluffy Clouds", "Fluffy clouds above Nindamos");
            oFluffyCloudsAboveNindamos.Intangible = false;
            e = AddExit(oLimbo, oFluffyCloudsAboveNindamos, "white");
            e.MustOpen = true;
            AddExit(oFluffyCloudsAboveNindamos, nindamosVillageCenter, "green");
            intangibleGraph.Rooms[oFluffyCloudsAboveNindamos] = new System.Windows.Point(2, 1);
        }

        private void AddNindamos(out Room oArmenelosGatesOutside, out Room oSouthernJunction, out Room oPathThroughTheValleyHiddenPath, out Room nindamosDocks, out RoomGraph nindamosGraph, out Room nindamosVillageCenter)
        {
            nindamosGraph = new RoomGraph("Nindamos");
            nindamosGraph.ScalingFactor = 100;
            _graphs[MapType.Nindamos] = nindamosGraph;

            nindamosVillageCenter = AddRoom("Village Center", "Nindamos Village Center");
            nindamosVillageCenter.AddPermanentMobs(MobTypeEnum.MaxTheVegetableVendor);
            nindamosGraph.Rooms[nindamosVillageCenter] = new System.Windows.Point(8, 4);

            Room oSandstoneNorth1 = AddRoom("Sandstone", "Sandstone Road North");
            AddBidirectionalExits(oSandstoneNorth1, nindamosVillageCenter, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneNorth1] = new System.Windows.Point(8, 3);

            Room oSandstoneNorth2 = AddRoom("Sandstone", "Sandstone Road North");
            AddBidirectionalExits(oSandstoneNorth2, oSandstoneNorth1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneNorth2] = new System.Windows.Point(8, 2);

            Room oSandyPath1 = AddRoom("Sandy Path", "Sandy Path");
            AddBidirectionalExits(oSandstoneNorth2, oSandyPath1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyPath1] = new System.Windows.Point(8.75, 2);

            Room oSandyPath2 = AddRoom("Sandy Path", "Sandy Path");
            AddBidirectionalExits(oSandyPath1, oSandyPath2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyPath2] = new System.Windows.Point(9.5, 2);

            Room oSandyPath3 = AddRoom("Sandy Path", "Sandy Path");
            AddBidirectionalExits(oSandyPath2, oSandyPath3, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyPath3] = new System.Windows.Point(9.5, 2.3);

            Room oMarketplace = AddRoom("Marketplace", "The Marketplace");
            Exit e = AddExit(oSandyPath3, oMarketplace, "door");
            e.MustOpen = true;
            e.RequiresDay = true;
            AddExit(oMarketplace, oSandyPath3, "door");
            nindamosGraph.Rooms[oMarketplace] = new System.Windows.Point(9.5, 2.6);

            Room oSmithy = AddRoom("Smithy", "The Marketplace");
            oSmithy.AddPermanentMobs(MobTypeEnum.Smithy);
            AddBidirectionalExits(oSmithy, oMarketplace, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSmithy] = new System.Windows.Point(8.5, 2.6);

            Room oSandstoneDrivel = AddRoom("Drivel/Sandstone", "Sandstone Road / Drivel Avenue");
            AddBidirectionalExits(oSandstoneDrivel, oSandstoneNorth2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneDrivel] = new System.Windows.Point(8, 1);

            Room oSandstoneSouth1 = AddRoom("Sandstone", "Sandstone Road South");
            AddBidirectionalExits(nindamosVillageCenter, oSandstoneSouth1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneSouth1] = new System.Windows.Point(8, 5);

            Room oSandstoneSouth2 = AddRoom("Sandstone", "Sandstone Road South");
            AddBidirectionalExits(oSandstoneSouth1, oSandstoneSouth2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneSouth2] = new System.Windows.Point(8, 6);

            Room oKKsIronWorksKosta = AddRoom("Kosta", "KK's Ironworks");
            oKKsIronWorksKosta.AddPermanentMobs(MobTypeEnum.Kosta);
            e = AddExit(oSandstoneSouth2, oKKsIronWorksKosta, "path");
            e.RequiresDay = true;
            AddExit(oKKsIronWorksKosta, oSandstoneSouth2, "out");
            nindamosGraph.Rooms[oKKsIronWorksKosta] = new System.Windows.Point(7, 6);

            Room oKauka = AddRoom("Kauka", "Kauka's Living Room");
            oKauka.AddPermanentMobs(MobTypeEnum.Kauka);
            AddBidirectionalExitsWithOut(oKKsIronWorksKosta, oKauka, "doorway");
            nindamosGraph.Rooms[oKauka] = new System.Windows.Point(7, 7);

            Room oLimestoneSandstone = AddRoom("Limestone/Sandstone", "Sandstone Road / Limestone Street");
            AddBidirectionalExits(oSandstoneSouth2, oLimestoneSandstone, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oLimestoneSandstone] = new System.Windows.Point(8, 7);

            Room oDrivel1 = AddRoom("Drivel", "Drivel Avenue");
            AddBidirectionalExits(oSandstoneDrivel, oDrivel1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oDrivel1] = new System.Windows.Point(9, 1);

            Room oDrivel2 = AddRoom("Drivel", "Drivel Avenue");
            AddBidirectionalExits(oDrivel1, oDrivel2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oDrivel2] = new System.Windows.Point(10, 1);

            Room oDrivelElysia = AddRoom("Drivel/Elysia", "Elysia Street / Drivel Avenue");
            AddBidirectionalExits(oDrivel2, oDrivelElysia, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oDrivelElysia] = new System.Windows.Point(11, 1);

            Room oSandyBeach1 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oDrivelElysia, oSandyBeach1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyBeach1] = new System.Windows.Point(12, 1);

            Room oPaledasenta1 = AddRoom("Paledasenta", "Paledasenta Street");
            AddBidirectionalExits(nindamosVillageCenter, oPaledasenta1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oPaledasenta1] = new System.Windows.Point(9, 4);

            Room oNindamosPostOffice = AddRoom("Post Office", "Nindamos Post Office");
            AddBidirectionalExitsWithOut(oPaledasenta1, oNindamosPostOffice, "south");
            nindamosGraph.Rooms[oNindamosPostOffice] = new System.Windows.Point(9, 5);

            Room oPaledasenta2 = AddRoom("Paledasenta", "Paledasenta Street");
            AddBidirectionalExits(oPaledasenta1, oPaledasenta2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oPaledasenta2] = new System.Windows.Point(10, 4);

            Room oHealthCenter = AddHealingRoom("Health Center", "Nindamos Health Center", HealingRoom.Nindamos);
            AddBidirectionalExits(oHealthCenter, oPaledasenta2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oHealthCenter] = new System.Windows.Point(10, 3.5);

            Room oPaledasentaElysia = AddRoom("Paledasenta/Elysia", "Elysia Street / Paledasenta Street");
            AddBidirectionalExits(oPaledasenta2, oPaledasentaElysia, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oPaledasentaElysia] = new System.Windows.Point(11, 4);

            Room oSandyBeach4 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oPaledasentaElysia, oSandyBeach4, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyBeach4] = new System.Windows.Point(12, 4);

            Room oLimestone1 = AddRoom("Limestone", "Limestone Street");
            AddBidirectionalExits(oLimestoneSandstone, oLimestone1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oLimestone1] = new System.Windows.Point(9, 7);

            Room oSandPlaygroundSW = AddRoom("Malika", "The Sand Playground");
            oSandPlaygroundSW.AddPermanentMobs(MobTypeEnum.Malika);
            AddBidirectionalExits(oSandPlaygroundSW, oLimestone1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandPlaygroundSW] = new System.Windows.Point(9, 6.5);

            Room oSandPlaygroundNW = AddRoom("Sand Playground", "Sand Playground");
            AddBidirectionalExits(oSandPlaygroundNW, oSandPlaygroundSW, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandPlaygroundNW] = new System.Windows.Point(9, 6);

            Room oSandPlaygroundNE = AddRoom("Sand Playground", "The Sand Playground");
            AddBidirectionalExits(oSandPlaygroundNW, oSandPlaygroundNE, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandPlaygroundNE] = new System.Windows.Point(10, 6);

            Room oSandPlaygroundSE = AddRoom("Sand Playground", "The Sand Playground");
            AddBidirectionalExits(oSandPlaygroundNE, oSandPlaygroundSE, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandPlaygroundSW, oSandPlaygroundSE, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandPlaygroundSE] = new System.Windows.Point(10, 6.5);

            Room oSandcastle = AddRoom("sobbing girl", "Inside the Sandcastle");
            oSandcastle.AddPermanentMobs(MobTypeEnum.SobbingGirl);
            AddBidirectionalExitsWithOut(oSandPlaygroundNE, oSandcastle, "sandcastle");
            nindamosGraph.Rooms[oSandcastle] = new System.Windows.Point(10, 5.5);

            Room oLimestone2 = AddRoom("Limestone", "Limestone Street");
            AddBidirectionalExits(oLimestone1, oLimestone2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oLimestone2] = new System.Windows.Point(10, 7);

            Room oLimestoneElysia = AddRoom("Numenorean Warder", "Elysia Street / Limestone Street");
            oLimestoneElysia.AddPermanentMobs(MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oLimestone2, oLimestoneElysia, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oLimestoneElysia] = new System.Windows.Point(11, 7);

            Room oSandyBeach7 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oLimestoneElysia, oSandyBeach7, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyBeach7] = new System.Windows.Point(12, 7);

            Room oSandyBeach2 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach1, oSandyBeach2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach2] = new System.Windows.Point(12, 2);

            Room oSandyBeach3 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach2, oSandyBeach3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeach3, oSandyBeach4, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach3] = new System.Windows.Point(12, 3);

            Room oSandyBeach5 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach4, oSandyBeach5, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach5] = new System.Windows.Point(12, 5);

            Room oSandyBeach6 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach5, oSandyBeach6, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeach6, oSandyBeach7, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach6] = new System.Windows.Point(12, 6);

            Room oSandyBeachNorth = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeachNorth, oSandyBeach1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeachNorth] = new System.Windows.Point(12, 0);

            Room oSandyBeachSouth = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach7, oSandyBeachSouth, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeachSouth] = new System.Windows.Point(12, 8);

            Room oShoreline1 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oSandyBeachNorth, oShoreline1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oShoreline1] = new System.Windows.Point(13, 0);

            Room oShoreline2 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oSandyBeach1, oShoreline2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oShoreline1, oShoreline2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline2] = new System.Windows.Point(13, 1);

            Room oShoreline3 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline2, oShoreline3, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline3] = new System.Windows.Point(13, 2);

            Room oShoreline4 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline3, oShoreline4, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline4] = new System.Windows.Point(13, 3);

            Room oShoreline5 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline4, oShoreline5, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeach4, oShoreline5, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oShoreline5] = new System.Windows.Point(13, 4);

            Room oShoreline6 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline5, oShoreline6, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline6] = new System.Windows.Point(13, 5);

            Room oShoreline7 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline6, oShoreline7, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline7] = new System.Windows.Point(13, 6);

            Room oShoreline8 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline7, oShoreline8, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline8] = new System.Windows.Point(13, 7);

            Room oSmallDock = AddRoom("Small Dock", "A Small Dock");
            e = AddExit(oShoreline8, oSmallDock, "east");
            e.Hidden = true;
            AddExit(oSmallDock, oShoreline8, "west");
            nindamosGraph.Rooms[oSmallDock] = new System.Windows.Point(14, 7);

            nindamosDocks = AddRoom("Small Dock", "A Small Dock");
            nindamosDocks.BoatLocationType = BoatEmbarkOrDisembark.BullroarerNindamos;
            AddBidirectionalExits(oSmallDock, nindamosDocks, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[nindamosDocks] = new System.Windows.Point(15, 7);

            Room oShoreline9 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline8, oShoreline9, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeachSouth, oShoreline9, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oShoreline9] = new System.Windows.Point(13, 8);

            Room oElysia1 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oDrivelElysia, oElysia1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia1] = new System.Windows.Point(11, 2);

            Room oElysia2 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oElysia1, oElysia2, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElysia2, oPaledasentaElysia, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia2] = new System.Windows.Point(11, 3);

            Room oHestasMarket = AddRoom("Hesta's Market", "Hesta's Market");
            oHestasMarket.AddPermanentMobs(MobTypeEnum.Hesta);
            e = AddExit(oElysia2, oHestasMarket, "market");
            e.RequiresDay = true;
            AddExit(oHestasMarket, oElysia2, "out");
            nindamosGraph.Rooms[oHestasMarket] = new System.Windows.Point(10, 3);

            Room oElysia3 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oPaledasentaElysia, oElysia3, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia3] = new System.Windows.Point(11, 5);

            Room oElysia4 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oElysia3, oElysia4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElysia4, oLimestoneElysia, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia4] = new System.Windows.Point(11, 6);

            Room oGranitePath1 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath1, nindamosVillageCenter, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath1] = new System.Windows.Point(7, 4);

            Room oGranitePath2 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath2, oGranitePath1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath2] = new System.Windows.Point(6, 4);

            Room oAlasse = AddRoom("Alasse's Pub", "Alasse's Pub");
            oAlasse.AddPermanentMobs(MobTypeEnum.Alasse);
            e = AddExit(oGranitePath2, oAlasse, "south");
            e.RequiresDay = true;
            AddExit(oAlasse, oGranitePath2, "out");
            nindamosGraph.Rooms[oAlasse] = new System.Windows.Point(6, 5);

            Room oGranitePath3 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath3, oGranitePath2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath3] = new System.Windows.Point(5, 4);

            Room oGranitePath4 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath4, oGranitePath3, BidirectionalExitType.SoutheastNorthwest);
            nindamosGraph.Rooms[oGranitePath4] = new System.Windows.Point(4, 3);

            Room oGranitePath5 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath5, oGranitePath4, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath5] = new System.Windows.Point(3, 3);

            Room oGranitePath6 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath6, oGranitePath5, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath6] = new System.Windows.Point(2, 3);

            Room oGranitePath7 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath7, oGranitePath6, BidirectionalExitType.SoutheastNorthwest);
            nindamosGraph.Rooms[oGranitePath7] = new System.Windows.Point(1, 2);

            oSouthernJunction = AddRoom("Southern Junction", "Southern Junction");
            oSouthernJunction.AddPermanentMobs(MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oSouthernJunction, oGranitePath7, BidirectionalExitType.SoutheastNorthwest);
            nindamosGraph.Rooms[oSouthernJunction] = new System.Windows.Point(0, 1);

            Room oPathToArmenelos1 = AddRoom("Valley Path");
            AddBidirectionalExits(oPathToArmenelos1, oSouthernJunction, BidirectionalExitType.SouthwestNortheast);
            nindamosGraph.Rooms[oPathToArmenelos1] = new System.Windows.Point(1, 0);

            oPathThroughTheValleyHiddenPath = AddRoom("Valley Path");
            AddBidirectionalExits(oPathThroughTheValleyHiddenPath, oPathToArmenelos1, BidirectionalExitType.SouthwestNortheast);
            nindamosGraph.Rooms[oPathThroughTheValleyHiddenPath] = new System.Windows.Point(2, -1);

            oArmenelosGatesOutside = AddRoom("Gate Outside");
            AddBidirectionalExits(oArmenelosGatesOutside, oPathThroughTheValleyHiddenPath, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oArmenelosGatesOutside] = new System.Windows.Point(2, -2);
        }

        private void AddArmenelos(Room oArmenelosGatesOutside)
        {
            RoomGraph armenelosGraph = new RoomGraph("Armenelos");
            armenelosGraph.ScalingFactor = 100;
            _graphs[MapType.Armenelos] = armenelosGraph;

            Room oAdrahilHirgon = AddRoom("Adrahil/Hirgon");
            armenelosGraph.Rooms[oAdrahilHirgon] = new System.Windows.Point(0, 0);

            Room oAdrahil1 = AddRoom("Adrahil");
            AddBidirectionalExits(oAdrahilHirgon, oAdrahil1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil1] = new System.Windows.Point(1, 0);

            Room oAdrahilRivel = AddRoom("Adrahil/Rivel");
            AddBidirectionalExits(oAdrahil1, oAdrahilHirgon, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilRivel] = new System.Windows.Point(2, 0);

            Room oAdrahil2 = AddRoom("Adrahil");
            AddBidirectionalExits(oAdrahilRivel, oAdrahil2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil2] = new System.Windows.Point(3, 0);

            Room oAdrahil3 = AddRoom("Adrahil");
            AddBidirectionalExits(oAdrahil2, oAdrahil3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil3] = new System.Windows.Point(4, 0);

            Room oAdrahil4 = AddRoom("Adrahil");
            AddBidirectionalExits(oAdrahil3, oAdrahil4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil4] = new System.Windows.Point(5, 0);

            Room oCityDump = AddRoom("City Dump");
            AddBidirectionalExitsWithOut(oAdrahil4, oCityDump, "gate");
            armenelosGraph.Rooms[oCityDump] = new System.Windows.Point(5, 1);

            Room oDori = AddRoom("Dori");
            AddBidirectionalExitsWithOut(oCityDump, oDori, "dump");
            armenelosGraph.Rooms[oDori] = new System.Windows.Point(4, 1);

            Room oAdrahilFolca = AddRoom("Adrahil/Folca");
            AddBidirectionalExits(oAdrahil4, oAdrahilFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilFolca] = new System.Windows.Point(6, 0);

            Room oAdrahil5 = AddRoom("Adrahil");
            AddBidirectionalExits(oAdrahilFolca, oAdrahil5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil5] = new System.Windows.Point(7, 0);

            Room oAdrahilWindfola = AddRoom("Adrahil/Windfola");
            AddBidirectionalExits(oAdrahil5, oAdrahilWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilWindfola] = new System.Windows.Point(8, 0);

            Room oHirgon1 = AddRoom("Hirgon");
            AddBidirectionalExits(oAdrahilHirgon, oHirgon1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon1] = new System.Windows.Point(0, 1);

            Room oDoctorFaramir = AddRoom("Dr Faramir");
            AddBidirectionalExitsWithOut(oHirgon1, oDoctorFaramir, "door");
            armenelosGraph.Rooms[oDoctorFaramir] = new System.Windows.Point(1, 1);

            Room oRivel1 = AddRoom("Rivel");
            AddBidirectionalExits(oAdrahilRivel, oRivel1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel1] = new System.Windows.Point(2, 1);

            Room oFolca1 = AddRoom("Folca");
            AddBidirectionalExits(oAdrahilFolca, oFolca1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca1] = new System.Windows.Point(6, 1);

            Room oWindfola1 = AddRoom("Windfola");
            AddBidirectionalExits(oAdrahilWindfola, oWindfola1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola1] = new System.Windows.Point(8, 1);

            Room oDorlasHirgon = AddRoom("Dorlas/Hirgon");
            AddBidirectionalExits(oHirgon1, oDorlasHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oDorlasHirgon] = new System.Windows.Point(0, 2);

            Room oDorlas1 = AddRoom("Dorlas");
            AddBidirectionalExits(oDorlasHirgon, oDorlas1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas1] = new System.Windows.Point(1, 2);

            Room oDorlasRivel = AddRoom("Dorlas/Rivel");
            AddBidirectionalExits(oRivel1, oDorlasRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas1, oDorlasRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasRivel] = new System.Windows.Point(2, 2);

            Room oDorlas2 = AddRoom("Dorlas");
            AddBidirectionalExits(oDorlasRivel, oDorlas2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas2] = new System.Windows.Point(3, 2);

            Room oDorlas3 = AddRoom("Dorlas");
            AddBidirectionalExits(oDorlas2, oDorlas3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas3] = new System.Windows.Point(4, 2);

            Room oDorlas4 = AddRoom("Dorlas");
            AddBidirectionalExits(oDorlas3, oDorlas4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas4] = new System.Windows.Point(5, 2);

            Room oDorlasFolca = AddRoom("Dorlas/Folca");
            AddBidirectionalExits(oFolca1, oDorlasFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas4, oDorlasFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasFolca] = new System.Windows.Point(6, 2);

            Room oDorlas5 = AddRoom("Dorlas");
            AddBidirectionalExits(oDorlasFolca, oDorlas5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas5] = new System.Windows.Point(7, 2);

            Room oTamar = AddRoom("Tamar");
            AddBidirectionalExits(oDorlas5, oTamar, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oTamar] = new System.Windows.Point(7, 2.5);

            Room oDorlasWindfola = AddRoom("Dorlas/Windfola");
            AddBidirectionalExits(oWindfola1, oDorlasWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas5, oDorlasWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasWindfola] = new System.Windows.Point(8, 2);

            Room oHirgon2 = AddRoom("Hirgon");
            AddBidirectionalExits(oDorlasHirgon, oHirgon2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon2] = new System.Windows.Point(0, 3);

            Room oRivel2 = AddRoom("Rivel");
            AddBidirectionalExits(oDorlasRivel, oRivel2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel2] = new System.Windows.Point(2, 3);

            Room oFolca2 = AddRoom("Folca");
            AddBidirectionalExits(oDorlasFolca, oFolca2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca2] = new System.Windows.Point(6, 3);

            Room oWindfola2 = AddRoom("Windfola");
            AddBidirectionalExits(oDorlasWindfola, oWindfola2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola2] = new System.Windows.Point(8, 3);

            Room oAzgara = AddRoom("Azgara");
            AddBidirectionalExits(oAzgara, oWindfola2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAzgara] = new System.Windows.Point(7, 3);

            Room oOnlyArmor = AddRoom("Kali");
            AddBidirectionalExitsWithOut(oAzgara, oOnlyArmor, "door");
            armenelosGraph.Rooms[oOnlyArmor] = new System.Windows.Point(6.5, 3.5);

            Room oSpecialtyShoppe = AddRoom("Specialty");
            AddBidirectionalExitsWithOut(oAzgara, oSpecialtyShoppe, "curtain");
            armenelosGraph.Rooms[oSpecialtyShoppe] = new System.Windows.Point(7.5, 3.5);

            Room oThalosHirgon = AddRoom("Hirgon/Thalos");
            AddBidirectionalExits(oHirgon2, oThalosHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oThalosHirgon] = new System.Windows.Point(0, 4);

            Room oThalos1 = AddRoom("Thalos");
            AddBidirectionalExits(oThalosHirgon, oThalos1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos1] = new System.Windows.Point(1, 4);

            Room oThalosRivel = AddRoom("Thalos/Rivel");
            AddBidirectionalExits(oRivel2, oThalosRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos1, oThalosRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosRivel] = new System.Windows.Point(2, 4);

            Room oThalos2 = AddRoom("Thalos");
            AddBidirectionalExits(oThalosRivel, oThalos2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos2] = new System.Windows.Point(3, 4);

            Room oThalos3 = AddRoom("Thalos");
            AddBidirectionalExits(oThalos2, oThalos3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos3] = new System.Windows.Point(4, 4);

            Room oThalos4 = AddRoom("Thalos");
            AddBidirectionalExits(oThalos3, oThalos4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos4] = new System.Windows.Point(5, 4);

            Room oThalosFolca = AddRoom("Thalos/Folca");
            AddBidirectionalExits(oFolca2, oThalosFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos4, oThalosFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosFolca] = new System.Windows.Point(6, 4);

            Room oThalos5 = AddRoom("Thalos");
            AddBidirectionalExits(oThalosFolca, oThalos5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos5] = new System.Windows.Point(7, 4);

            Room oThalosWindfola = AddRoom("Thalos/Windfola");
            AddBidirectionalExits(oWindfola2, oThalosWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos5, oThalosWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosWindfola] = new System.Windows.Point(8, 4);

            Room oHirgon3 = AddRoom("Hirgon");
            AddBidirectionalExits(oThalosHirgon, oHirgon3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon3] = new System.Windows.Point(0, 5);

            Room oRivel3 = AddRoom("Rivel");
            AddBidirectionalExits(oThalosRivel, oRivel3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel3] = new System.Windows.Point(2, 5);
            //CSRTODO: south (blocked)

            Room oFolca3 = AddRoom("Folca");
            AddBidirectionalExits(oThalosFolca, oFolca3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca3] = new System.Windows.Point(6, 5);
            //CSRTODO: south (blocked)

            Room oWindfola3 = AddRoom("Windfola");
            AddBidirectionalExits(oThalosWindfola, oWindfola3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola3] = new System.Windows.Point(8, 5);

            Room oEllessarHirgon = AddRoom("Ellessar/Hirgon");
            AddBidirectionalExits(oHirgon3, oEllessarHirgon, BidirectionalExitType.NorthSouth);
            //CSRTODO: east (blocked)
            armenelosGraph.Rooms[oEllessarHirgon] = new System.Windows.Point(0, 6);

            Room oEllessarWindfola = AddRoom("Ellessar/Windfola");
            AddBidirectionalExits(oWindfola3, oEllessarWindfola, BidirectionalExitType.NorthSouth);
            //CSRTODO: west (blocked)
            armenelosGraph.Rooms[oEllessarWindfola] = new System.Windows.Point(8, 6);

            Room oHirgon4 = AddRoom("Hirgon");
            AddBidirectionalExits(oEllessarHirgon, oHirgon4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon4] = new System.Windows.Point(0, 7);

            Room oOutdoorMarket = AddRoom("OutdoorMarket");
            AddBidirectionalExits(oHirgon4, oOutdoorMarket, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOutdoorMarket] = new System.Windows.Point(1, 7);

            Room oRivel4 = AddRoom("Rivel");
            armenelosGraph.Rooms[oRivel4] = new System.Windows.Point(2, 7);
            //CSRTODO: north (blocked)

            Room oFolca4 = AddRoom("Folca");
            armenelosGraph.Rooms[oFolca4] = new System.Windows.Point(6, 7);
            //CSRTODO: north (blocked)

            Room oWindfola4 = AddRoom("Windfola");
            AddBidirectionalExits(oEllessarWindfola, oWindfola4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola4] = new System.Windows.Point(8, 7);

            Room oOrithilHirgon = AddRoom("Orithil/Hirgon");
            AddBidirectionalExits(oHirgon4, oOrithilHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oOrithilHirgon] = new System.Windows.Point(0, 8);

            Room oOrithil1 = AddRoom("Orithil");
            AddBidirectionalExits(oOutdoorMarket, oOrithil1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithilHirgon, oOrithil1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil1] = new System.Windows.Point(1, 8);

            Room oOrithilRivel = AddRoom("Orithil/Rivel");
            AddBidirectionalExits(oRivel4, oOrithilRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil1, oOrithilRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilRivel] = new System.Windows.Point(2, 8);

            Room oOrithil2 = AddRoom("Orithil");
            AddBidirectionalExits(oOrithil1, oOrithil2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil2] = new System.Windows.Point(3, 8);

            Room oOrithil3 = AddRoom("Orithil");
            AddBidirectionalExits(oOrithil2, oOrithil3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil3] = new System.Windows.Point(4, 8);

            Room oOrithil4 = AddRoom("Orithil");
            AddBidirectionalExits(oOrithil3, oOrithil4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil4] = new System.Windows.Point(5, 8);

            Room oYurahtamJewelers = AddRoom("Yurahtam Jewlers");
            AddBidirectionalExitsWithOut(oOrithil4, oYurahtamJewelers, "south");
            armenelosGraph.Rooms[oYurahtamJewelers] = new System.Windows.Point(5, 8.5);

            Room oOrithilFolca = AddRoom("Orithil/Folca");
            AddBidirectionalExits(oFolca4, oOrithilFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil4, oOrithilFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilFolca] = new System.Windows.Point(6, 8);

            Room oOrithil5 = AddRoom("Orithil");
            AddBidirectionalExits(oOrithilFolca, oOrithil5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil5] = new System.Windows.Point(7, 8);
            //CSRTODO: archway (blocked)

            Room oOrithilWindfola = AddRoom("Orithil/Windfola");
            AddBidirectionalExits(oWindfola4, oOrithilWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil5, oOrithilWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilWindfola] = new System.Windows.Point(8, 8);

            Room oHirgon5 = AddRoom("Hirgon");
            AddBidirectionalExits(oOrithilHirgon, oHirgon5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon5] = new System.Windows.Point(0, 9);

            Room oStairwayLanding = AddRoom("Stairway Landing");
            AddExit(oHirgon5, oStairwayLanding, "stairway");
            AddExit(oStairwayLanding, oHirgon5, "down");
            armenelosGraph.Rooms[oStairwayLanding] = new System.Windows.Point(1, 9);

            Room oAmme = AddRoom("Amme");
            AddBidirectionalExitsWithOut(oStairwayLanding, oAmme, "doorway");
            armenelosGraph.Rooms[oAmme] = new System.Windows.Point(1, 8.5);

            Room oRivel5 = AddRoom("Rivel");
            AddBidirectionalExits(oOrithilRivel, oRivel5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel5] = new System.Windows.Point(2, 9);

            Room oFolca5 = AddRoom("Folca");
            AddBidirectionalExits(oOrithilFolca, oFolca5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca5] = new System.Windows.Point(6, 9);

            Room oWindfola5 = AddRoom("Windfola");
            AddBidirectionalExits(oOrithilWindfola, oWindfola5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola5] = new System.Windows.Point(8, 9);

            Room oBalanHirgon = AddRoom("Balan/Hirgon");
            AddBidirectionalExits(oHirgon5, oBalanHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oBalanHirgon] = new System.Windows.Point(0, 10);

            Room oBalan1 = AddRoom("Balan");
            AddBidirectionalExits(oBalanHirgon, oBalan1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan1] = new System.Windows.Point(1, 10);

            Room oBalanRivel = AddRoom("Balan/Rivel");
            AddBidirectionalExits(oRivel5, oBalanRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan1, oBalanRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanRivel] = new System.Windows.Point(2, 10);

            Room oBalan2 = AddRoom("Balan");
            AddBidirectionalExits(oBalanRivel, oBalan2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan2] = new System.Windows.Point(3, 10);

            Room oBalan3 = AddRoom("Balan");
            AddBidirectionalExits(oBalan2, oBalan3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan3] = new System.Windows.Point(4, 10);

            Room oBalan4 = AddRoom("Balan");
            AddBidirectionalExits(oBalan3, oBalan4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan4] = new System.Windows.Point(5, 10);

            Room oMerchantsMarket1 = AddRoom("Merchant Market");
            AddBidirectionalExits(oMerchantsMarket1, oBalan2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket1] = new System.Windows.Point(3, 9.5);

            Room oMerchantsMarket2 = AddRoom("Merchant Market");
            AddBidirectionalExits(oMerchantsMarket1, oMerchantsMarket2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMerchantsMarket2, oBalan3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket2] = new System.Windows.Point(4, 9.5);

            Room oMerchantsMarket3 = AddRoom("Merchant Market");
            AddBidirectionalExits(oMerchantsMarket2, oMerchantsMarket3, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMerchantsMarket3, oBalan4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket3] = new System.Windows.Point(5, 9.5);

            Room oBalanFolca = AddRoom("Balan/Folca");
            AddBidirectionalExits(oFolca5, oBalanFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan4, oBalanFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanFolca] = new System.Windows.Point(6, 10);

            Room oBalan5 = AddRoom("Balan");
            AddBidirectionalExits(oBalanFolca, oBalan5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan5] = new System.Windows.Point(7, 10);

            Room oBalanWindfola = AddRoom("Balan/Windfola");
            AddBidirectionalExits(oWindfola5, oBalanWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan5, oBalanWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanWindfola] = new System.Windows.Point(8, 10);

            Room oHirgon6 = AddRoom("Hirgon");
            AddBidirectionalExits(oBalanHirgon, oHirgon6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon6] = new System.Windows.Point(0, 11);

            Room oRivel6 = AddRoom("Rivel");
            AddBidirectionalExits(oBalanRivel, oRivel6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel6] = new System.Windows.Point(2, 11);

            Room oFolca6 = AddRoom("Folca");
            AddBidirectionalExits(oBalanFolca, oFolca6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca6] = new System.Windows.Point(6, 11);

            Room oWindfola6 = AddRoom("Windfola");
            AddBidirectionalExits(oBalanWindfola, oWindfola6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola6] = new System.Windows.Point(8, 11);

            Room oGoldberryHirgon = AddRoom("Goldberry/Hirgon");
            AddBidirectionalExits(oHirgon6, oGoldberryHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oGoldberryHirgon] = new System.Windows.Point(0, 12);

            Room oGoldberry1 = AddRoom("Goldberry");
            AddBidirectionalExits(oGoldberryHirgon, oGoldberry1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry1] = new System.Windows.Point(1, 12);

            Room oImrahil = AddRoom("Imrahil");
            AddBidirectionalSameNameExit(oGoldberry1, oImrahil, "swinging");
            armenelosGraph.Rooms[oImrahil] = new System.Windows.Point(1, 11);

            Room oGoldberryRivel = AddRoom("Goldberry/Rivel");
            AddBidirectionalExits(oRivel6, oGoldberryRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry1, oGoldberryRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberryRivel] = new System.Windows.Point(2, 12);

            Room oGoldberry2 = AddRoom("Goldberry");
            AddBidirectionalExits(oGoldberryRivel, oGoldberry2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry2] = new System.Windows.Point(3, 12);

            Room oGoldberry3 = AddRoom("Goldberry");
            AddBidirectionalExits(oGoldberry2, oGoldberry3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry3] = new System.Windows.Point(4, 12);

            Room oHummley = AddRoom("Hummley");
            AddBidirectionalExitsWithOut(oGoldberry3, oHummley, "doorway");
            armenelosGraph.Rooms[oHummley] = new System.Windows.Point(4, 11);

            Room oGoldberry4 = AddRoom("Goldberry");
            AddBidirectionalExits(oGoldberry3, oGoldberry4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry4] = new System.Windows.Point(5, 12);

            Room oGoldberryFolca = AddRoom("Goldberry/Folca");
            AddBidirectionalExits(oFolca6, oGoldberryFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry4, oGoldberryFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberryFolca] = new System.Windows.Point(6, 12);

            Room oGoldberry5 = AddRoom("Goldberry");
            AddBidirectionalExits(oGoldberryFolca, oGoldberry5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry5] = new System.Windows.Point(7, 12);

            Room oZain = AddRoom("Zain");
            AddBidirectionalExitsWithOut(oGoldberry5, oZain, "north");
            armenelosGraph.Rooms[oZain] = new System.Windows.Point(7, 11);

            Room oGateInside = AddRoom("Gate Inside");
            AddBidirectionalExits(oWindfola6, oGateInside, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry5, oGateInside, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGateInside] = new System.Windows.Point(8, 12);

            AddExit(oGateInside, oArmenelosGatesOutside, "gate");
            Exit e = AddExit(oArmenelosGatesOutside, oGateInside, "gate");
            e.RequiresDay = true;
            armenelosGraph.Rooms[oArmenelosGatesOutside] = new System.Windows.Point(8, 13);
        }

        private void AddWestOfNindamosAndArmenelos(Room oSouthernJunction, Room oPathThroughTheValley, out Room oEldemondeEastGateOutside)
        {
            Room r;
            Exit e;
            Room previousRoom = oSouthernJunction;
            for (int i = 0; i < 7; i++)
            {
                r = AddRoom("Laiquendi");
                AddBidirectionalExits(r, previousRoom, BidirectionalExitType.WestEast);
                previousRoom = r;
            }
            Room hiddenPathRoom = null;
            for (int i = 0; i < 9; i++)
            {
                r = AddRoom("Liara");
                e = AddExit(r, previousRoom, "south");
                if (i == 4)
                {
                    hiddenPathRoom = r;
                }
                AddExit(previousRoom, r, "north");
                previousRoom = r;
            }
            r = AddRoom("Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.SoutheastNorthwest);
            previousRoom = r;
            r = AddRoom("Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.SoutheastNorthwest);
            previousRoom = r;
            r = AddRoom("Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.NorthSouth);
            previousRoom = r;
            r = AddRoom("Liara");
            e = AddExit(r, previousRoom, "south");
            AddExit(previousRoom, r, "north");
            previousRoom = r;
            Room oLastLiara = r;

            Room oBaseOfMenelTarma = AddRoom("Base of Menel tarma");
            oBaseOfMenelTarma.AddPermanentMobs(MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oBaseOfMenelTarma, previousRoom, BidirectionalExitType.WestEast);

            Room oHiddenPath1 = AddRoom("Hidden Path");
            AddBidirectionalExits(hiddenPathRoom, oHiddenPath1, BidirectionalExitType.SoutheastNorthwest, true);
            Room oHiddenPath2 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath1, oHiddenPath2, BidirectionalExitType.SoutheastNorthwest, true);
            Room oHiddenPath3 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath2, oHiddenPath3, BidirectionalExitType.NorthSouth, true);
            Room oHiddenPath4 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath3, oHiddenPath4, BidirectionalExitType.NorthSouth, true);
            Room oHiddenPath5 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath4, oHiddenPath5, BidirectionalExitType.NorthSouth, true);
            Room oHiddenPath6 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath5, oHiddenPath6, BidirectionalExitType.NorthSouth, true);
            Room oHiddenPath7 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath6, oHiddenPath7, BidirectionalExitType.NorthSouth, true);
            Room oHiddenPath8 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath7, oHiddenPath8, BidirectionalExitType.NorthSouth, true);
            Room oHiddenPath9 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath8, oHiddenPath9, BidirectionalExitType.SoutheastNorthwest, true);
            Room oHiddenPath10 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath9, oHiddenPath10, BidirectionalExitType.WestEast, true);
            Room oHiddenPath11 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath10, oHiddenPath11, BidirectionalExitType.WestEast, true);
            Room oHiddenPath12 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath12, oHiddenPath11, BidirectionalExitType.SouthwestNortheast, true);
            Room oHiddenPath13 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath12, oHiddenPath13, BidirectionalExitType.WestEast, true);
            Room oHiddenPath14 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath13, oHiddenPath14, BidirectionalExitType.WestEast, true);
            Room oHiddenPath15 = AddRoom("Hidden Path");
            AddBidirectionalExits(oHiddenPath14, oHiddenPath15, BidirectionalExitType.WestEast, true);
            AddBidirectionalExits(oHiddenPath15, oPathThroughTheValley, BidirectionalExitType.SoutheastNorthwest, true);

            Room oGrasslands1 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oSouthernJunction, oGrasslands1, BidirectionalExitType.SouthwestNortheast);

            Room oGrasslands2 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands2, oGrasslands1, BidirectionalExitType.WestEast);

            Room oHostaEncampment = AddRoom("Hosta Encampment");
            AddBidirectionalExits(oHostaEncampment, oGrasslands2, BidirectionalExitType.SoutheastNorthwest);

            Room oGrasslands3 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands2, oGrasslands3, BidirectionalExitType.SouthwestNortheast);

            Room oGrasslands4 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands4, oGrasslands3, BidirectionalExitType.WestEast);

            Room oGrasslands5 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands5, oGrasslands4, BidirectionalExitType.NorthSouth);

            Room oGrasslands6 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands5, oGrasslands6, BidirectionalExitType.WestEast);

            Room oGrasslands7 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands6, oGrasslands7, BidirectionalExitType.NorthSouth);
            AddExit(oGrasslands7, oGrasslands3, "south");

            Room oGrasslands8 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands4, oGrasslands8, BidirectionalExitType.SouthwestNortheast);

            Room oGrasslands9 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands9, oGrasslands4, BidirectionalExitType.WestEast);
            e = AddExit(oGrasslands8, oGrasslands9, "north");
            AddExit(oGrasslands9, oGrasslands8, "north");

            Room oGrasslands10 = AddRoom("Mittalmar Grasslands");
            e = AddExit(oGrasslands4, oGrasslands10, "south");
            AddExit(oGrasslands10, oGrasslands4, "north");

            Room oGrasslands11 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands10, oGrasslands11, BidirectionalExitType.SouthwestNortheast);

            Room oGrasslands12 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands12, oGrasslands11, BidirectionalExitType.WestEast);

            Room oGrasslands13 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands13, oGrasslands12, BidirectionalExitType.NorthSouth);

            Room oGrasslands14 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands14, oGrasslands13, BidirectionalExitType.NorthSouth);

            Room oGrasslands15 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands15, oGrasslands14, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oGrasslands15, oGrasslands5, BidirectionalExitType.SoutheastNorthwest);

            Room oGrasslands16 = AddRoom("Mittalmar Grasslands");
            AddBidirectionalExits(oGrasslands16, oGrasslands13, BidirectionalExitType.SoutheastNorthwest);

            Room oDeathValleyEntrance = AddRoom("Death Valley Entrance");
            AddBidirectionalExits(oGrasslands16, oDeathValleyEntrance, BidirectionalExitType.NorthSouth);

            Room oGrassCoveredField1 = AddRoom("Grass Field");
            AddBidirectionalExits(oLastLiara, oGrassCoveredField1, BidirectionalExitType.SouthwestNortheast);

            Room oGrassCoveredField2 = AddRoom("Grass Field");
            AddBidirectionalExits(oGrassCoveredField2, oGrassCoveredField1, BidirectionalExitType.WestEast);

            Room oGrassCoveredField3 = AddRoom("Grass Field");
            AddBidirectionalExits(oGrassCoveredField3, oGrassCoveredField2, BidirectionalExitType.WestEast);

            Room oGrassCoveredField4 = AddRoom("Grass Field");
            AddBidirectionalExits(oGrassCoveredField4, oGrassCoveredField3, BidirectionalExitType.SoutheastNorthwest);

            Room oRiverPath1 = AddRoom("River Path");
            AddBidirectionalExits(oRiverPath1, oGrassCoveredField4, BidirectionalExitType.WestEast);

            Room oRiverPath2 = AddRoom("River Path");
            AddBidirectionalExits(oRiverPath2, oRiverPath1, BidirectionalExitType.WestEast);

            Room oRiverBank = AddRoom("River Bank");
            AddBidirectionalExits(oRiverBank, oRiverPath2, BidirectionalExitType.SoutheastNorthwest);

            Room oGrassCoveredField5 = AddRoom("Grass Field");
            AddBidirectionalExits(oGrassCoveredField3, oGrassCoveredField5, BidirectionalExitType.SouthwestNortheast);

            Room oGrassCoveredField6 = AddRoom("Grass Field");
            AddBidirectionalExits(oGrassCoveredField6, oGrassCoveredField5, BidirectionalExitType.WestEast);

            Room oEdgeOfNisimaldar = AddRoom("Nisimaldar Edge");
            AddBidirectionalExits(oGrassCoveredField6, oEdgeOfNisimaldar, BidirectionalExitType.SouthwestNortheast);

            Room oNisimaldar1 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar1, oEdgeOfNisimaldar, BidirectionalExitType.WestEast);

            Room oNisimaldar2 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar2, oNisimaldar1, BidirectionalExitType.WestEast);

            Room oNisimaldar3 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar3, oNisimaldar2, BidirectionalExitType.SoutheastNorthwest);

            Room oNisimaldar4 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar4, oNisimaldar3, BidirectionalExitType.WestEast);

            Room oNisimaldar5 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar4, oNisimaldar5, BidirectionalExitType.SouthwestNortheast);

            Room oNisimaldar6 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar6, oNisimaldar5, BidirectionalExitType.WestEast);

            Room oNisimaldar7 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar6, oNisimaldar7, BidirectionalExitType.SouthwestNortheast);

            Room oNisimaldar8 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar8, oNisimaldar7, BidirectionalExitType.WestEast);

            Room oNisimaldar9 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar9, oNisimaldar8, BidirectionalExitType.SoutheastNorthwest);

            Room oNisimaldar10 = AddRoom("Nisimaldar");
            AddBidirectionalExits(oNisimaldar10, oNisimaldar9, BidirectionalExitType.WestEast);

            oEldemondeEastGateOutside = AddRoom("East Gate Outside");
            AddBidirectionalExits(oEldemondeEastGateOutside, oNisimaldar10, BidirectionalExitType.SoutheastNorthwest);

            AddDeathValley(oDeathValleyEntrance);

            AddLocation(_aNindamosArmenelos, oBaseOfMenelTarma);
            AddLocation(_aNindamosArmenelos, oHostaEncampment);
            AddLocation(_aNindamosArmenelos, oDeathValleyEntrance);
            AddLocation(_aNindamosArmenelos, oEldemondeEastGateOutside);
        }

        private void AddDeathValley(Room oDeathValleyEntrance)
        {
            Room oDeathValleyWest1 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyWest1, oDeathValleyEntrance, BidirectionalExitType.WestEast);

            Room oDeathValleyWest2 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyWest2, oDeathValleyWest1, BidirectionalExitType.NorthSouth);

            Room oAmlug = AddRoom("Amlug");
            //CSRTODO
            //oAmlug.Mob1 = "Amlug";
            AddBidirectionalExitsWithOut(oDeathValleyWest2, oAmlug, "tomb");

            Room oDeathValleyWest3 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyWest3, oDeathValleyWest2, BidirectionalExitType.SouthwestNortheast);

            Room oDeathValleyWest4 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyWest4, oDeathValleyWest3, BidirectionalExitType.NorthSouth);

            Room oKallo = AddRoom("Kallo");
            //CSRTODO
            //oKallo.Mob1 = "Kallo";
            AddBidirectionalExitsWithOut(oDeathValleyWest4, oKallo, "tomb");

            Room oDeathValleyWest5 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyWest5, oDeathValleyWest4, BidirectionalExitType.SoutheastNorthwest);

            Room oDeathValleyWest6 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyWest6, oDeathValleyWest5, BidirectionalExitType.NorthSouth);

            Room oWizard = AddRoom("Wizard of the First Order");
            //CSRTODO: oWizard.Mob1 = "Wizard";
            AddBidirectionalExitsWithOut(oDeathValleyWest6, oWizard, "vault");

            Room oDeathValleyWest7 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyWest7, oDeathValleyWest6, BidirectionalExitType.SouthwestNortheast);
            //CSRTODO: doorway

            Room oDeathValleyEast1 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEntrance, oDeathValleyEast1, BidirectionalExitType.WestEast);

            Room oDeathValleyEast2 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEast2, oDeathValleyEast1, BidirectionalExitType.NorthSouth);

            Room oDeathValleyEast3 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEast3, oDeathValleyEast2, BidirectionalExitType.NorthSouth);

            Room oTranquilParkKaivo = AddHealingRoom("Kaivo", UNKNOWN_ROOM_NAME, HealingRoom.DeathValley);
            //CSRTODO
            //oTranquilParkKaivo.Mob1 = "Kaivo";
            AddBidirectionalExits(oTranquilParkKaivo, oDeathValleyEast3, BidirectionalExitType.WestEast);

            Room oDeathValleyEast4 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEast4, oDeathValleyEast3, BidirectionalExitType.SouthwestNortheast);

            Room oDeathValleyEast5 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEast4, oDeathValleyEast5, BidirectionalExitType.SoutheastNorthwest);

            Room oDeathValleyEast6 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEast6, oDeathValleyEast5, BidirectionalExitType.SouthwestNortheast);

            Room oDeathValleyEast7 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEast7, oDeathValleyEast6, BidirectionalExitType.WestEast);

            Room oDeathValleyEast8 = AddRoom("Death Valley");
            AddBidirectionalExits(oDeathValleyEast8, oDeathValleyEast7, BidirectionalExitType.WestEast);

            Room oStorageRoom = AddRoom("Storage Room");
            Exit e = AddExit(oDeathValleyEast8, oStorageRoom, "rocky");
            e.Hidden = true;
            AddExit(oStorageRoom, oDeathValleyEast8, "out");
        }

        private void AddEldemondeCity(Room oEldemondeEastGateOutside)
        {
            RoomGraph eldemondeGraph = new RoomGraph("Eldemonde");
            eldemondeGraph.ScalingFactor = 100;
            _graphs[MapType.Eldemonde] = eldemondeGraph;

            eldemondeGraph.Rooms[oEldemondeEastGateOutside] = new System.Windows.Point(10, 7);

            Room oEldemondeEastGateInside = AddRoom("East Gate Inside");
            AddBidirectionalSameNameExit(oEldemondeEastGateOutside, oEldemondeEastGateInside, "gate");
            eldemondeGraph.Rooms[oEldemondeEastGateInside] = new System.Windows.Point(9, 7);

            Room oCebe1 = AddRoom("Cebe");
            AddBidirectionalExits(oCebe1, oEldemondeEastGateInside, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCebe1] = new System.Windows.Point(9, 6);

            Room oDori1 = AddRoom("Dori");
            AddBidirectionalExits(oEldemondeEastGateInside, oDori1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oDori1] = new System.Windows.Point(9, 8);

            Room oCebe2 = AddRoom("Cebe");
            AddBidirectionalExits(oCebe2, oCebe1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe2] = new System.Windows.Point(8, 6);

            Room oElros2 = AddRoom("Elros");
            AddBidirectionalExits(oElros2, oEldemondeEastGateInside, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros2] = new System.Windows.Point(8, 7);

            Room oDori2 = AddRoom("Dori");
            AddBidirectionalExits(oDori2, oDori1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori2] = new System.Windows.Point(8, 8);

            Room oGuardHall = AddRoom("Guard Hall");
            AddBidirectionalExits(oGuardHall, oDori2, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oGuardHall] = new System.Windows.Point(8, 7.7);

            Room oBarracks = AddRoom("Barracks");
            AddBidirectionalExits(oBarracks, oGuardHall, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oBarracks] = new System.Windows.Point(7.6, 7.4);

            Room oGuardHQ = AddRoom("Guard HQ");
            AddBidirectionalExits(oGuardHall, oGuardHQ, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oGuardHQ] = new System.Windows.Point(8.4, 7.4);

            Room oCebe3 = AddRoom("Cebe");
            AddBidirectionalExits(oCebe3, oCebe2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe3] = new System.Windows.Point(7, 6);

            Room oTower = AddRoom("Tower");
            AddBidirectionalSameNameExit(oCebe3, oTower, "door");
            eldemondeGraph.Rooms[oTower] = new System.Windows.Point(7, 5.5);

            Room oElementsChamber = AddRoom("Elements Chamber");
            AddBidirectionalExits(oElementsChamber, oTower, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oElementsChamber] = new System.Windows.Point(8, 5.5);

            Room oGolemsChamber = AddRoom("Golems Chamber");
            AddBidirectionalExits(oGolemsChamber, oElementsChamber, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oGolemsChamber] = new System.Windows.Point(9, 5.5);

            Room oMorgatha = AddRoom("Morgatha");
            AddBidirectionalExits(oMorgatha, oGolemsChamber, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oMorgatha] = new System.Windows.Point(10, 5.5);

            Room oElros3 = AddRoom("Elros");
            AddBidirectionalExits(oElros3, oElros2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros3] = new System.Windows.Point(7, 7);

            Room oDori3 = AddRoom("Dori");
            AddBidirectionalExits(oDori3, oDori2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori3] = new System.Windows.Point(7, 8);

            Room oPostOffice = AddRoom("Post Office");
            AddBidirectionalExits(oPostOffice, oDori3, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oPostOffice] = new System.Windows.Point(7, 7.5);

            Room oIothCandol = AddRoom("Ioth/Candol");
            eldemondeGraph.Rooms[oIothCandol] = new System.Windows.Point(6, 4);

            Room oCandol1 = AddRoom("Candol");
            AddBidirectionalExits(oIothCandol, oCandol1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCandol1] = new System.Windows.Point(6, 5);

            Room oUniversityHall = AddRoom("University Hall");
            AddBidirectionalExits(oCandol1, oUniversityHall, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oUniversityHall] = new System.Windows.Point(7, 4);

            Room oUniversityHallSouth = AddRoom("University Hall");
            AddBidirectionalExits(oUniversityHall, oUniversityHallSouth, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oUniversityHallSouth] = new System.Windows.Point(7, 4.5);

            Room oUniversityHallSE = AddRoom("University Hall");
            AddBidirectionalExits(oUniversityHallSouth, oUniversityHallSE, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oUniversityHallSE] = new System.Windows.Point(8, 4.5);

            Room oAlchemy = AddRoom("Alchemy");
            AddBidirectionalSameNameExit(oUniversityHallSE, oAlchemy, "door");
            eldemondeGraph.Rooms[oAlchemy] = new System.Windows.Point(8, 5);

            Room oAurelius = AddRoom("Aurelius");
            AddBidirectionalSameNameExit(oUniversityHallSouth, oAurelius, "door");
            eldemondeGraph.Rooms[oAurelius] = new System.Windows.Point(7, 5);

            Room oUniversityHallNorth = AddRoom("University Hall");
            AddBidirectionalExits(oUniversityHallNorth, oUniversityHall, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oUniversityHallNorth] = new System.Windows.Point(7, 4);

            Room oMathemathics = AddRoom("Mathemathics");
            AddBidirectionalExitsWithOut(oUniversityHallNorth, oMathemathics, "door");
            eldemondeGraph.Rooms[oMathemathics] = new System.Windows.Point(8, 4);

            Room oCebeCandol = AddRoom("Cebe/Candol");
            AddBidirectionalExits(oCebeCandol, oCebe3, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCandol1, oCebeCandol, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCebeCandol] = new System.Windows.Point(6, 6);

            Room oElrosCandol = AddRoom("Elros/Candol");
            AddBidirectionalExits(oCebeCandol, oElrosCandol, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElrosCandol, oElros3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElrosCandol] = new System.Windows.Point(6, 7);

            Room oDoriCandol = AddRoom("Dori/Candol");
            AddBidirectionalExits(oElrosCandol, oDoriCandol, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDoriCandol, oDori3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDoriCandol] = new System.Windows.Point(6, 8);

            Room oIoth1 = AddRoom("Ioth");
            AddBidirectionalExits(oIoth1, oIothCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth1] = new System.Windows.Point(5, 4);

            Room oCebe4 = AddRoom("Cebe");
            AddBidirectionalExits(oCebe4, oCebeCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe4] = new System.Windows.Point(5, 6);

            Room oIsildur = AddRoom("Isildur");
            AddBidirectionalExitsWithOut(oCebe4, oIsildur, "shop");
            eldemondeGraph.Rooms[oIsildur] = new System.Windows.Point(5, 5.5);

            Room oElros4 = AddRoom("Elros");
            AddBidirectionalExits(oElros4, oElrosCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros4] = new System.Windows.Point(5, 7);

            Room oGate = AddRoom("Palace Gate");
            AddBidirectionalSameNameExit(oElros4, oGate, "gate");
            eldemondeGraph.Rooms[oGate] = new System.Windows.Point(4.6, 6.5);
            //CSRTODO: rest of palace

            Room oPalaceSouth = AddRoom("Palace");
            AddBidirectionalSameNameExit(oGate, oPalaceSouth, "stairway");
            eldemondeGraph.Rooms[oPalaceSouth] = new System.Windows.Point(4.6, 3);

            Room oPalaceSouthwest = AddRoom("Palace");
            AddBidirectionalExits(oPalaceSouthwest, oPalaceSouth, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oPalaceSouthwest] = new System.Windows.Point(3.6, 3);

            Room oFaeldor = AddRoom("Faeldor");
            AddBidirectionalSameNameExit(oPalaceSouthwest, oFaeldor, "door");
            eldemondeGraph.Rooms[oFaeldor] = new System.Windows.Point(2.6, 3);

            Room oPalaceSoutheast = AddRoom("Palace");
            AddBidirectionalExits(oPalaceSouth, oPalaceSoutheast, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oPalaceSoutheast] = new System.Windows.Point(5.6, 3);

            Room oGrimaxeGrimson = AddRoom("Grimaxe Grimson");
            AddBidirectionalSameNameExit(oPalaceSoutheast, oGrimaxeGrimson, "door");
            eldemondeGraph.Rooms[oGrimaxeGrimson] = new System.Windows.Point(6.6, 3);

            Room oMirrorHallEast = AddRoom("Mirror Hall");
            AddBidirectionalExits(oMirrorHallEast, oPalaceSoutheast, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallEast] = new System.Windows.Point(5.6, 2);

            Room oMirrorHallCenter = AddRoom("Mirror Hall");
            AddBidirectionalExits(oMirrorHallCenter, oMirrorHallEast, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMirrorHallCenter, oPalaceSouth, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallCenter] = new System.Windows.Point(4.6, 2);

            Room oMirrorHallWest = AddRoom("Mirror Hall");
            AddBidirectionalExits(oMirrorHallWest, oMirrorHallCenter, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMirrorHallWest, oPalaceSouthwest, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallWest] = new System.Windows.Point(3.6, 2);

            Room oThroneHallWest = AddRoom("Throne Hall");
            AddExit(oMirrorHallWest, oThroneHallWest, "hall");
            AddExit(oThroneHallWest, oMirrorHallWest, "south");
            eldemondeGraph.Rooms[oThroneHallWest] = new System.Windows.Point(3.6, 1);

            Room oThroneHallCenter = AddRoom("Throne Hall");
            AddBidirectionalExits(oThroneHallWest, oThroneHallCenter, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oThroneHallCenter] = new System.Windows.Point(4.6, 1);

            Room oThroneHallEast = AddRoom("Throne Hall");
            AddBidirectionalExits(oThroneHallCenter, oThroneHallEast, BidirectionalExitType.WestEast);
            AddExit(oMirrorHallEast, oThroneHallEast, "hall");
            AddExit(oThroneHallEast, oMirrorHallEast, "south");
            eldemondeGraph.Rooms[oThroneHallEast] = new System.Windows.Point(5.6, 1);

            Room oThroneHall = AddRoom("Throne Hall");
            AddBidirectionalSameNameExit(oThroneHallCenter, oThroneHall, "stairs");
            eldemondeGraph.Rooms[oThroneHall] = new System.Windows.Point(4.6, 0);

            Room oDori4 = AddRoom("Dori");
            AddBidirectionalExits(oDori4, oDoriCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori4] = new System.Windows.Point(5, 8);

            Room oIoth2 = AddRoom("Ioth");
            AddBidirectionalExits(oIoth2, oIoth1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth2] = new System.Windows.Point(4, 4);

            Room oCebe5 = AddRoom("Cebe");
            AddBidirectionalExits(oCebe5, oCebe4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe5] = new System.Windows.Point(4, 6);

            Room oSssreth = AddRoom("Sssreth");
            AddBidirectionalExitsWithOut(oCebe5, oSssreth, "south");
            eldemondeGraph.Rooms[oSssreth] = new System.Windows.Point(4, 6.5);

            Room oElros5 = AddRoom("Elros");
            AddBidirectionalExits(oElros5, oElros4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros5] = new System.Windows.Point(4, 7);

            Room oDori5 = AddRoom("Elros Statue");
            AddBidirectionalExits(oDori5, oDori4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori5] = new System.Windows.Point(4, 8);

            Room oIothNundine = AddRoom("North Gate Inside");
            AddBidirectionalExits(oIothNundine, oIoth2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIothNundine] = new System.Windows.Point(3, 4);

            Room oNundine1 = AddRoom("Nundine");
            AddBidirectionalExits(oIothNundine, oNundine1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oNundine1] = new System.Windows.Point(3, 5);

            Room oKegTavern = AddRoom("Keg Tavern");
            AddBidirectionalExitsWithOut(oNundine1, oKegTavern, "west");
            eldemondeGraph.Rooms[oKegTavern] = new System.Windows.Point(2, 5.3);

            Room oTavernKitchen = AddRoom("Tavern Kitchen");
            Exit e = AddExit(oKegTavern, oTavernKitchen, "door");
            e.Hidden = true;
            AddExit(oTavernKitchen, oKegTavern, "door");
            eldemondeGraph.Rooms[oTavernKitchen] = new System.Windows.Point(2, 4.7);

            Room oCebeNundine = AddRoom("Cebe/Nundine");
            AddBidirectionalExits(oNundine1, oCebeNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCebeNundine, oCebe5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebeNundine] = new System.Windows.Point(3, 6);

            Room oElrosNundine = AddRoom("Elros/Nundine");
            AddBidirectionalExits(oCebeNundine, oElrosNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElrosNundine, oElros5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElrosNundine] = new System.Windows.Point(3, 7);

            Room oDoriNundine = AddRoom("Dori/Nundine");
            AddBidirectionalExits(oElrosNundine, oDoriNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDoriNundine, oDori5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDoriNundine] = new System.Windows.Point(3, 8);

            Room oIoth3 = AddRoom("Ioth");
            AddBidirectionalExits(oIoth3, oIothNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth3] = new System.Windows.Point(2, 4);

            Room oCebe6 = AddRoom("Cebe");
            AddBidirectionalExits(oCebe6, oCebeNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe6] = new System.Windows.Point(2, 6);

            Room oElros6 = AddRoom("Wish Fountain");
            AddBidirectionalExits(oElros6, oElrosNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros6] = new System.Windows.Point(2, 7);

            Room oDori6 = AddRoom("Dark Lord Taunting");
            AddBidirectionalExits(oDori6, oDoriNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori6] = new System.Windows.Point(2, 8);

            Room oIoth4 = AddRoom("Ioth");
            AddBidirectionalExits(oIoth4, oIoth3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth4] = new System.Windows.Point(1, 4);

            Room oSmallPath = AddRoom("Small Path");
            e = AddExit(oIoth4, oSmallPath, "south");
            e.Hidden = true;
            e = AddExit(oSmallPath, oTavernKitchen, "backdoor");
            e.Hidden = true;
            AddExit(oSmallPath, oIoth4, "north");
            eldemondeGraph.Rooms[oSmallPath] = new System.Windows.Point(1, 5);

            Room oCebe7 = AddRoom("Cebe");
            AddBidirectionalExits(oSmallPath, oCebe7, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCebe7, oCebe6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe7] = new System.Windows.Point(1, 6);

            Room oBezanthi = AddRoom("Bezanthi");
            AddBidirectionalExitsWithOut(oCebe7, oBezanthi, "shop");
            eldemondeGraph.Rooms[oBezanthi] = new System.Windows.Point(0, 6);

            Room oElros7 = AddRoom("Elros");
            AddBidirectionalExits(oElros7, oElros6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros7] = new System.Windows.Point(1, 7);

            Room oDori7 = AddRoom("Dori");
            AddBidirectionalExits(oDori7, oDori6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori7] = new System.Windows.Point(1, 8);

            Room oElros8 = AddRoom("Elros");
            AddBidirectionalExits(oCebe7, oElros8, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oElros8, oElros7, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oElros8, oDori7, BidirectionalExitType.SoutheastNorthwest);
            eldemondeGraph.Rooms[oElros8] = new System.Windows.Point(0, 7);

            Room oCityWalkway1 = AddRoom("City Walkway");
            AddBidirectionalExits(oDori1, oCityWalkway1, BidirectionalExitType.SouthwestNortheast);
            eldemondeGraph.Rooms[oCityWalkway1] = new System.Windows.Point(8, 9);

            Room oCityWalkway2 = AddRoom("City Walkway");
            AddBidirectionalExits(oCityWalkway2, oCityWalkway1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway2] = new System.Windows.Point(6, 9);

            Room oCityWalkway3 = AddRoom("City Walkway");
            AddBidirectionalExits(oCityWalkway3, oCityWalkway2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway3] = new System.Windows.Point(4, 9);

            Room oCityWalkway4 = AddRoom("City Walkway");
            AddBidirectionalExits(oDori7, oCityWalkway4, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oCityWalkway4, oCityWalkway3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway4] = new System.Windows.Point(2, 9);
        }

        private Room AddRoom(string roomName)
        {
            return AddRoom(roomName, UNKNOWN_ROOM_NAME);
        }

        private Room AddRoom(string roomName, string backendName)
        {
            Room r = new Room(roomName, backendName);
            _map.AddVertex(r);
            if (backendName != UNKNOWN_ROOM_NAME)
            {
                if (AmbiguousRooms.TryGetValue(backendName, out List<Room> rooms))
                {
                    rooms.Add(r);
                }
                else if (UnambiguousRooms.TryGetValue(backendName, out Room existingRoom))
                {
                    UnambiguousRooms.Remove(backendName);
                    AmbiguousRooms[backendName] = new List<Room>() { existingRoom, r };
                }
                else
                {
                    UnambiguousRooms[backendName] = r;
                }
            }
            return r;
        }

        private Room AddHealingRoom(string roomName, string backendName, HealingRoom healingRoom)
        {
            Room r = AddRoom(roomName, backendName);
            r.HealingRoom = healingRoom;
            HealingRooms[healingRoom] = r;
            return r;
        }

        private void AddLocation(Area area, Room locRoom)
        {
            area.Locations.Add(locRoom);
        }

        private Exit AddExit(Room a, Room b, string exitText)
        {
            Exit e = new Exit(a, b, exitText);
            AddExit(e);
            return e;
        }

        private void AddExit(Exit exit)
        {
            exit.Source.Exits.Add(exit);
            _map.AddEdge(exit);
        }

        private void AddBidirectionalExitsWithOut(Room aRoom, Room bRoom, string inText)
        {
            AddExit(aRoom, bRoom, inText);
            AddExit(bRoom, aRoom, "out");
        }

        private void AddBidirectionalSameNameExit(Room aRoom, Room bRoom, string exitText)
        {
            AddBidirectionalSameNameExit(aRoom, bRoom, exitText, false);
        }

        private void AddBidirectionalSameNameExit(Room aRoom, Room bRoom, string exitText, bool mustOpen)
        {
            Exit e = new Exit(aRoom, bRoom, exitText);
            e.MustOpen = mustOpen;
            AddExit(e);
            e = new Exit(bRoom, aRoom, exitText);
            e.MustOpen = mustOpen;
            AddExit(e);
        }

        private void AddBidirectionalExits(Room aRoom, Room bRoom, BidirectionalExitType exitType)
        {
            AddBidirectionalExits(aRoom, bRoom, exitType, false);
        }

        private void AddBidirectionalExits(Room aRoom, Room bRoom, BidirectionalExitType exitType, bool hidden)
        {
            string exitAtoB = string.Empty;
            string exitBtoA = string.Empty;
            switch (exitType)
            {
                case BidirectionalExitType.WestEast:
                    exitAtoB = "east";
                    exitBtoA = "west";
                    break;
                case BidirectionalExitType.NorthSouth:
                    exitAtoB = "south";
                    exitBtoA = "north";
                    break;
                case BidirectionalExitType.SoutheastNorthwest:
                    exitAtoB = "southeast";
                    exitBtoA = "northwest";
                    break;
                case BidirectionalExitType.SouthwestNortheast:
                    exitAtoB = "southwest";
                    exitBtoA = "northeast";
                    break;
                case BidirectionalExitType.UpDown:
                    exitAtoB = "down";
                    exitBtoA = "up";
                    break;
                default:
                    throw new InvalidOperationException();
            }
            Exit e = new Exit(aRoom, bRoom, exitAtoB);
            e.Hidden = hidden;
            AddExit(e);
            e = new Exit(bRoom, aRoom, exitBtoA);
            e.Hidden = hidden;
            AddExit(e);
        }

        private Area AddArea(string areaName)
        {
            Area a = new Area(areaName);
            _areas.Add(a);
            _areasByName[a.Name] = a;
            return a;
        }
    }

    internal class Area
    {
        public Area(string name)
        {
            this.Name = name;
            this.Locations = new List<Room>();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string Name { get; set; }
        public List<Room> Locations { get; set; }
    }

    internal static class MapComputation
    {
        public static List<Exit> ComputeLowestCostPath(Room currentRoom, Room targetRoom, bool flying, bool levitating, bool isDay, int level)
        {
            if (currentRoom == null)
            {
                return null;
            }

            List<Exit> ret = null;
            Dictionary<Room, Exit> pathMapping = new Dictionary<Room, Exit>();
            GenericPriorityQueue<ExitPriorityNode, int> pq = new GenericPriorityQueue<ExitPriorityNode, int>(2000);

            pathMapping[currentRoom] = null;

            Func<Exit, bool> discriminator = (exit) =>
            {
                return !pathMapping.ContainsKey(exit.Target) && exit.ExitIsUsable(flying, levitating, isDay, level);
            };
            foreach (Exit e in IsengardMap.GetRoomExits(currentRoom, discriminator))
            {
                pq.Enqueue(new ExitPriorityNode(e), e.GetCost());
            }
            while (pq.Count > 0)
            {
                ExitPriorityNode nextNode = pq.Dequeue();
                Exit nextNodeExit = nextNode.Exit;
                Room nextNodeTarget = nextNodeExit.Target;
                if (!pathMapping.ContainsKey(nextNodeTarget))
                {
                    int iPriority = nextNode.Priority;
                    pathMapping[nextNodeTarget] = nextNodeExit;

                    if (nextNodeTarget == targetRoom)
                    {
                        Room tempRoom = targetRoom;
                        ret = new List<Exit>();
                        while (currentRoom != tempRoom)
                        {
                            Exit nextExit = pathMapping[tempRoom];
                            ret.Add(nextExit);
                            tempRoom = nextExit.Source;
                        }
                        ret.Reverse();
                        break;
                    }
                    else
                    {
                        foreach (Exit e in IsengardMap.GetRoomExits(nextNodeTarget, discriminator))
                        {
                            pq.Enqueue(new ExitPriorityNode(e), iPriority + e.GetCost());
                        }
                    }
                }
            }
            return ret;
        }
    }
}
