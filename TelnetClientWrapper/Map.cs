using Priority_Queue;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsengardClient
{
    internal class IsengardMap
    {
        internal AdjacencyGraph<Room, Exit> _map;
        private Dictionary<MapType, RoomGraph> _graphs;
        internal Dictionary<HealingRoom, Room> HealingRooms = new Dictionary<HealingRoom, Room>();
        internal Dictionary<PawnShoppe, Room> PawnShoppes = new Dictionary<PawnShoppe, Room>();
        internal Dictionary<Room, MapType> RoomsToMaps = new Dictionary<Room, MapType>();
        internal Dictionary<Room, List<MapType>> BoundaryPointsToMaps = new Dictionary<Room, List<MapType>>();

        /// <summary>
        /// maps room backend names to rooms when unambiguous, ambiguous rooms have a separate mapping.
        /// </summary>
        internal Dictionary<string, Room> UnambiguousRoomsByBackendName = new Dictionary<string, Room>();

        /// <summary>
        /// maps room backend names to rooms when ambiguous.
        /// </summary>
        internal Dictionary<string, List<Room>> AmbiguousRoomsByBackendName = new Dictionary<string, List<Room>>();

        /// <summary>
        /// maps room display names to rooms when unambiguous. for ambiguous room names the value is null.
        /// </summary>
        internal Dictionary<string, Room> UnambiguousRoomsByDisplayName = new Dictionary<string, Room>();

        public IsengardMap(List<string> errorMessages)
        {
            _graphs = new Dictionary<MapType, RoomGraph>();
            _map = new AdjacencyGraph<Room, Exit>();

            Type t = typeof(MapType);
            foreach (MapType nextMapType in Enum.GetValues(typeof(MapType)))
            {
                var memberInfos = t.GetMember(nextMapType.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == t);
                object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(MapTypeDisplayNameAttribute), false);
                RoomGraph rg = new RoomGraph(nextMapType, ((MapTypeDisplayNameAttribute)valueAttributes[0]).Name);
                rg.ScalingFactor = 100;
                _graphs[nextMapType] = rg;
            }

            AddBreeCity(out Room oConstructionSite, out Room oBreeTownSquare, out Room oBreeWestGateInside, out Room oSmoulderingVillage, out Room oNorthBridge, out Room oSewerPipeExit, out Room breeEastGateInside, out Room boatswain, out Room breeEastGateOutside, out Room oCemetery, out Room breeDocks, out Room accursedGuildHall, out Room crusaderGuildHall, out Room thievesGuildHall);
            AddMayorMillwoodMansion(oConstructionSite, _graphs[MapType.BreeStreets]);
            AddBreeToHobbiton(oBreeWestGateInside, oSmoulderingVillage);
            AddBreeToImladris(out Room oOuthouse, breeEastGateInside, breeEastGateOutside, out Room imladrisWestGateOutside, oCemetery);
            AddUnderBree(oNorthBridge, oOuthouse, oSewerPipeExit);
            AddImladrisCity(out Room oImladrisSouthGateInside, out Room oEastGateOfImladrisOutside, imladrisWestGateOutside, out Room healingHand, out Room oEastGateOfImladrisInside);
            AddEastOfImladris(oEastGateOfImladrisOutside, oEastGateOfImladrisInside, out Room westGateOfEsgaroth);
            AddImladrisToTharbad(oImladrisSouthGateInside, out Room oTharbadGateOutside);
            AddTharbadCity(oTharbadGateOutside, out Room tharbadWestGateOutside, out Room tharbadDocks, out Room tharbadEastGate);
            AddWestOfTharbad(tharbadWestGateOutside);
            AddEastOfTharbad(tharbadEastGate);
            AddEsgaroth(westGateOfEsgaroth, out Room esgarothNorthGateOutside);
            AddNorthOfEsgaroth(esgarothNorthGateOutside);
            AddNindamos(out Room oArmenelosGatesOutside, out Room oSouthernJunction, out Room oPathThroughTheValleyHiddenPath, out Room nindamosDocks, out RoomGraph nindamosGraph, out Room nindamosVillageCenter);
            AddArmenelos(oArmenelosGatesOutside);
            AddWestOfNindamosAndArmenelos(oSouthernJunction, oPathThroughTheValleyHiddenPath, out Room oEldemondeEastGateOutside, nindamosGraph);
            AddEldemondeCity(oEldemondeEastGateOutside);
            AddMithlond(breeDocks, boatswain, tharbadDocks, nindamosDocks, nindamosGraph);
            AddIntangible(oBreeTownSquare, healingHand, nindamosVillageCenter, accursedGuildHall, crusaderGuildHall, thievesGuildHall);

            Dictionary<Room, MapType> roomsWithoutExplicitMaps = new Dictionary<Room, MapType>();
            HashSet<Room> unmappedRooms = new HashSet<Room>();
            HashSet<Room> allRooms = new HashSet<Room>();
            HashSet<Room> boundaryPointsMissingValidation = new HashSet<Room>();
            Dictionary<Room, List<MapType>> roomMapsToCheck = new Dictionary<Room, List<MapType>>();
            foreach (var next in BoundaryPointsToMaps)
            {
                roomMapsToCheck[next.Key] = new List<MapType>(next.Value);
            }
            foreach (Room r in _map.Vertices)
            {
                unmappedRooms.Add(r);
                allRooms.Add(r);
            }
            foreach (var next in RoomsToMaps)
            {
                boundaryPointsMissingValidation.Add(next.Key);
            }
            foreach (KeyValuePair<MapType, RoomGraph> nextGraph in _graphs)
            {
                MapType mt = nextGraph.Key;
                RoomGraph g = nextGraph.Value;
                var oldRooms = g.Rooms;
                g.Rooms = new Dictionary<Room, System.Windows.Point>();
                foreach (KeyValuePair<Room, System.Windows.Point> next in oldRooms)
                {
                    Room r = next.Key;
                    if (roomMapsToCheck.TryGetValue(r, out List<MapType> mts))
                    {
                        if (mts.Contains(mt))
                        {
                            mts.Remove(mt);
                        }
                        if (mts.Count == 0)
                        {
                            roomMapsToCheck.Remove(r);
                        }
                    }
                    if (RoomsToMaps.TryGetValue(r, out MapType foundMap))
                    {
                        if (foundMap == mt)
                        {
                            boundaryPointsMissingValidation.Remove(r);
                        }
                    }
                    else
                    {
                        if (roomsWithoutExplicitMaps.ContainsKey(r))
                        {
                            errorMessages.Add("Duplicate room without map disambiguation: " + r.DisplayName + " - " + r.BackendName);
                        }
                        else
                        {
                            roomsWithoutExplicitMaps[r] = mt;
                        }
                    }
                    if (unmappedRooms.Contains(r))
                    {
                        unmappedRooms.Remove(r);
                    }
                    g.Rooms[r] = new System.Windows.Point(next.Value.X * g.ScalingFactor, next.Value.Y * g.ScalingFactor);
                }
            }
            foreach (var next in roomMapsToCheck)
            {
                Room r = next.Key;
                foreach (MapType mt in next.Value)
                {
                    errorMessages.Add("Boundary point missing from " + mt.ToString() + ": " + r.DisplayName + " (" + r.BackendName + ")");
                }
            }
            foreach (Room r in boundaryPointsMissingValidation)
            {
                errorMessages.Add("Disambiguated map room missing from map: " + r.DisplayName + " (" + r.BackendName + ")");
            }
            foreach (var nextMapping in roomsWithoutExplicitMaps)
            {
                RoomsToMaps[nextMapping.Key] = nextMapping.Value;
            }
            foreach (Room r in unmappedRooms)
            {
                errorMessages.Add("Unmapped room: " + r.DisplayName + " (" + r.BackendName + ")");
            }
        }

        /// <summary>
        /// retrieves the room text identifier. This is the backend name if unambiguous, and failing that the display name if unambiguous,
        /// and failing that it returns blank.
        /// </summary>
        /// <param name="room">room to check</param>
        /// <returns>identifier for the room, or blank if ambiguous</returns>
        public string GetRoomTextIdentifier(Room room)
        {
            string ret = string.Empty;
            if (UnambiguousRoomsByBackendName.TryGetValue(room.BackendName, out _))
            {
                ret = room.BackendName;
            }
            else if (UnambiguousRoomsByDisplayName.TryGetValue(room.DisplayName, out Room r) && r != null)
            {
                ret = room.DisplayName;
            }
            return ret;
        }

        /// <summary>
        /// retrieves a room by room text identifier. First it checks the backend name, and falling back on the display name, 
        /// and failing that it returns null.
        /// </summary>
        /// <param name="identifier">room text identifier</param>
        /// <returns>room if found, null if not found</returns>
        public Room GetRoomFromTextIdentifier(string identifier)
        {
            Room ret;
            if (!UnambiguousRoomsByBackendName.TryGetValue(identifier, out ret))
            {
                UnambiguousRoomsByDisplayName.TryGetValue(identifier, out ret);
            }
            return ret;
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
            RoomGraph tharbadEastGraph = _graphs[MapType.AlliskPlainsEastOfTharbad];

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
            RoomGraph tharbadWestGraph = _graphs[MapType.WestOfTharbad];
            RoomGraph tharbadGraph = _graphs[MapType.Tharbad];

            tharbadWestGraph.Rooms[tharbadWestGateOutside] = new System.Windows.Point(6, 5);

            Room lelionBeachAndPark = AddRoom("Lelion Beach and Park", "Lelion Beach and Park");
            AddBidirectionalSameNameExit(tharbadWestGateOutside, lelionBeachAndPark, "ramp");
            tharbadWestGraph.Rooms[lelionBeachAndPark] = new System.Windows.Point(5, 5);
            tharbadGraph.Rooms[lelionBeachAndPark] = new System.Windows.Point(-1, 7);
            AddMapBoundaryPoint(tharbadWestGateOutside, lelionBeachAndPark, MapType.Tharbad, MapType.WestOfTharbad);

            Room beachPath = AddRoom("Beach Path", "Beach Path");
            AddBidirectionalExits(lelionBeachAndPark, beachPath, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[beachPath] = new System.Windows.Point(5, 6);

            Room marinersAnchor = AddRoom("Mariner's Anchor", "The Mariner's Anchor");
            AddBidirectionalExitsWithOut(beachPath, marinersAnchor, "west");
            tharbadWestGraph.Rooms[marinersAnchor] = new System.Windows.Point(4, 6);

            Room dalePurves = AddRoom("Dale Purves", "Dale's Beach");
            dalePurves.AddPermanentMobs(MobTypeEnum.DalePurves);
            AddExit(marinersAnchor, dalePurves, "back door");
            AddExit(dalePurves, marinersAnchor, "east");
            tharbadWestGraph.Rooms[dalePurves] = new System.Windows.Point(3, 6);

            Room greyfloodRiver1 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver1.DamageType = RoomDamageType.Water;
            AddExit(dalePurves, greyfloodRiver1, "river");
            AddExit(greyfloodRiver1, dalePurves, "beach");
            tharbadWestGraph.Rooms[greyfloodRiver1] = new System.Windows.Point(2, 6);

            Room greyfloodRiver2 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver2.DamageType = RoomDamageType.Water;
            AddBidirectionalExits(greyfloodRiver1, greyfloodRiver2, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[greyfloodRiver2] = new System.Windows.Point(2, 7);

            Room greyfloodRiver3 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver3.DamageType = RoomDamageType.Water;
            AddBidirectionalExits(greyfloodRiver2, greyfloodRiver3, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[greyfloodRiver3] = new System.Windows.Point(2, 8);

            Room riverMouth = AddRoom("River Mouth", "The Mouth of the Greyflood River");
            riverMouth.DamageType = RoomDamageType.Water;
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
            e = AddBidirectionalExitsWithOut(oNorthLookoutTowerCellar, oShroudedTunnel, "shroud");
            e.Hidden = true;
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

        private void AddMithlond(Room breeDocks, Room boatswain, Room tharbadDocks, Room nindamosDocks, RoomGraph nindamosGraph)
        {
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];

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
            oMithlondPort.AddPermanentItems(ItemTypeEnum.PortManifest);
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

            AddHarbringer(oHarbringerGangplank, tharbadDocks);
            AddBullroarer(oBullroarerSlip, nindamosDocks);
            AddCelduinExpress(boatswain, breeDocks);
        }

        private void AddCelduinExpress(Room boatswain, Room breeDocks)
        {
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];

            mithlondGraph.Rooms[boatswain] = new System.Windows.Point(1, 4);
            mithlondGraph.Rooms[breeDocks] = new System.Windows.Point(1, 3.5);

            Room oBeneathBridge = AddRoom("Under Bridge", "Beneath the Bridge of the Celduin Express");
            mithlondGraph.Rooms[oBeneathBridge] = new System.Windows.Point(-0.5, 4.5);
            AddBidirectionalExits(boatswain, oBeneathBridge, BidirectionalExitType.SouthwestNortheast);

            Room oBridge = AddRoom("Capt. Felagund", "Bridge of the Celduin Express");
            oBridge.AddPermanentMobs(MobTypeEnum.CaptainFelagund);
            Exit e = AddExit(oBeneathBridge, oBridge, "hatchway");
            e.KeyType = ItemTypeEnum.BridgeKey;
            e.MustOpen = true;
            e = AddExit(oBridge, oBeneathBridge, "down");
            e.MustOpen = true;
            mithlondGraph.Rooms[oBridge] = new System.Windows.Point(-0.5, 4.25);

            Room oUnderDeck = AddRoom("Under Deck", "Beneath the Deck of the Celduin Express");
            AddBidirectionalSameNameExit(oBeneathBridge, oUnderDeck, "stairway");
            mithlondGraph.Rooms[oUnderDeck] = new System.Windows.Point(-1, 5.5);

            Room oPassengerLounge = AddRoom("Passenger Lounge", "Passenger Lounge");
            AddBidirectionalExits(oUnderDeck, oPassengerLounge, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oPassengerLounge] = new System.Windows.Point(-1, 5.75);

            Room oBoilerRoom = AddRoom("Boiler Room", "Boiler Room");
            e = AddExit(oUnderDeck, oBoilerRoom, "door");
            e.KeyType = ItemTypeEnum.BoilerKey;
            e.MustOpen = true;
            e = AddExit(oBoilerRoom, oUnderDeck, "door");
            e.MustOpen = true;
            mithlondGraph.Rooms[oBoilerRoom] = new System.Windows.Point(0, 5.5);

            Room oCelduinExpressNW = AddRoom("Stern", "Stern of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressNW, boatswain, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCelduinExpressNW, oBeneathBridge, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oCelduinExpressNW] = new System.Windows.Point(-2, 4);

            Room oCelduinExpressMainDeckW = AddRoom("Main Deck", "Main Deck of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressNW, oCelduinExpressMainDeckW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBeneathBridge, oCelduinExpressMainDeckW, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oCelduinExpressMainDeckW] = new System.Windows.Point(-2, 6);

            Room oCelduinExpressMainDeckE = AddRoom("Main Deck", "Main Deck of the Celduin Express");
            AddBidirectionalExits(boatswain, oCelduinExpressMainDeckE, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBeneathBridge, oCelduinExpressMainDeckE, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oCelduinExpressMainDeckW, oCelduinExpressMainDeckE, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oCelduinExpressMainDeckE] = new System.Windows.Point(1, 6);

            Room oCelduinExpressBow = AddRoom("Bow", "Bow of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressMainDeckE, oCelduinExpressBow, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oCelduinExpressMainDeckW, oCelduinExpressBow, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oCelduinExpressBow] = new System.Windows.Point(-0.5, 7);
        }

        /// <summary>
        /// harbringer allows travel from Tharbad to Mithlond (but not the reverse?)
        /// </summary>
        private void AddHarbringer(Room mithlondEntrance, Room tharbadDocks)
        {
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];
            RoomGraph tharbadGraph = _graphs[MapType.Tharbad];

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
            mithlondGraph.Rooms[tharbadDocks] = new System.Windows.Point(3, 6.5);
            tharbadGraph.Rooms[oHarbringerMithlondEntrance] = new System.Windows.Point(0, 9);
            AddMapBoundaryPoint(tharbadDocks, oHarbringerMithlondEntrance, MapType.Tharbad, MapType.Mithlond);

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

            Room oShipsHoldSE = AddRoom("Ship's Hold", "Ship's Hold");
            AddExit(oHarbringerEast3, oShipsHoldSE, "hatch");
            AddExit(oShipsHoldSE, oHarbringerEast3, "up");
            mithlondGraph.Rooms[oShipsHoldSE] = new System.Windows.Point(7, 7.5);

            Room oShipsHoldS = AddRoom("Ship's Hold", "Ship's Hold");
            AddBidirectionalExits(oShipsHoldSE, oShipsHoldS, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oShipsHoldS] = new System.Windows.Point(6.5, 8);

            Room oShipsHoldSW = AddRoom("Ship's Hold", "Ship's Hold");
            AddBidirectionalExits(oShipsHoldSW, oShipsHoldS, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oShipsHoldSW] = new System.Windows.Point(6, 7.5);

            Room oBrig = AddRoom("Brig", "Brig");
            oBrig.AddPermanentMobs(MobTypeEnum.Smee);
            AddExit(oShipsHoldSW, oBrig, "door");
            mithlondGraph.Rooms[oBrig] = new System.Windows.Point(6, 6.5);

            Room oShipsHoldNE = AddRoom("Ship's Hold", "Ship's Hold");
            AddBidirectionalExits(oShipsHoldNE, oShipsHoldSE, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oShipsHoldNE] = new System.Windows.Point(7, 7);

            Room oRandsQuarters = AddRoom("Rand's Quarters", "Rand's Quarters");
            e = AddExit(oShipsHoldNE, oRandsQuarters, "door");
            e.MustOpen = true;
            e = AddExit(oRandsQuarters, oShipsHoldNE, "door");
            e.MustOpen = true;
            mithlondGraph.Rooms[oRandsQuarters] = new System.Windows.Point(7, 6.5);
        }

        private void AddBullroarer(Room mithlondEntrance, Room nindamosDocks)
        {
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];
            RoomGraph nindamosGraph = _graphs[MapType.Nindamos];

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
            mithlondGraph.Rooms[nindamosDocks] = new System.Windows.Point(6, 5);
            AddMapBoundaryPoint(nindamosDocks, bullroarerSE, MapType.Nindamos, MapType.Mithlond);

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
            fishHold.DamageType = RoomDamageType.Wind;
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

        public Dictionary<MapType, RoomGraph> Graphs
        {
            get
            {
                return _graphs;
            }
        }

        private void AddTharbadCity(Room oTharbadGateOutside, out Room tharbadWestGateOutside, out Room tharbadDocks, out Room tharbadEastGate)
        {
            RoomGraph tharbadGraph = _graphs[MapType.Tharbad];
            RoomGraph eastOfTharbadGraph = _graphs[MapType.AlliskPlainsEastOfTharbad];

            tharbadGraph.Rooms[oTharbadGateOutside] = new System.Windows.Point(3, 0.5);

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
            eastOfTharbadGraph.Rooms[sabreEvard] = new System.Windows.Point(-1, 4);

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
            e = AddBidirectionalExitsWithOut(bardicGuildhall, oGuildmasterAnsette, "door");
            e.Hidden = true;
            e.RequiresDay = true;
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

            Room marketTrinkets = AddPawnShoppeRoom("Market Trinkets", "Market District - Trinkets and Baubles", PawnShoppe.Tharbad);
            AddBidirectionalExits(marketBeast, marketTrinkets, BidirectionalExitType.WestEast);
            AddBidirectionalExits(marketTrinkets, oMasterJeweler, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketTrinkets] = new System.Windows.Point(6, 5);

            Room oEntranceToGypsyEncampment = AddRoom("Gypsy Encampment", "Entrance to Gypsy Encampment");
            e = AddExit(oMasterJeweler, oEntranceToGypsyEncampment, "row");
            e.MaximumLevel = 13;
            AddExit(oEntranceToGypsyEncampment, oMasterJeweler, "market");
            tharbadGraph.Rooms[oEntranceToGypsyEncampment] = new System.Windows.Point(7, 6);

            Room oGypsyRow1 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oEntranceToGypsyEncampment, oGypsyRow1, BidirectionalExitType.WestEast);
            e = AddExit(alley, oGypsyRow1, "north");
            e.Hidden = true;
            e.MaximumLevel = 13;
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
            AddMapBoundaryPoint(sabreEvard, tharbadEastGate, MapType.Tharbad, MapType.AlliskPlainsEastOfTharbad);
        }

        private void AddBreeCity(out Room oConstructionSite, out Room oBreeTownSquare, out Room oWestGateInside, out Room oSmoulderingVillage, out Room oNorthBridge, out Room oSewerPipeExit, out Room breeEastGateInside, out Room boatswain, out Room breeEastGateOutside, out Room oCemetery, out Room breeDocks, out Room accursedGuildHall, out Room crusaderGuildHall, out Room thievesGuildHall)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            //Bree's road structure is a 15x11 grid
            Room[,] breeStreets = new Room[16, 11];
            Room[,] breeSewers = new Room[16, 11];
            breeStreets[0, 0] = AddRoom("Thalion/Wain", "Wain Road South/Thalion Road Intersection"); //1x1
            breeStreets[1, 0] = AddRoom("Thalion", "Thalion Road"); //2x1
            breeStreets[1, 0].AddPermanentMobs(MobTypeEnum.SeasonedVeteran);
            breeStreets[2, 0] = AddRoom("Thalion", "Thalion Road"); //3x1
            breeStreets[3, 0] = AddRoom("Thalion/High", "Thalion Road/South High Street Intersection"); //4x1
            breeStreets[4, 0] = AddRoom("Thalion", "Thalion Road"); //5x1
            breeStreets[5, 0] = AddRoom("Thalion", "Thalion Road"); //6x1
            breeStreets[6, 0] = AddRoom("Thalion", "Thalion Road"); //7x1
            breeStreets[7, 0] = AddRoom("Thalion/Main", "Main Street/Thalion Road Intersection"); //8x1
            breeDocks = breeStreets[9, 0] = AddRoom("Docks", "Bree Docks"); //10x1
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
            breeStreets[1, 3].AddPermanentMobs(MobTypeEnum.SeasonedVeteran);
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
            breeStreets[8, 3].AddPermanentMobs(MobTypeEnum.SeasonedVeteran);
            breeStreets[9, 3] = AddRoom("South Bridge", "South Bridge"); //10x4
            breeStreets[10, 3] = AddRoom("Periwinkle/Crissaegrim", "Periwinkle Road/Crissaegrim Road Intersection"); //11x4
            breeStreets[11, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //12x4
            Room oPeriwinklePoorAlley = breeStreets[12, 3] = AddRoom("Periwinkle/PoorAlley", "Periwinkle Road/Poor Alley Intersection"); //13x4
            Room oToSmithy = breeStreets[13, 3] = AddRoom("Periwinkle", "Periwinkle Road"); //14x4
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
            Room oToMagicShop = breeStreets[10, 5] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x6
            breeStreets[14, 5] = AddRoom("Brownhaven", "Brownhaven Road"); //15x6
            breeStreets[0, 6] = AddRoom("Wain", "Wain Road South"); //1x7
            breeSewers[0, 6] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x7
            breeStreets[3, 6] = AddRoom("High", "South High Street"); //4x7
            breeStreets[7, 6] = AddRoom("Main", "Main Street"); //8x7
            breeStreets[10, 6] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x7
            breeStreets[14, 6] = AddRoom("Brownhaven", "Brownhaven Road"); //15x7
            oWestGateInside = breeStreets[0, 7] = AddRoom("West Gate Inside", "West Gate of Bree"); //1x8
            oWestGateInside.AddPermanentItems(ItemTypeEnum.GateWarning);
            breeSewers[0, 7] = AddRoom("Sewers West Gate", "Wain Road/Leviathan Way Sewer Main"); //1x8
            AddExit(breeSewers[0, 7], oWestGateInside, "up");
            breeStreets[1, 7] = AddRoom("Leviathan", "Leviathan Way"); //2x8
            Room oHauntedMansionEntrance = breeStreets[2, 7] = AddRoom("Leviathan", "Leviathan Way"); //3x8
            breeStreets[3, 7] = AddRoom("Leviathan/High", "Leviathan Way/High Street"); //4x8
            breeStreets[3, 7].AddPermanentMobs(MobTypeEnum.SeasonedVeteran);
            breeStreets[4, 7] = AddRoom("Leviathan", "Leviathan Way"); //5x8
            oBreeTownSquare = breeStreets[5, 7] = AddRoom("Town Square", "Bree Town Square"); //6x8
            oBreeTownSquare.AddPermanentMobs(MobTypeEnum.TheTownCrier, MobTypeEnum.Scribe, MobTypeEnum.SmallSpider, MobTypeEnum.Vagrant);
            oBreeTownSquare.AddPermanentItems(ItemTypeEnum.WelcomeSign);
            breeStreets[6, 7] = AddRoom("Leviathan", "Leviathan Way"); //7x8
            breeStreets[7, 7] = AddRoom("Leviathan/Main", "Leviathan Way/Main Street"); //8x8
            breeStreets[8, 7] = AddRoom("Leviathan", "Leviathan Way"); //9x8
            oNorthBridge = breeStreets[9, 7] = AddRoom("North Bridge", "North Bridge"); //10x8
            breeStreets[10, 7] = AddRoom("Leviathan/Crissaegrim", "Leviathan Way/Crissaegrim Road"); //11x8
            breeStreets[11, 7] = AddRoom("Leviathan", "Leviathan Way"); //12x8
            Room oLeviathanPoorAlley = breeStreets[12, 7] = AddRoom("Leviathan", "Leviathan Way"); //13x8
            Room oToGrantsStables = breeStreets[13, 7] = AddRoom("Leviathan", "Leviathan Way"); //14x8
            breeEastGateInside = breeStreets[14, 7] = AddRoom("East Gate Inside", "Bree's East Gate"); //15x8
            breeEastGateInside.AddPermanentItems(ItemTypeEnum.GateWarning);
            breeStreets[0, 8] = AddRoom("Wain", "Wain Road North"); //1x9
            breeSewers[0, 8] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x9
            breeStreets[3, 8] = AddRoom("High", "North High Street"); //4x9
            breeStreets[7, 8] = AddRoom("Main", "Main Street"); //8x9
            breeStreets[10, 8] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x9
            breeStreets[14, 8] = AddRoom("Brownhaven", "Brownhaven Road"); //15x9
            Room orderOfLove = breeStreets[15, 8] = AddHealingRoom("Order of Love", "Order of Love", HealingRoom.BreeNortheast); //16x9
            orderOfLove.AddNonPermanentMobs(MobTypeEnum.Drunk, MobTypeEnum.HobbitishDoctor, MobTypeEnum.Hobbit, MobTypeEnum.LittleMouse);
            breeStreets[0, 9] = AddRoom("Wain", "Wain Road North"); //1x10
            breeSewers[0, 9] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x10
            breeStreets[3, 9] = AddRoom("High", "North High Street"); //4x10
            Room oToGuildHall = breeStreets[7, 9] = AddRoom("Main", "Main Street"); //8x10
            Room oToLeonardosFoundry = breeStreets[10, 9] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x10
            Room oToGamblingPit = breeStreets[14, 9] = AddRoom("Brownhaven", "Brownhaven Road"); //15x10
            breeStreets[0, 10] = AddRoom("Ormenel/Wain", "Wain Road North/Ormenel Street Intersection"); //1x11
            breeSewers[0, 10] = AddRoom("Sewers Ormenel/Wain", "Wain Road Sewer Main"); //1x11
            Exit e = AddExit(breeStreets[0, 10], breeSewers[0, 10], "sewer");
            e.Hidden = true;
            e.MustOpen = true;
            e.MinimumLevel = 4;
            AddExit(breeSewers[0, 10], breeStreets[0, 10], "up");
            AddMapBoundaryPoint(breeStreets[0, 10], breeSewers[0, 10], MapType.BreeStreets, MapType.BreeSewers);
            breeStreets[1, 10] = AddRoom("Ormenel", "Ormenel Street"); //2x11
            Room oToZoo = breeStreets[2, 10] = AddRoom("Ormenel", "Ormenel Street"); //3x11
            breeStreets[3, 10] = AddRoom("Ormenel/High", "North High Street/Ormenel Street Intersection"); //4x11
            Room oToCasino = breeStreets[4, 10] = AddRoom("Ormenel", "Ormenel Street"); //5x11
            oToCasino.AddPermanentMobs(MobTypeEnum.SeasonedVeteran);
            Room oToArena = breeStreets[5, 10] = AddRoom("Ormenel", "Ormenel Street"); //6x11
            breeStreets[6, 10] = AddRoom("Ormenel", "Ormenel Street"); //7x11
            breeStreets[7, 10] = AddRoom("Ormenel/Main", "Main Street/Ormenel Street Intersection"); //8x11
            breeStreets[10, 10] = AddRoom("Ormenel/Crissaegrim", "Crissaegrim Road/Ormenel Street Intersection"); //11x11
            Room oToRealEstateOffice = breeStreets[11, 10] = AddRoom("Ormenel", "Ormenel Street"); //12x11
            breeStreets[12, 10] = AddRoom("Ormenel", "Ormenel Street"); //13x11
            Room oStreetToFallon = breeStreets[13, 10] = AddRoom("Ormenel", "Ormenel Street"); //14x11
            breeStreets[14, 10] = AddRoom("Brownhaven/Ormenel", "Brownhaven Road/Ormenel Street Intersection"); //15x11

            AddBreeSewers(breeStreets, breeSewers, out oSmoulderingVillage);

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
            breeStreetsGraph.Rooms[oPoorAlley1] = new System.Windows.Point(12, 4);

            Room oPoorAlley2 = AddRoom("Poor Alley", "Poor Alley");
            AddBidirectionalExits(oPoorAlley1, oPoorAlley2, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oPoorAlley2] = new System.Windows.Point(12, 5);

            Room oPoorAlley3 = AddRoom("Poor Alley", "Poor Alley");
            AddBidirectionalExits(oPoorAlley2, oPoorAlley3, BidirectionalExitType.NorthSouth);
            AddExit(oPeriwinklePoorAlley, oPoorAlley3, "alley");
            AddExit(oPoorAlley3, oPeriwinklePoorAlley, "south");
            breeStreetsGraph.Rooms[oPoorAlley3] = new System.Windows.Point(12, 6);

            Room oMensClub = AddRoom("Men's Club", "Men's Club");
            AddBidirectionalExits(oMensClub, oPoorAlley3, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oMensClub] = new System.Windows.Point(11, 6);

            Room oCampusFreeClinic = AddHealingRoom("Bree Campus Free Clinic", "Campus Free Clinic", HealingRoom.BreeSouthwest);
            oCampusFreeClinic.AddNonPermanentMobs(MobTypeEnum.Student);
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");
            breeStreetsGraph.Rooms[oCampusFreeClinic] = new System.Windows.Point(4, 9);

            Room oBreeRealEstateOffice = AddRoom("Real Estate Office", "Bree Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oBreeRealEstateOffice] = new System.Windows.Point(11, 0.3);

            Room oIxell = AddRoom("Ixell", "Kista Hills Show Home");
            oIxell.AddPermanentMobs(MobTypeEnum.IxellDeSantis);
            AddBidirectionalExitsWithOut(oBreeRealEstateOffice, oIxell, "door");
            breeStreetsGraph.Rooms[oIxell] = new System.Windows.Point(11, 0.6);

            Room oApartmentComplex = AddRoom("Apartment Complex", "Kista Hills Apartment Complex Entrance");
            AddBidirectionalExitsWithOut(oIxell, oApartmentComplex, "apartments");
            breeStreetsGraph.Rooms[oApartmentComplex] = new System.Windows.Point(12, 0.6);

            Room oApartmentComplexHallway = AddRoom("Hallway", "Kista Hills Apartment Complex Hallway");
            oApartmentComplexHallway.AddPermanentItems(ItemTypeEnum.StorageSign);
            AddBidirectionalExitsWithOut(oApartmentComplex, oApartmentComplexHallway, "hallway");
            breeStreetsGraph.Rooms[oApartmentComplexHallway] = new System.Windows.Point(13, 0.6);

            Room oApartmentComplexPlaza = AddRoom("Plaza", "Kista Hills Apartment Complex Plaza");
            oApartmentComplexPlaza.AddPermanentItems(ItemTypeEnum.StorageSign);
            AddBidirectionalExitsWithOut(oApartmentComplex, oApartmentComplexPlaza, "plaza");
            breeStreetsGraph.Rooms[oApartmentComplexPlaza] = new System.Windows.Point(12, 1.6);

            oConstructionSite = AddRoom("Construction Site", "Construction Site");
            AddExit(oIxell, oConstructionSite, "back door");
            AddExit(oConstructionSite, oIxell, "hoist");
            breeStreetsGraph.Rooms[oConstructionSite] = new System.Windows.Point(11, 0.9);

            Room oKistaHillsHousing = AddRoom("Kista Hills Housing", "Kista Hills Housing");
            AddBidirectionalExits(oStreetToFallon, oKistaHillsHousing, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oKistaHillsHousing] = new System.Windows.Point(13, -0.5);

            Room oChurchsEnglishGarden = AddRoom("Chuch's English Garden", "Church's English Garden");
            AddBidirectionalSameNameExit(oKistaHillsHousing, oChurchsEnglishGarden, "gate");
            Room oFallon = AddRoom("Fallon", "The Home of Church, the Cleric");
            oFallon.AddPermanentMobs(MobTypeEnum.Fallon);
            AddBidirectionalExitsWithOut(oChurchsEnglishGarden, oFallon, "door");
            breeStreetsGraph.Rooms[oChurchsEnglishGarden] = new System.Windows.Point(13, -1);
            breeStreetsGraph.Rooms[oFallon] = new System.Windows.Point(13, -1.5);

            Room oGrantsStables = AddRoom("Grant's stables", "Grant's stables");
            e = AddExit(oToGrantsStables, oGrantsStables, "stable");
            e.MaximumLevel = 10;
            AddExit(oGrantsStables, oToGrantsStables, "south");
            breeStreetsGraph.Rooms[oGrantsStables] = new System.Windows.Point(13, 2.5);

            Room oGrant = AddRoom("Grant", "Grant's Office");
            oGrant.AddPermanentMobs(MobTypeEnum.Grant);
            AddBidirectionalExitsWithOut(oGrantsStables, oGrant, "gate", true);
            breeStreetsGraph.Rooms[oGrant] = new System.Windows.Point(13, 2);

            Room oDTansLeatherArmory = AddRoom("Leather Armory", "D'Tan's Leather Armory");
            AddExit(oToGrantsStables, oDTansLeatherArmory, "armory");
            AddExit(oDTansLeatherArmory, oToGrantsStables, "north");
            breeStreetsGraph.Rooms[oDTansLeatherArmory] = new System.Windows.Point(13, 3.5);

            Room oPansy = AddRoom("Pansy Smallburrows", "Gambling Pit");
            oPansy.AddPermanentMobs(MobTypeEnum.PansySmallburrows);
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oPansy] = new System.Windows.Point(13, 1);

            Room oIgor = AddRoom("Igor", "Blind Pig Pub");
            oIgor.AddPermanentMobs(MobTypeEnum.IgorTheBouncer);
            AddExit(oIgor, oToBlindPigPubAndUniversity, "east");
            AddExit(oToBlindPigPubAndUniversity, oIgor, "pub");
            breeStreetsGraph.Rooms[oIgor] = new System.Windows.Point(2, 5.5);

            Room oUniversityEntrance = AddRoom("University", "University of Bree");
            AddExit(oToBlindPigPubAndUniversity, oUniversityEntrance, "university");
            AddExit(oUniversityEntrance, oToBlindPigPubAndUniversity, "west");
            breeStreetsGraph.Rooms[breeStreets[3, 4]] = new System.Windows.Point(3, 5.5);
            breeStreetsGraph.Rooms[oUniversityEntrance] = new System.Windows.Point(3.5, 5.5);

            Room oTulkasBookstore = AddRoom("Tulkas Bookstore", "Tulkas Memorial Bookstore");
            AddBidirectionalExits(oUniversityEntrance, oTulkasBookstore, BidirectionalExitType.SoutheastNorthwest);
            breeStreetsGraph.Rooms[oTulkasBookstore] = new System.Windows.Point(3.75, 6.5);

            Room oTulkasLibrary = AddRoom("Tulkas Library", "Tulkas Memorial Library");
            oTulkasLibrary.AddPermanentMobs(MobTypeEnum.VeristriaTheLibrarian);
            AddBidirectionalExits(oTulkasBookstore, oTulkasLibrary, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oTulkasLibrary] = new System.Windows.Point(4.75, 6.5);

            Room oFoyer = AddRoom("Foyer", "Foyer of the Iluvatar Research Building");
            e = AddExit(oUniversityEntrance, oFoyer, "east");
            e.MaximumLevel = 12;
            AddExit(oFoyer, oUniversityEntrance, "west");
            breeStreetsGraph.Rooms[oFoyer] = new System.Windows.Point(4.25, 5.5);

            Room oInstructionalHall = AddRoom("Instruction", "Iluvatar Memorial Instructional Hall");
            AddBidirectionalExits(oFoyer, oInstructionalHall, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oInstructionalHall] = new System.Windows.Point(4.25, 5.75);

            Room oPhrenologyOffice = AddRoom("Phrenology Office", "Department of Molecular Phrenology Office");
            e = AddExit(oInstructionalHall, oPhrenologyOffice, "office");
            e.RequiresDay = true;
            AddExit(oPhrenologyOffice, oInstructionalHall, "north");
            breeStreetsGraph.Rooms[oPhrenologyOffice] = new System.Windows.Point(4.25, 6);
            //CSRTODO: locked door

            Room oInstructionHall2 = AddRoom("Instruction", "Iluvatar Memorial Instructional Hall");
            AddBidirectionalExits(oInstructionalHall, oInstructionHall2, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oInstructionHall2] = new System.Windows.Point(5, 5.75);

            Room oInstructionHall3 = AddRoom("Instruction", "Iluvatar Memorial Instructional Hall");
            AddExit(oInstructionHall2, oInstructionHall3, "east");
            e = AddExit(oInstructionHall3, oInstructionHall2, "west");
            e.MaximumLevel = 9;
            breeStreetsGraph.Rooms[oInstructionHall3] = new System.Windows.Point(5.75, 5.75);

            Room oPhrenology = AddRoom("Phrenology", "Molecular Phrenology Classroom");
            AddExit(oInstructionHall2, oPhrenology, "north");
            AddExit(oPhrenology, oInstructionHall2, "door");
            breeStreetsGraph.Rooms[oPhrenology] = new System.Windows.Point(5, 5.5);

            Room oResearchHall = AddRoom("Research", "Iluvatar Memorial Research Hall");
            AddBidirectionalExits(oResearchHall, oFoyer, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oResearchHall] = new System.Windows.Point(4.25, 5);

            Room oErech = AddRoom("Erech", "Erech's Laboratory");
            oErech.AddPermanentMobs(MobTypeEnum.Erech);
            AddExit(oResearchHall, oErech, "north");
            AddExit(oErech, oResearchHall, "door");
            breeStreetsGraph.Rooms[oErech] = new System.Windows.Point(4.25, 4.75);

            Room oResearchHall2 = AddRoom("Research", "Iluvatar Memorial Research Hall");
            AddBidirectionalExits(oResearchHall, oResearchHall2, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oResearchHall2] = new System.Windows.Point(5, 5);

            Room oMysticalSciences = AddRoom("Mystic Science", "Department of Mystical Sciences Office");
            e = AddExit(oResearchHall2, oMysticalSciences, "office");
            e.RequiresDay = true;
            AddExit(oMysticalSciences, oResearchHall2, "north");
            breeStreetsGraph.Rooms[oMysticalSciences] = new System.Windows.Point(5, 5.25);
            //CSRTODO: locked door

            Room oResearchHall3 = AddRoom("Research", "Iluvatar Memorial Research Hall");
            AddBidirectionalExits(oResearchHall2, oResearchHall3, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oResearchHall3] = new System.Windows.Point(5.75, 5);

            Room oPhrenologyLaboratory = AddRoom("Lab", "Molecular Phrenology Laboratory");
            AddExit(oResearchHall3, oPhrenologyLaboratory, "north");
            AddExit(oPhrenologyLaboratory, oResearchHall3, "door");
            breeStreetsGraph.Rooms[oPhrenologyLaboratory] = new System.Windows.Point(5.75, 4.75);

            Room oMysticalScienceLab = AddRoom("Lab", "Mystical Sciences Laboratory");
            AddExit(oResearchHall3, oMysticalScienceLab, "south");
            AddExit(oMysticalScienceLab, oResearchHall3, "door");
            breeStreetsGraph.Rooms[oMysticalScienceLab] = new System.Windows.Point(5.75, 5.25);

            Room oCampusWalkSouth = AddRoom("Walk", "Campus Walk South");
            AddBidirectionalExits(oInstructionHall3, oCampusWalkSouth, BidirectionalExitType.WestEast);
            AddExit(breeStreets[5, 3], oCampusWalkSouth, "university");
            AddExit(oCampusWalkSouth, breeStreets[5, 3], "south");
            breeStreetsGraph.Rooms[oCampusWalkSouth] = new System.Windows.Point(6.25, 5.75);

            Room oCampusWalkSouth2 = AddRoom("Walk", "Campus Walk South");
            AddBidirectionalExits(oCampusWalkSouth2, oCampusWalkSouth, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oCampusWalkSouth2] = new System.Windows.Point(6.25, 5.5);

            Room oFinancialAidOffice = AddRoom("Aid", "Financial Aid Office");
            AddBidirectionalExits(oCampusWalkSouth2, oFinancialAidOffice, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oFinancialAidOffice] = new System.Windows.Point(6.6, 5.5);

            Room oCampusWalkSouth3 = AddRoom("Walk", "Campus Walk South");
            AddBidirectionalExits(oCampusWalkSouth3, oCampusWalkSouth2, BidirectionalExitType.NorthSouth);
            AddExit(oResearchHall3, oCampusWalkSouth3, "east");
            e = AddExit(oCampusWalkSouth3, oResearchHall3, "west");
            e.MaximumLevel = 12;
            breeStreetsGraph.Rooms[oCampusWalkSouth3] = new System.Windows.Point(6.25, 5);

            Room oUniversityQuad = AddRoom("Quad", "University Quad");
            AddBidirectionalExits(oUniversityQuad, oCampusWalkSouth3, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oUniversityQuad] = new System.Windows.Point(6.25, 4.75);

            Room oCampusWalkEast = AddRoom("Walk", "Campus Walk East");
            AddBidirectionalExits(oUniversityQuad, oCampusWalkEast, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCampusWalkEast, breeStreets[7, 5], BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oCampusWalkEast] = new System.Windows.Point(6.6, 4.75);
            breeStreetsGraph.Rooms[breeStreets[7, 5]] = new System.Windows.Point(7, 4.75);
            breeStreetsGraph.Rooms[oBigPapa] = new System.Windows.Point(8, 4.75);

            Room oCampusWalkNorth1 = AddRoom("Walk", "Campus Walk North");
            AddExit(oCampusWalkNorth1, oUniversityQuad, "south");
            e = AddExit(oUniversityQuad, oCampusWalkNorth1, "north");
            e.MaximumLevel = 7;
            breeStreetsGraph.Rooms[oCampusWalkNorth1] = new System.Windows.Point(6.25, 4.5);

            Room oCampusWalkNorth2 = AddRoom("Walk", "Campus Walk North");
            AddBidirectionalExits(oCampusWalkNorth2, oCampusWalkNorth1, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oCampusWalkNorth2] = new System.Windows.Point(6.25, 4.25);

            Room oLuistrin = AddRoom("Luistrin", "Halfast Hall");
            oLuistrin.AddPermanentMobs(MobTypeEnum.LuistrinTheArchitect);
            AddExit(oLuistrin, oCampusWalkNorth2, "out");
            AddExit(oCampusWalkNorth2, oLuistrin, "west");
            breeStreetsGraph.Rooms[oLuistrin] = new System.Windows.Point(5.5, 4.25);

            Room oHalfastHall = AddRoom("Halfast Hall", "Halfast Hall");
            AddBidirectionalExits(oHalfastHall, oLuistrin, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oHalfastHall] = new System.Windows.Point(4.75, 4.25);

            Room oToHalfastHall = AddRoom("Halfast Steps", "Steps to Halfast Hall");
            AddBidirectionalExits(oToHalfastHall, oHalfastHall, BidirectionalExitType.WestEast);
            e = AddExit(oUniversityEntrance, oToHalfastHall, "northeast");
            e.MaximumLevel = 7;
            AddExit(oToHalfastHall, oUniversityEntrance, "southwest");
            breeStreetsGraph.Rooms[oToHalfastHall] = new System.Windows.Point(4, 4.25);

            Room oSnarlingMutt = AddRoom("Snarling Mutt", "Snar Slystone's Apothecary and Curio Shoppe");
            oSnarlingMutt.AddPermanentMobs(MobTypeEnum.SnarlingMutt);
            AddBidirectionalExitsWithOut(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            breeStreetsGraph.Rooms[oSnarlingMutt] = new System.Windows.Point(9, 6);

            Room oSnarSlystone = AddRoom("Snar Slystone", "Behind The Counter");
            oSnarSlystone.AddPermanentMobs(MobTypeEnum.SnarSlystone);
            AddBidirectionalExitsWithOut(oSnarlingMutt, oSnarSlystone, "counter");
            breeStreetsGraph.Rooms[oSnarSlystone] = new System.Windows.Point(9, 5.75);

            Room oBackHall = AddRoom("Back Hall", "Back Hall");
            AddBidirectionalSameNameExit(oSnarSlystone, oBackHall, "curtain");
            breeStreetsGraph.Rooms[oBackHall] = new System.Windows.Point(9, 5.5);

            Room oAtticCrawlway = AddRoom("Attic Crawlway", "Attic Crawlway");
            oAtticCrawlway.AddPermanentMobs(MobTypeEnum.Bugbear);
            e = AddExit(oBackHall, oAtticCrawlway, "hatch");
            e.Hidden = true;
            AddExit(oAtticCrawlway, oBackHall, "hatch");
            breeStreetsGraph.Rooms[oAtticCrawlway] = new System.Windows.Point(9, 5.25);

            Room oGuido = AddRoom("Guido", "Godfather's House of Games");
            oGuido.AddPermanentMobs(MobTypeEnum.Guido);
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");
            breeStreetsGraph.Rooms[oGuido] = new System.Windows.Point(4, -0.5);

            Room oGodfather = AddRoom("Godfather", "Godfather's Office");
            oGodfather.AddPermanentMobs(MobTypeEnum.Godfather);
            e = AddExit(oGuido, oGodfather, "door");
            e.Hidden = true;
            e.MustOpen = true;
            e = AddExit(oGodfather, oGuido, "door");
            e.MustOpen = true;
            breeStreetsGraph.Rooms[oGodfather] = new System.Windows.Point(4, -1);

            Room oSergeantGrimgall = AddRoom("Sergeant Grimgall", "Guard Headquarters");
            oSergeantGrimgall.AddPermanentMobs(MobTypeEnum.SergeantGrimgall);
            AddExit(oToBarracks, oSergeantGrimgall, "barracks");
            AddExit(oSergeantGrimgall, oToBarracks, "east");
            breeStreetsGraph.Rooms[oSergeantGrimgall] = new System.Windows.Point(6, 8);

            Room oGuardsRecRoom = AddRoom("Guard's Rec Room", "Guard's Rec Room");
            AddBidirectionalExits(oSergeantGrimgall, oGuardsRecRoom, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oGuardsRecRoom] = new System.Windows.Point(6, 8.5);

            Room oBreePawnShopWest = AddPawnShoppeRoom("Pawn SW", "Ixell's Antique Shop", PawnShoppe.BreeSouthwest);
            AddBidirectionalExits(oBreePawnShopWest, oToPawnShopWest, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oBreePawnShopWest] = new System.Windows.Point(2, 8);

            Room oIxellsGeneralStore = AddRoom("General Store", "Ixell's General Store");
            AddExit(breeStreets[1, 3], oIxellsGeneralStore, "store");
            AddExit(oIxellsGeneralStore, breeStreets[1, 3], "north");
            AddBidirectionalSameNameExit(oIxellsGeneralStore, oBreePawnShopWest, "door");
            breeStreetsGraph.Rooms[oIxellsGeneralStore] = new System.Windows.Point(1, 8);

            Room oBreePawnShopEast = AddPawnShoppeRoom("Pawn Shop", "Pawn Shop", PawnShoppe.BreeNortheast);
            e = AddExit(oPoorAlley1, oBreePawnShopEast, "east");
            e.Hidden = true;
            AddExit(oBreePawnShopEast, oPoorAlley1, "west");
            breeStreetsGraph.Rooms[oBreePawnShopEast] = new System.Windows.Point(13, 4);

            Room oLeonardosFoundry = AddRoom("Leo Foundry", "Leonardo's Foundry");
            AddExit(oToLeonardosFoundry, oLeonardosFoundry, "foundry");
            AddExit(oLeonardosFoundry, oToLeonardosFoundry, "east");
            breeStreetsGraph.Rooms[oLeonardosFoundry] = new System.Windows.Point(9, 1);

            Room oLeonardosSwords = AddRoom("Leo Swords", "Custom Swords");
            AddBidirectionalExits(oLeonardosSwords, oLeonardosFoundry, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oLeonardosSwords] = new System.Windows.Point(9, 0.5);

            Room oLeonardosArmor = AddRoom("Leo Armor", "Unblemished Armor");
            AddBidirectionalExits(oLeonardosArmor, oLeonardosFoundry, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oLeonardosArmor] = new System.Windows.Point(8, 1);

            Room oLeonardosShields = AddRoom("Leo Shields", "Cast Iron Shields");
            AddBidirectionalExits(oLeonardosFoundry, oLeonardosShields, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oLeonardosShields] = new System.Windows.Point(9, 1.5);

            Room oZooEntrance = AddRoom("Zoo Entrance", "Scranlin's Zoological Wonders");
            AddExit(oToZoo, oZooEntrance, "zoo");
            AddExit(oZooEntrance, oToZoo, "exit");
            breeStreetsGraph.Rooms[oZooEntrance] = new System.Windows.Point(2, -0.5);

            Room oPathThroughScranlinsZoo = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo, oZooEntrance, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo] = new System.Windows.Point(2, -1);

            Room oScranlinsPettingZoo = AddRoom("Petting Zoo", "Scranlin's Petting Zoo");
            AddExit(oPathThroughScranlinsZoo, oScranlinsPettingZoo, "north");
            AddExit(oScranlinsPettingZoo, oPathThroughScranlinsZoo, "south");
            breeStreetsGraph.Rooms[oScranlinsPettingZoo] = new System.Windows.Point(2, -1.25);

            Room oScranlinsTrainingArea = AddRoom("Training Area", "Scranlin's Training Area");
            oScranlinsTrainingArea.NoFlee = true;
            e = AddExit(oScranlinsPettingZoo, oScranlinsTrainingArea, "clearing");
            e.Hidden = true;
            AddExit(oScranlinsTrainingArea, oScranlinsPettingZoo, "gate");
            breeStreetsGraph.Rooms[oScranlinsTrainingArea] = new System.Windows.Point(2, -1.5);

            Room oScranlin = AddRoom("Scranlin", "Scranlin's Outhouse");
            oScranlin.AddPermanentMobs(MobTypeEnum.Scranlin);
            e = AddBidirectionalExitsWithOut(oScranlinsTrainingArea, oScranlin, "outhouse");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oScranlin] = new System.Windows.Point(2, -1.75);

            Room oPathThroughScranlinsZoo2 = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo2, oPathThroughScranlinsZoo, BidirectionalExitType.SoutheastNorthwest);
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo2] = new System.Windows.Point(1, -2);

            Room oPathThroughScranlinsZoo3 = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo3, oPathThroughScranlinsZoo2, BidirectionalExitType.SouthwestNortheast);
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo3] = new System.Windows.Point(2, -3);

            Room oPathThroughScranlinsZoo4 = AddRoom("Path", "Path through Scranlin's Zoo");
            e = AddExit(oPathThroughScranlinsZoo3, oPathThroughScranlinsZoo4, "southeast");
            e.MaximumLevel = 10;
            AddExit(oPathThroughScranlinsZoo4, oPathThroughScranlinsZoo3, "northwest");
            e = AddExit(oPathThroughScranlinsZoo, oPathThroughScranlinsZoo4, "northeast");
            e.MaximumLevel = 10;
            AddExit(oPathThroughScranlinsZoo4, oPathThroughScranlinsZoo, "southwest");
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo4] = new System.Windows.Point(3, -2);

            Room oDogHouse = AddRoom("Dog House", "The Dog House");
            oDogHouse.AddPermanentMobs(MobTypeEnum.Lathlorien);
            AddBidirectionalExitsWithOut(oPathThroughScranlinsZoo2, oDogHouse, "doghouse");
            breeStreetsGraph.Rooms[oDogHouse] = new System.Windows.Point(1, -1);

            Room oMonkeyHouse = AddRoom("Monkey House", "Monkey House");
            AddBidirectionalExits(oMonkeyHouse, oPathThroughScranlinsZoo4, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oMonkeyHouse] = new System.Windows.Point(2.67, -2);

            Room oReptileHouse = AddRoom("Reptile House", "Reptile House");
            AddBidirectionalExits(oPathThroughScranlinsZoo4, oReptileHouse, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oReptileHouse] = new System.Windows.Point(4, -2);

            Room oCreaturesOfMyth = AddRoom("Creatures of Myth", "Creatures of Myth");
            e = AddExit(oPathThroughScranlinsZoo2, oCreaturesOfMyth, "west");
            e.MinimumLevel = 10;
            AddExit(oCreaturesOfMyth, oPathThroughScranlinsZoo2, "east");
            breeStreetsGraph.Rooms[oCreaturesOfMyth] = new System.Windows.Point(0, -2);

            Room oGeneticBlunders = AddRoom("Genetic Blunders", "Genetic Blunders");
            e = AddExit(oPathThroughScranlinsZoo2, oGeneticBlunders, "east");
            e.MinimumLevel = 4;
            AddExit(oGeneticBlunders, oPathThroughScranlinsZoo2, "west");
            breeStreetsGraph.Rooms[oGeneticBlunders] = new System.Windows.Point(1.67, -2);

            Room oBeastsOfFire = AddRoom("Beasts of Fire", "Beasts of Fire");
            e = AddExit(oPathThroughScranlinsZoo3, oBeastsOfFire, "north");
            e.MustOpen = true;
            e.MinimumLevel = 5;
            e = AddExit(oBeastsOfFire, oPathThroughScranlinsZoo3, "door");
            e.MustOpen = true;
            breeStreetsGraph.Rooms[oBeastsOfFire] = new System.Windows.Point(2, -4);

            Room oOceania = AddRoom("Oceania", "Oceania");
            e = AddExit(oPathThroughScranlinsZoo3, oOceania, "south");
            e.MinimumLevel = 4;
            AddExit(oOceania, oPathThroughScranlinsZoo3, "north");
            breeStreetsGraph.Rooms[oOceania] = new System.Windows.Point(2, -2.5);
            //CSRTODO: tank (fly)

            boatswain = AddRoom("Boatswain", "Stern of the Celduin Express");
            boatswain.AddPermanentMobs(MobTypeEnum.Boatswain);
            boatswain.BoatLocationType = BoatEmbarkOrDisembark.CelduinExpress;
            breeStreetsGraph.Rooms[boatswain] = new System.Windows.Point(9, 9.5);
            e = AddExit(breeDocks, boatswain, "steamboat");
            e.PresenceType = ExitPresenceType.Periodic;
            e = AddExit(boatswain, breeDocks, "dock");
            e.PresenceType = ExitPresenceType.Periodic;
            AddMapBoundaryPoint(breeDocks, boatswain, MapType.BreeStreets, MapType.Mithlond);

            Room oPearlAlley = AddRoom("Pearl Alley", "Pearl Alley");
            AddExit(oBreeTownSquare, oPearlAlley, "alley");
            AddExit(oPearlAlley, oBreeTownSquare, "north");
            breeStreetsGraph.Rooms[oPearlAlley] = new System.Windows.Point(5, 4);

            Room oPrancingPony = AddRoom("Prancing Pony", "Prancing Pony Tavern");
            oPrancingPony.AddPermanentMobs(MobTypeEnum.Bartender, MobTypeEnum.Bartender, MobTypeEnum.Waitress, MobTypeEnum.Waitress, MobTypeEnum.Waitress);
            AddBidirectionalExits(oPearlAlley, oPrancingPony, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oPrancingPony] = new System.Windows.Point(5.8, 4);

            Room oHobbitsHideawayEntrance = AddRoom("Hideaway Entrance", "Entrance to the Hobbit's Hideaway");
            e = AddExit(orderOfLove, oHobbitsHideawayEntrance, "cubbyhole");
            e.Hidden = true;
            e.MaximumLevel = 8;
            AddExit(oHobbitsHideawayEntrance, orderOfLove, "west");
            breeStreetsGraph.Rooms[oHobbitsHideawayEntrance] = new System.Windows.Point(16, 2);

            Room oHobbitClearing = AddRoom("Hobbit Clearing", "Hobbit Clearing");
            AddBidirectionalExits(oHobbitsHideawayEntrance, oHobbitClearing, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oHobbitClearing] = new System.Windows.Point(17, 2);

            Room oChiefsHole = AddRoom("Chief's Hole", "Chief's Hole");
            AddBidirectionalExitsWithOut(oHobbitClearing, oChiefsHole, "chief's");
            breeStreetsGraph.Rooms[oChiefsHole] = new System.Windows.Point(16, 1);

            Room oBranco = AddRoom("Branco", "The Chief's Bedchambers");
            oBranco.AddPermanentMobs(MobTypeEnum.BrancoTheHobbitsChief);
            AddBidirectionalExitsWithOut(oChiefsHole, oBranco, "bedchambers");
            breeStreetsGraph.Rooms[oBranco] = new System.Windows.Point(15, 1);

            Room oHobbitsTemple = AddRoom("Temple", "The Hobbit's Temple");
            AddBidirectionalExitsWithOut(oHobbitClearing, oHobbitsTemple, "temple");
            breeStreetsGraph.Rooms[oHobbitsTemple] = new System.Windows.Point(16, 2.5);

            Room oBeneathTheHobbitsAltar = AddRoom("Under Altar", "Beneath the Hobbit's Altar");
            oBeneathTheHobbitsAltar.AddPermanentMobs(MobTypeEnum.LuthicTheHighPriestess);
            oBeneathTheHobbitsAltar.IsTrapRoom = true;
            e = AddExit(oHobbitsTemple, oBeneathTheHobbitsAltar, "altar");
            e.Hidden = true;
            AddExit(oBeneathTheHobbitsAltar, oHobbitsTemple, "up");
            breeStreetsGraph.Rooms[oBeneathTheHobbitsAltar] = new System.Windows.Point(17, 2.5);

            breeEastGateOutside = AddRoom("East Gate Outside", "East Gate of Bree");
            breeStreetsGraph.Rooms[breeEastGateOutside] = new System.Windows.Point(18, 3);

            oCemetery = AddRoom("Cemetery", "The Cemetery");
            e = AddExit(breeEastGateOutside, oCemetery, "path");
            e.Hidden = true;
            e.RequiresDay = true;
            e = AddExit(oCemetery, oHobbitClearing, "west");
            e.MaximumLevel = 8;
            breeStreetsGraph.Rooms[oCemetery] = new System.Windows.Point(18, 2);
            AddMapBoundaryPoint(breeEastGateOutside, oCemetery, MapType.BreeToImladris, MapType.BreeStreets);

            Room oCommonArea = AddRoom("Common Area", "The Common Area");
            AddBidirectionalExitsWithOut(oHobbitClearing, oCommonArea, "common");
            breeStreetsGraph.Rooms[oCommonArea] = new System.Windows.Point(17, 1);

            Room oMainDiningHall = AddRoom("Dining Hall", "The Main Dining Hall");
            AddBidirectionalExitsWithOut(oCommonArea, oMainDiningHall, "dining");
            breeStreetsGraph.Rooms[oMainDiningHall] = new System.Windows.Point(17, 0);

            Room oBigPapaSmallHallway = AddRoom("Small Hallway", "Small hallway");
            e = AddExit(oBigPapa, oBigPapaSmallHallway, "panel");
            e.Hidden = true;
            e = AddExit(oBigPapaSmallHallway, oBigPapa, "panel");
            e.MustOpen = true;
            breeStreetsGraph.Rooms[oBigPapaSmallHallway] = new System.Windows.Point(8, 4.5);

            Room oWizardsEye = AddRoom("Wizard's Eye", "Wizard's Eye");
            e = AddBidirectionalExitsWithOut(breeStreets[5, 0], oWizardsEye, "north");
            e.RequiredClass = ClassType.Mage;
            breeStreetsGraph.Rooms[oWizardsEye] = new System.Windows.Point(5, 9.75);

            Room oMageTraining = AddRoom("Mage Training", "Mage Training");
            oMageTraining.AddPermanentMobs(MobTypeEnum.MagorTheInstructor);
            e = AddExit(oWizardsEye, oMageTraining, "archway");
            e.Hidden = true;
            AddExit(oMageTraining, oWizardsEye, "door");
            breeStreetsGraph.Rooms[oMageTraining] = new System.Windows.Point(5, 9.5);

            Room oPostOffice = AddRoom("Post Office", "Post Office");
            AddExit(breeStreets[1, 7], oPostOffice, "post office");
            AddExit(oPostOffice, breeStreets[1, 7], "north");
            breeStreetsGraph.Rooms[oPostOffice] = new System.Windows.Point(1, 3.5);

            Room oHallOfQuests = AddRoom("Quest Hall", "Hall of Quests");
            oHallOfQuests.AddPermanentMobs(MobTypeEnum.DenethoreTheWise);
            AddExit(breeStreets[1, 7], oHallOfQuests, "hall");
            AddExit(oHallOfQuests, breeStreets[1, 7], "south");
            breeStreetsGraph.Rooms[oHallOfQuests] = new System.Windows.Point(1, 2.5);

            Room oHallOfEarth = AddRoom("Earth Hall", "Hall of Earth");
            oHallOfEarth.AddPermanentMobs(MobTypeEnum.EarthenLoremaster);
            AddBidirectionalExitsWithOut(oHallOfQuests, oHallOfEarth, "earth");
            breeStreetsGraph.Rooms[oHallOfEarth] = new System.Windows.Point(1, 2);

            Room oFarmersMarket = AddRoom("Farmer's Market", "Farmer's Market");
            AddExit(breeStreets[11, 7], oFarmersMarket, "market");
            AddExit(oFarmersMarket, breeStreets[11, 7], "south");
            breeStreetsGraph.Rooms[oFarmersMarket] = new System.Windows.Point(11, 2.5);

            Room oBank = AddRoom("Bank", "Bree Municipal Bank");
            AddExit(breeStreets[8, 7], oBank, "bank");
            AddExit(oBank, breeStreets[8, 7], "south");
            breeStreetsGraph.Rooms[oBank] = new System.Windows.Point(8, 2.5);

            Room oShrineEntrance = AddRoom("Shrine Entrance", "Entrance to Shrine of Moradin");
            AddExit(breeStreets[8, 7], oShrineEntrance, "shrine");
            AddExit(oShrineEntrance, breeStreets[8, 7], "north");
            breeStreetsGraph.Rooms[oShrineEntrance] = new System.Windows.Point(8, 3.3);

            Room oShrineFoyer = AddRoom("Shrine Foyer", "Foyer of Shrine of Moradin");
            AddBidirectionalSameNameExit(oShrineEntrance, oShrineFoyer, "gate");
            breeStreetsGraph.Rooms[oShrineFoyer] = new System.Windows.Point(8, 3.8);

            Room oShrineWest = AddRoom("Thoringil", "High Priest's Chambers");
            oShrineWest.AddPermanentMobs(MobTypeEnum.ThoringilTheHoly);
            e = AddExit(oShrineFoyer, oShrineWest, "west door");
            e.MustOpen = true;
            AddExit(oShrineWest, oShrineFoyer, "east");
            breeStreetsGraph.Rooms[oShrineWest] = new System.Windows.Point(7.3, 3.8);

            Room oShrineEast = AddRoom("Prayer Room", "Prayer room");
            e = AddExit(oShrineFoyer, oShrineEast, "east door");
            e.MustOpen = true;
            AddExit(oShrineEast, oShrineFoyer, "west");
            breeStreetsGraph.Rooms[oShrineEast] = new System.Windows.Point(8.7, 3.8);

            Room oGreatHallOfMoradin = AddRoom("Great Hall", "Great Hall of Moradin");
            AddBidirectionalExits(oShrineFoyer, oGreatHallOfMoradin, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oGreatHallOfMoradin] = new System.Windows.Point(8, 4.3);

            Room oChapel = AddRoom("Princess Bride", "The Chapel");
            oChapel.AddPermanentMobs(MobTypeEnum.BrideNamedPrincess);
            AddExit(breeStreets[6, 7], oChapel, "chapel");
            AddExit(oChapel, breeStreets[6, 7], "north");
            breeStreetsGraph.Rooms[oChapel] = new System.Windows.Point(6, 3.5);

            Room oShepherdsShop = AddRoom("Shepherd's Shop", "Shepherd's Shop");
            AddExit(breeStreets[6, 7], oShepherdsShop, "shop");
            AddExit(oShepherdsShop, breeStreets[6, 7], "south");
            breeStreetsGraph.Rooms[oShepherdsShop] = new System.Windows.Point(6, 2.5);

            Room oMonkTraining = AddRoom("Caladon Monastery", "Monastary");
            oMonkTraining.AddPermanentMobs(MobTypeEnum.AgedMonk);
            e = AddExit(breeStreets[7, 4], oMonkTraining, "monastary");
            e.RequiredClass = ClassType.Monk;
            AddExit(oMonkTraining, breeStreets[7, 4], "east");
            breeStreetsGraph.Rooms[oMonkTraining] = new System.Windows.Point(6, 6.75);
            breeStreetsGraph.Rooms[breeStreets[7, 4]] = new System.Windows.Point(7, 6.75);

            Room oOldClassRoom = AddRoom("Old Classroom", "Old Classroom");
            AddExit(oBreeTownSquare, oOldClassRoom, "school");
            AddExit(oOldClassRoom, oBreeTownSquare, "south");
            breeStreetsGraph.Rooms[oOldClassRoom] = new System.Windows.Point(5, 2.5);

            Room oClassroomDesk = AddRoom("Classroom Desk", Room.UNKNOWN_ROOM);
            AddExit(oOldClassRoom, oClassroomDesk, "north");
            breeStreetsGraph.Rooms[oClassroomDesk] = new System.Windows.Point(5, 2);

            Room oGrandBallroom = AddRoom("Grant Ballroom", "The Isengard Grand Ballroom");
            AddBidirectionalExitsWithOut(breeStreets[4, 7], oGrandBallroom, "ballroom");
            breeStreetsGraph.Rooms[oGrandBallroom] = new System.Windows.Point(4, 3.5);

            Room oCityHall = AddRoom("City Hall", "Bree City Hall");
            oCityHall.AddPermanentItems(ItemTypeEnum.InformationKiosk, ItemTypeEnum.TownMap);
            AddExit(breeStreets[4, 7], oCityHall, "hall");
            AddExit(oCityHall, breeStreets[4, 7], "south");
            breeStreetsGraph.Rooms[oCityHall] = new System.Windows.Point(4, 2.5);

            Room oStocks = AddRoom("Stocks", "The Stocks");
            oStocks.AddPermanentMobs(MobTypeEnum.Rex, MobTypeEnum.Accuser);
            AddExit(oCityHall, oStocks, "stocks");
            AddExit(oStocks, oCityHall, "office");
            AddExit(oStocks, breeStreets[3, 9], "west");
            AddExit(breeStreets[3, 8], oStocks, "stocks");
            AddExit(breeStreets[3, 9], oStocks, "stocks");
            breeStreetsGraph.Rooms[oStocks] = new System.Windows.Point(4, 2);

            Room oExecutionersChamber = AddRoom("Execution Chamber", "Executioner's Chamber");
            AddBidirectionalExits(oStocks, oExecutionersChamber, BidirectionalExitType.UpDown);
            breeStreetsGraph.Rooms[oExecutionersChamber] = new System.Windows.Point(4, 1.5);

            Room oHallOfAvatars = AddRoom("Avatar Hall", "Hall of Avatars");
            AddBidirectionalExitsWithOut(oCityHall, oHallOfAvatars, "avatar hall");
            breeStreetsGraph.Rooms[oHallOfAvatars] = new System.Windows.Point(3.33, 2.67);

            Room oHallOfAvatars2 = AddRoom("Avatar Hall", "Hall of Avatars");
            AddBidirectionalSameNameExit(oHallOfAvatars, oHallOfAvatars2, "curtain");
            breeStreetsGraph.Rooms[oHallOfAvatars2] = new System.Windows.Point(3.33, 2.33);

            Room oDabinsFuneralHome = AddRoom("Funeral Home", "Dabin's Funeral Home");
            AddExit(breeStreets[3, 9], oDabinsFuneralHome, "home");
            AddExit(oDabinsFuneralHome, breeStreets[3, 9], "east");
            breeStreetsGraph.Rooms[oDabinsFuneralHome] = new System.Windows.Point(2, 1);

            Room oFuneralHomeCemetery = AddRoom("Cemetery", "Cemetery");
            oFuneralHomeCemetery.AddPermanentMobs(MobTypeEnum.Caretaker);
            e = AddExit(oDabinsFuneralHome, oFuneralHomeCemetery, "northwest");
            e.Hidden = true;
            AddExit(oFuneralHomeCemetery, oDabinsFuneralHome, "southeast");
            breeStreetsGraph.Rooms[oFuneralHomeCemetery] = new System.Windows.Point(1.5, 0.5);
            //CSRTODO: hidden exit that doesn't seem to be returnable since the return exit is locked

            Room oClans = AddRoom("Clans", "The Clans of Middle Earth");
            AddExit(breeStreets[7, 6], oClans, "hall");
            AddExit(oClans, breeStreets[7, 6], "east");
            breeStreetsGraph.Rooms[oClans] = new System.Windows.Point(6.4, 4);

            Room oTempleOfLolth = AddRoom("Lolth Temple", "Temple of Lolth");
            AddExit(breeStreets[0, 9], oTempleOfLolth, "temple");
            AddExit(oTempleOfLolth, breeStreets[0, 9], "west");
            breeStreetsGraph.Rooms[oTempleOfLolth] = new System.Windows.Point(0.67, 1);

            Room oUndergroundTemple = AddRoom("Underground Temple", "Underground Temple of Lolth");
            oUndergroundTemple.AddPermanentMobs(MobTypeEnum.DrowElf, MobTypeEnum.DrowElf, MobTypeEnum.DrowElf);
            e = AddExit(oTempleOfLolth, oUndergroundTemple, "underground temple");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oUndergroundTemple] = new System.Windows.Point(0.67, 0.75);
            //CSRTODO: more hidden rooms from here

            Room oReadyRoom = AddRoom("Ready Room", "Ready Room");
            AddExit(oToArena, oReadyRoom, "arena");
            AddExit(oReadyRoom, oToArena, "north");
            breeStreetsGraph.Rooms[oReadyRoom] = new System.Windows.Point(5, 0.375);

            Room oArenaWarmup = AddRoom("Warmup", "Arena Warmup");
            AddBidirectionalExits(oReadyRoom, oArenaWarmup, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oArenaWarmup] = new System.Windows.Point(5, 0.75);

            Room oMainArena = AddRoom("Main", "Main Arena");
            AddBidirectionalExits(oArenaWarmup, oMainArena, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oMainArena] = new System.Windows.Point(5, 1.125);

            Room oArenaChallenge = AddRoom("Challenge", "Arena Challenge");
            AddBidirectionalExits(oMainArena, oArenaChallenge, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oArenaChallenge] = new System.Windows.Point(5, 1.5);

            Room oArenaFirstAid = AddHealingRoom("First Aid", "Arena First Aid", HealingRoom.BreeArena);
            AddBidirectionalExits(oArenaFirstAid, oArenaChallenge, BidirectionalExitType.SouthwestNortheast);
            breeStreetsGraph.Rooms[oArenaFirstAid] = new System.Windows.Point(6, 0.5);

            Room oChampionsArena = AddRoom("Champion", "Champion's Arena");
            AddBidirectionalExits(oArenaChallenge, oChampionsArena, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oChampionsArena] = new System.Windows.Point(5.67, 1.5);

            Room oChampionsWarmup = AddRoom("Warmup", "Warmup for Champion's Arena");
            AddBidirectionalExits(oChampionsArena, oChampionsWarmup, BidirectionalExitType.WestEast);
            AddExit(oChampionsWarmup, breeStreets[7, 8], "east");
            e = AddExit(breeStreets[7, 8], oChampionsWarmup, "arena");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oChampionsWarmup] = new System.Windows.Point(6.33, 1.5);
            breeStreetsGraph.Rooms[breeStreets[7, 8]] = new System.Windows.Point(7, 1.5);

            Room oHallOfGuilds = AddRoom("Guild Hall", "Hall of Guilds");
            e = AddExit(oToGuildHall, oHallOfGuilds, "hall");
            e.MustOpen = true;
            AddExit(oHallOfGuilds, oToGuildHall, "door");
            breeStreetsGraph.Rooms[oHallOfGuilds] = new System.Windows.Point(8, 0);

            accursedGuildHall = AddRoom("Accursed Guild", "Accursed Guild Hall");
            AddBidirectionalExitsWithOut(oHallOfGuilds, accursedGuildHall, "accursed");
            breeStreetsGraph.Rooms[accursedGuildHall] = new System.Windows.Point(6, -1);

            Room oDeathbringer = AddRoom("Deathbringer", "Chamber of the Accursed");
            oDeathbringer.AddPermanentMobs(MobTypeEnum.Deathbringer);
            AddBidirectionalSameNameExit(accursedGuildHall, oDeathbringer, "curtain");
            breeStreetsGraph.Rooms[oDeathbringer] = new System.Windows.Point(5, -1);

            Room oItemsOfDeath = AddRoom("Death Items", "Items of Death");
            AddBidirectionalExitsWithOut(accursedGuildHall, oItemsOfDeath, "desk");
            breeStreetsGraph.Rooms[oItemsOfDeath] = new System.Windows.Point(5, -0.5);

            crusaderGuildHall = AddRoom("Crusader Guild", "Crusader's Guild");
            AddBidirectionalExitsWithOut(oHallOfGuilds, crusaderGuildHall, "crusader");
            breeStreetsGraph.Rooms[crusaderGuildHall] = new System.Windows.Point(8, -1);

            Room oLadyGwyneth = AddRoom("Lady Gwyneth", "Guardian of the Crusader's Guild");
            AddBidirectionalExitsWithOut(crusaderGuildHall, oLadyGwyneth, "door");
            breeStreetsGraph.Rooms[oLadyGwyneth] = new System.Windows.Point(7, -1);

            thievesGuildHall = AddRoom("Thieves Guild", "Thieves Guild");
            AddBidirectionalExitsWithOut(oHallOfGuilds, thievesGuildHall, "thief");
            breeStreetsGraph.Rooms[thievesGuildHall] = new System.Windows.Point(9, -1);

            Room oWilliamTasker = AddRoom("William Tasker", "Dark Meeting Hall");
            AddBidirectionalExitsWithOut(thievesGuildHall, oWilliamTasker, "beads");
            breeStreetsGraph.Rooms[oWilliamTasker] = new System.Windows.Point(10, -1);

            Room oHonestBobsDiscountWares = AddRoom("Discount Wares", "Honest Bob's Discount Wares");
            AddExit(thievesGuildHall, oHonestBobsDiscountWares, "counter");
            AddExit(oHonestBobsDiscountWares, thievesGuildHall, "hall");
            breeStreetsGraph.Rooms[oHonestBobsDiscountWares] = new System.Windows.Point(10, -0.5);

            Room oSmithy = AddRoom("Smithy", "U'tral Smiths Inc.");
            AddExit(oToSmithy, oSmithy, "smithy");
            AddExit(oSmithy, oToSmithy, "south");
            breeStreetsGraph.Rooms[oSmithy] = new System.Windows.Point(13, 6.5);

            Room oMagicShop = AddRoom("Magic Shop", "Magic Shop");
            AddBidirectionalExitsWithOut(oToMagicShop, oMagicShop, "shop");
            breeStreetsGraph.Rooms[oMagicShop] = new System.Windows.Point(9, 5);

            Room oShippingWarehouse = AddRoom("Shipping Warehouse", "Shipping Warehouse");
            AddExit(breeStreets[11, 3], oShippingWarehouse, "warehouse");
            AddExit(oShippingWarehouse, breeStreets[11, 3], "north");
            AddExit(breeStreets[10, 2], oShippingWarehouse, "warehouse");
            AddExit(oShippingWarehouse, breeStreets[10, 2], "west");
            breeStreetsGraph.Rooms[oShippingWarehouse] = new System.Windows.Point(11, 8);

            Room oSouthernDump = AddRoom("Dump", "The Southern Dump");
            AddExit(breeStreets[10, 1], oSouthernDump, "dump");
            AddExit(oSouthernDump, breeStreets[10, 1], "west");
            AddExit(breeStreets[11, 0], oSouthernDump, "dump");
            AddExit(oSouthernDump, breeStreets[11, 0], "south");
            breeStreetsGraph.Rooms[oSouthernDump] = new System.Windows.Point(11, 9);

            Room oRavensRuseTavern = AddRoom("Raven's Ruse", "Raven's Ruse Tavern");
            AddExit(breeStreets[13, 0], oRavensRuseTavern, "tavern");
            AddExit(oRavensRuseTavern, breeStreets[13, 0], "south");
            AddExit(breeStreets[14, 1], oRavensRuseTavern, "tavern");
            AddExit(oRavensRuseTavern, breeStreets[14, 1], "east");
            breeStreetsGraph.Rooms[oRavensRuseTavern] = new System.Windows.Point(13, 9);

            Room oThievesDen = AddRoom("Thieves Den", "Thieve's Den");
            e = AddExit(oRavensRuseTavern, oThievesDen, "sliding panel");
            e.Hidden = true;
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            AddExit(oThievesDen, oRavensRuseTavern, "door");
            breeStreetsGraph.Rooms[oThievesDen] = new System.Windows.Point(13, 8.67);

            Room oRavensBackroom = AddRoom("Backroom", "Raven's Backroom");
            e = AddExit(oThievesDen, oRavensBackroom, "hatchway");
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            e.Hidden = true;
            AddExit(oRavensBackroom, oThievesDen, "hatchway");
            breeStreetsGraph.Rooms[oRavensBackroom] = new System.Windows.Point(13, 8.33);

            Room oReneesElvenLeatherWorks = AddRoom("Leather Works", "Renee's Elven Leather Works");
            AddExit(breeStreets[10, 8], oReneesElvenLeatherWorks, "shoppe");
            AddExit(oReneesElvenLeatherWorks, breeStreets[10, 8], "west");
            breeStreetsGraph.Rooms[oReneesElvenLeatherWorks] = new System.Windows.Point(11, 2);

            Room oHagbardsFineWeapons = AddRoom("Hagbard's Weapons", "Hagbard's Fine Weapons");
            AddExit(breeStreets[8, 3], oHagbardsFineWeapons, "shop");
            AddExit(oHagbardsFineWeapons, breeStreets[8, 3], "south");
            breeStreetsGraph.Rooms[oHagbardsFineWeapons] = new System.Windows.Point(8, 6.5);

            Room oBreeParkAndRecreation = AddRoom("Park and Recreation", "Bree Park and Recreation");
            AddExit(breeStreets[0, 4], oBreeParkAndRecreation, "park");
            AddExit(oBreeParkAndRecreation, breeStreets[0, 4], "west");
            breeStreetsGraph.Rooms[oBreeParkAndRecreation] = new System.Windows.Point(1, 6);

            Room oKandyAndToyShoppe = AddRoom("Kandy/Toy Shop", "The Kandy and Toy Shoppe");
            e = AddExit(breeStreets[3, 6], oKandyAndToyShoppe, "shoppe");
            e.MaximumLevel = 6;
            AddExit(oKandyAndToyShoppe, breeStreets[3, 6], "east");
            breeStreetsGraph.Rooms[oKandyAndToyShoppe] = new System.Windows.Point(2, 4);
            
            Room oBardConservatory = AddRoom("Bard Conservatory", "Bard Conservatory");
            oBardConservatory.AddPermanentMobs(MobTypeEnum.AgedBard);
            e = AddExit(breeStreets[4, 3], oBardConservatory, "conservatory");
            e.RequiredClass = ClassType.Bard;
            AddExit(oBardConservatory, breeStreets[4, 3], "north");
            breeStreetsGraph.Rooms[oBardConservatory] = new System.Windows.Point(4, 8);

            AddHauntedMansion(oHauntedMansionEntrance);
        }

        private void AddHauntedMansion(Room hauntedMansionEntrance)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            RoomGraph hauntedMansionGraph = _graphs[MapType.BreeHauntedMansion];

            hauntedMansionGraph.Rooms[hauntedMansionEntrance] = new System.Windows.Point(2, 8);

            Room oOldGardener = AddRoom("Old Gardener", "Path to Mansion");
            oOldGardener.AddPermanentMobs(MobTypeEnum.OldGardener);
            Exit e = AddExit(hauntedMansionEntrance, oOldGardener, "gate");
            e.KeyType = ItemTypeEnum.SilverKey;
            e.MustOpen = true;
            AddExit(oOldGardener, hauntedMansionEntrance, "gate");
            breeStreetsGraph.Rooms[oOldGardener] = new System.Windows.Point(2, 2.5);
            hauntedMansionGraph.Rooms[oOldGardener] = new System.Windows.Point(2, 7);
            AddMapBoundaryPoint(hauntedMansionEntrance, oOldGardener, MapType.BreeStreets, MapType.BreeHauntedMansion);

            Room oFoyer = AddRoom("Foyer", "Foyer of the Old Mansion");
            e = AddBidirectionalExitsWithOut(oOldGardener, oFoyer, "door");
            e.KeyType = ItemTypeEnum.SilverKey;
            e.MustOpen = true;
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
            oGhostlyFencer.AddPermanentMobs(MobTypeEnum.GhostlyFencer);
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

        private void AddUnderBree(Room oNorthBridge, Room oOuthouse, Room oSewerPipeExit)
        {
            RoomGraph underBreeGraph = _graphs[MapType.UnderBree];
            RoomGraph breeToImladrisGraph = _graphs[MapType.BreeToImladris];
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            Room droolie = AddRoom("Droolie", "Under North Bridge");
            droolie.AddPermanentMobs(MobTypeEnum.DroolieTheTroll);
            Exit e = AddExit(oNorthBridge, droolie, "rope");
            e.Hidden = true;
            AddExit(droolie, oNorthBridge, "up");
            breeStreetsGraph.Rooms[droolie] = new System.Windows.Point(9, 3.3);
            AddMapBoundaryPoint(oNorthBridge, droolie, MapType.BreeStreets, MapType.UnderBree);

            underBreeGraph.Rooms[oNorthBridge] = new System.Windows.Point(0, 0);
            underBreeGraph.Rooms[droolie] = new System.Windows.Point(0, 0.5);
            underBreeGraph.Rooms[oOuthouse] = new System.Windows.Point(8, 12);
            underBreeGraph.Rooms[oSewerPipeExit] = new System.Windows.Point(7, 2);

            Room oCatchBasin = AddRoom("Catch Basin", "Catch Basin");
            e = AddBidirectionalExitsWithOut(oOuthouse, oCatchBasin, "hole");
            e.Hidden = true;
            underBreeGraph.Rooms[oCatchBasin] = new System.Windows.Point(8, 11);
            breeToImladrisGraph.Rooms[oCatchBasin] = new System.Windows.Point(5, 7.5);
            AddMapBoundaryPoint(oOuthouse, oCatchBasin, MapType.BreeToImladris, MapType.UnderBree);

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

            Room oSewerDitch = AddRoom("Ditch", "Sewer Ditch");
            AddBidirectionalExitsWithOut(oBrandywineRiverShore, oSewerDitch, "ditch");
            underBreeGraph.Rooms[oSewerDitch] = new System.Windows.Point(8, 6);

            Room oSewerTunnel1 = AddRoom("Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel1, oSewerDitch, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTunnel1] = new System.Windows.Point(8, 5);

            Room oBoardedSewerTunnel = AddRoom("to Monster", "Boarded Sewer Tunnel");
            e = AddExit(oSewerTunnel1, oBoardedSewerTunnel, "east");
            e.FloatRequirement = FloatRequirement.Levitation;
            AddExit(oBoardedSewerTunnel, oSewerTunnel1, "west");
            underBreeGraph.Rooms[oBoardedSewerTunnel] = new System.Windows.Point(9, 5);

            Room oSewagePit = AddRoom("Sewage Pit", "Sewage Pit");
            oSewagePit.DamageType = RoomDamageType.Poison;
            oSewagePit.AddPermanentMobs(MobTypeEnum.Monster);
            e = AddExit(oSewagePit, oBoardedSewerTunnel, "up");
            e.FloatRequirement = FloatRequirement.Levitation;
            e = AddExit(oSewerTunnel1, oSewagePit, "east");
            e.FloatRequirement = FloatRequirement.NoLevitation;
            e.IsTrapExit = true;
            underBreeGraph.Rooms[oSewagePit] = new System.Windows.Point(10, 4.5);

            Room oStagnantCesspool = AddRoom("Stagnant Cesspool", "Stagnant Cesspool");
            AddBidirectionalExits(oSewagePit, oStagnantCesspool, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oStagnantCesspool] = new System.Windows.Point(10, 5);

            Room oSewerTConnection = AddRoom("T-Connection", "Sewer T-Connection");
            AddBidirectionalExits(oSewerTConnection, oSewerTunnel1, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTConnection] = new System.Windows.Point(8, 4);

            Room oSewerTunnel2 = AddRoom("Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel2, oSewerTConnection, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerTunnel2] = new System.Windows.Point(7, 4);

            Room oSewerPipe = AddRoom("Sewer Pipe", "Sewer Pipe");
            AddExit(oSewerTunnel2, oSewerPipe, "pipe");
            AddExit(oSewerPipe, oSewerTunnel2, "down");
            AddExit(oSewerPipe, oSewerPipeExit, "up");
            underBreeGraph.Rooms[oSewerPipe] = new System.Windows.Point(7, 3);
            breeStreetsGraph.Rooms[oSewerPipe] = new System.Windows.Point(10, 11);
            AddMapBoundaryPoint(oSewerPipe, oSewerPipeExit, MapType.UnderBree, MapType.BreeStreets);

            Room oSalamander = AddRoom("Salamander", "The Brandywine River");
            oSalamander.AddPermanentMobs(MobTypeEnum.Salamander);
            AddExit(oBrandywineRiverShore, oSalamander, "reeds");
            AddExit(oSalamander, oBrandywineRiverShore, "shore");
            underBreeGraph.Rooms[oSalamander] = new System.Windows.Point(9, 7);

            Room oBrandywineRiver1 = AddRoom("Brandywine River", "The Brandywine River");
            oBrandywineRiver1.DamageType = RoomDamageType.Water;
            AddExit(droolie, oBrandywineRiver1, "down");
            e = AddExit(oBrandywineRiver1, droolie, "rope");
            e.FloatRequirement = FloatRequirement.Fly;
            underBreeGraph.Rooms[oBrandywineRiver1] = new System.Windows.Point(0, 1);
            //CSRTODO: north

            Room oBrandywineRiver2 = AddRoom("Brandywine River", "The Brandywine River");
            oBrandywineRiver2.DamageType = RoomDamageType.Water;
            AddBidirectionalExits(oBrandywineRiver1, oBrandywineRiver2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver2] = new System.Windows.Point(1, 1);

            Room oOohlgrist = AddRoom("Oohlgrist", "Small Boat");
            oOohlgrist.AddPermanentMobs(MobTypeEnum.Oohlgrist);
            AddExit(oBrandywineRiver2, oOohlgrist, "boat");
            AddExit(oOohlgrist, oBrandywineRiver2, "river");
            underBreeGraph.Rooms[oOohlgrist] = new System.Windows.Point(2, 1);

            Room oBrandywineRiverBoathouse = AddRoom("Boathouse", "Brandywine River Boathouse");
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
            e = AddBidirectionalExitsWithOut(oRockyBeach2, oHermitsCave, "cave");
            e.Hidden = true;
            underBreeGraph.Rooms[oHermitsCave] = new System.Windows.Point(6, 1);

            Room oRockyAlcove = AddRoom("Rocky Alcove", "Rocky Alcove");
            AddExit(oRockyBeach1, oRockyAlcove, "alcove");
            AddExit(oRockyAlcove, oRockyBeach1, "north");
            underBreeGraph.Rooms[oRockyAlcove] = new System.Windows.Point(5, 0);

            Room oSewerDrain = AddRoom("Drain", "Sewer Drain");
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

            Room sewerTunnelToTConnection = AddRoom("Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oDrainTunnel4, sewerTunnelToTConnection, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(sewerTunnelToTConnection, oSewerTConnection, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[sewerTunnelToTConnection] = new System.Windows.Point(8, 3);

            Room oBoardedSewerTunnel2 = AddRoom("to Sewer Orc Guards", "Boarded Sewer Tunnel");
            AddBidirectionalExits(sewerTunnelToTConnection, oBoardedSewerTunnel2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBoardedSewerTunnel2] = new System.Windows.Point(9, 3);

            Room oSewerOrcChamber = AddRoom("Sewer Orc Guards", "Sewer Orc Chamber");
            oSewerOrcChamber.AddPermanentMobs(MobTypeEnum.SewerOrcGuard, MobTypeEnum.SewerOrcGuard);
            e = AddExit(oBoardedSewerTunnel2, oSewerOrcChamber, "busted board");
            e.Hidden = true;
            AddExit(oSewerOrcChamber, oBoardedSewerTunnel2, "busted board");
            underBreeGraph.Rooms[oSewerOrcChamber] = new System.Windows.Point(10, 3);

            Room oSewerOrcLair = AddRoom("Sewer Orc Lair", "Sewer Orc Lair");
            oSewerOrcLair.AddPermanentMobs(MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerRat, MobTypeEnum.SewerRat);
            AddBidirectionalExits(oSewerOrcLair, oSewerOrcChamber, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerOrcLair] = new System.Windows.Point(10, 2);

            Room oSewerPassage = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oSewerOrcLair, oSewerPassage, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerPassage] = new System.Windows.Point(11, 2);

            Room oSewerOrcStorageRoom = AddRoom("Storage Room", "Sewer Orc Storage Room");
            AddBidirectionalExits(oSewerPassage, oSewerOrcStorageRoom, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerOrcStorageRoom] = new System.Windows.Point(12, 2);

            Room oSlopingSewerPassage = AddRoom("Sloping Passage", "Sloping Sewer Passage");
            AddBidirectionalExits(oSewerOrcStorageRoom, oSlopingSewerPassage, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSlopingSewerPassage] = new System.Windows.Point(12, 3);

            Room oSewerPassageInFrontOfGate = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oSlopingSewerPassage, oSewerPassageInFrontOfGate, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerPassageInFrontOfGate] = new System.Windows.Point(12, 4);

            Room oSmoothedSewerPassage = AddRoom("Smoothed Passage", "Smoothed Sewer Passage");
            AddBidirectionalExits(oSewerPassageInFrontOfGate, oSmoothedSewerPassage, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSmoothedSewerPassage] = new System.Windows.Point(12, 5);

            Room oSlopingSewerPassage2 = AddRoom("Sloping Passage", "Sloping Sewer Passage");
            oSlopingSewerPassage2.AddPermanentMobs(MobTypeEnum.SewerWolf);
            AddBidirectionalExits(oStagnantCesspool, oSlopingSewerPassage2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oSlopingSewerPassage2, oSmoothedSewerPassage, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSlopingSewerPassage2] = new System.Windows.Point(11, 5);
        }

        private void AddBreeSewers(Room[,] breeStreets, Room[,] breeSewers, out Room oSmoulderingVillage)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];
            RoomGraph breeSewersGraph = _graphs[MapType.BreeSewers];

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
            oLatrine.DamageType = RoomDamageType.Wind;
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
            oEugeneTheExecutioner.AddPermanentItems(ItemTypeEnum.CarvedIvoryKey);
            oEugeneTheExecutioner.DamageType = RoomDamageType.Fire;
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
            oShirriff.AddPermanentItems(ItemTypeEnum.PotHelm, ItemTypeEnum.Torch);

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
            oSewerPassageToSewerDemon.DamageType = RoomDamageType.Earth;
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
            breeSewersGraph.Rooms[oSmoulderingVillage] = new System.Windows.Point(1, -1);

            Room oFirePit = AddRoom("Fire Pit", "Fire Pit");
            AddExit(oSmoulderingVillage, oFirePit, "fire pit");
            AddExit(oFirePit, oSmoulderingVillage, "village");
            breeSewersGraph.Rooms[oFirePit] = new System.Windows.Point(0, -1);

            Room oCeremonialPit = AddRoom("Ceremonial Pit", "Ceremonial Pit");
            e = AddExit(oFirePit, oCeremonialPit, "down");
            e.Hidden = true;
            AddExit(oCeremonialPit, oFirePit, "fire");
            breeSewersGraph.Rooms[oCeremonialPit] = new System.Windows.Point(0, -0.5);

            Room oMasterCeremonyRoom = AddRoom("Ceremony Room", "Master Ceremony Room");
            AddExit(oCeremonialPit, oMasterCeremonyRoom, "west");
            AddExit(oMasterCeremonyRoom, oCeremonialPit, "pit");
            AddExit(oMasterCeremonyRoom, oSmoulderingVillage, "village");
            breeSewersGraph.Rooms[oMasterCeremonyRoom] = new System.Windows.Point(0, 0);

            Room oBurntHut = AddRoom("Burnt Hut", "Burnt Hut");
            AddBidirectionalExitsWithOut(oSmoulderingVillage, oBurntHut, "hut");
            breeSewersGraph.Rooms[oBurntHut] = new System.Windows.Point(2, -1);

            Room oEaldsHideout = AddRoom("Eald the Wise", "Eald's Hideout");
            oEaldsHideout.AddPermanentMobs(MobTypeEnum.EaldTheWise);
            e = AddBidirectionalExitsWithOut(oBurntHut, oEaldsHideout, "sliding");
            e.Hidden = true;
            e.MustOpen = true;
            e = AddExit(oEaldsHideout, oBurnedRemainsOfNimrodel, "trap door");
            e.IsUnknownKnockableKeyType = true;
            AddExit(oBurnedRemainsOfNimrodel, oEaldsHideout, "up");
            breeSewersGraph.Rooms[oEaldsHideout] = new System.Windows.Point(2, 0);

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
            e.KeyType = ItemTypeEnum.KasnarsRedKey;
            e.MustOpen = true;

            Room oOldMansReadingRoom = AddRoom("Reading Room", "Old man's reading room");
            AddBidirectionalSameNameExit(oBurnedRemainsOfNimrodel, oOldMansReadingRoom, "hallway");
            breeSewersGraph.Rooms[oOldMansReadingRoom] = new System.Windows.Point(3, 0);
            //CSRTODO: safe
        }

        private void AddGridBidirectionalExits(Room[,] grid, int x, int y)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];
            Room r = grid[x, y];
            if (r != null)
            {
                breeStreetsGraph.Rooms[r] = new System.Windows.Point(x, 10 - y);

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
        /// <param name="oConstructionSite">construction site</param>
        private void AddMayorMillwoodMansion(Room oConstructionSite, RoomGraph breeStreetsGraph)
        {
            RoomGraph graphMillwoodMansion = _graphs[MapType.MillwoodMansion];

            Room oPathToMansion2 = AddRoom("Southern View", "Southern View");
            AddBidirectionalExits(oConstructionSite, oPathToMansion2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion2] = new System.Windows.Point(1, 2);
            graphMillwoodMansion.Rooms[oConstructionSite] = new System.Windows.Point(1, 1);
            breeStreetsGraph.Rooms[oPathToMansion2] = new System.Windows.Point(11, 1.2);
            AddMapBoundaryPoint(oConstructionSite, oPathToMansion2, MapType.BreeStreets, MapType.MillwoodMansion);

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
            Exit e = AddExit(oGrandPorch, oMansionInside1, "door");
            e.MustOpen = true;
            e.MaximumLevel = 12;
            e = AddExit(oMansionInside1, oGrandPorch, "door");
            e.MustOpen = true;
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

            Room oWarriorBardMansionNorth = AddRoom("Stairwell Downstairs", "Northern Stairwell");
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

            Room oWarriorBardMansionSouth = AddRoom("Stairwell Downstairs", "Southern Stairwell");
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

            Room oWarriorBardMansionEast = AddRoom("Grand Staircase", "Grand Staircase");
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
            RoomGraph millwoodMansionUpstairsGraph = _graphs[MapType.MillwoodMansionUpstairs];
            RoomGraph millwoodMansionGraph = _graphs[MapType.MillwoodMansion];

            millwoodMansionUpstairsGraph.Rooms[northStairwell] = new System.Windows.Point(1, 0);
            millwoodMansionUpstairsGraph.Rooms[southStairwell] = new System.Windows.Point(1, 12);
            millwoodMansionUpstairsGraph.Rooms[eastStairwell] = new System.Windows.Point(5, 5);

            Room oGrandStaircaseUpstairs = AddRoom("Grand Staircase", "Grand Staircase");
            AddBidirectionalExits(oGrandStaircaseUpstairs, eastStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oGrandStaircaseUpstairs] = new System.Windows.Point(5, 6);
            millwoodMansionGraph.Rooms[oGrandStaircaseUpstairs] = new System.Windows.Point(10, 10.5);
            AddMapBoundaryPoint(eastStairwell, oGrandStaircaseUpstairs, MapType.MillwoodMansion, MapType.MillwoodMansionUpstairs);

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
            AddBidirectionalExitsWithOut(oNorthCorridor4, oMeditationChamber, "door", true);
            millwoodMansionUpstairsGraph.Rooms[oMeditationChamber] = new System.Windows.Point(0, 2);

            Room oNorthernStairwell = AddRoom("Northern Stairwell", "Northern Stairwell");
            AddBidirectionalExits(oNorthernStairwell, oNorthCorridor4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthernStairwell, northStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oNorthernStairwell] = new System.Windows.Point(1, 1);
            millwoodMansionGraph.Rooms[oNorthernStairwell] = new System.Windows.Point(7, 6);
            AddMapBoundaryPoint(northStairwell, oNorthernStairwell, MapType.MillwoodMansion, MapType.MillwoodMansionUpstairs);

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
            AddBidirectionalExitsWithOut(oSouthCorridor4, oStorageRoom, "door", true);
            millwoodMansionUpstairsGraph.Rooms[oStorageRoom] = new System.Windows.Point(0, 10);

            Room oSouthernStairwell = AddRoom("Southern Stairwell", "Southern Stairwell");
            AddBidirectionalExits(oSouthCorridor4, oSouthernStairwell, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSouthernStairwell, southStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oSouthernStairwell] = new System.Windows.Point(1, 11);
            millwoodMansionGraph.Rooms[oSouthernStairwell] = new System.Windows.Point(7, 16);
            AddMapBoundaryPoint(southStairwell, oSouthernStairwell, MapType.MillwoodMansion, MapType.MillwoodMansionUpstairs);

            Room oMayorMillwood = AddRoom("Mayor Millwood", "Royal Chamber");
            oMayorMillwood.AddPermanentMobs(MobTypeEnum.MayorMillwood);
            AddBidirectionalExitsWithOut(oRoyalHallwayToMayor, oMayorMillwood, "chamber", true);
            millwoodMansionUpstairsGraph.Rooms[oMayorMillwood] = new System.Windows.Point(4, 8);

            Room oChancellorOfProtection = AddRoom("Chancellor of Protection", "The Chancellor of Protection's Chambers");
            oChancellorOfProtection.AddPermanentMobs(MobTypeEnum.ChancellorOfProtection);
            AddBidirectionalExitsWithOut(oRoyalHallwayToChancellor, oChancellorOfProtection, "chamber", true);
            millwoodMansionUpstairsGraph.Rooms[oChancellorOfProtection] = new System.Windows.Point(4, 4);
        }

        private void AddBreeToImladris(out Room oOuthouse, Room breeEastGateInside, Room breeEastGateOutside, out Room imladrisWestGateOutside, Room oCemetery)
        {
            RoomGraph breeToImladrisGraph = _graphs[MapType.BreeToImladris];

            breeToImladrisGraph.Rooms[breeEastGateInside] = new System.Windows.Point(2, 4);
            breeToImladrisGraph.Rooms[oCemetery] = new System.Windows.Point(2, 3);

            AddExit(breeEastGateInside, breeEastGateOutside, "gate");
            breeToImladrisGraph.Rooms[breeEastGateOutside] = new System.Windows.Point(3, 4);
            Exit e = AddExit(breeEastGateOutside, breeEastGateInside, "gate");
            e.RequiresDay = true;
            AddMapBoundaryPoint(breeEastGateInside, breeEastGateOutside, MapType.BreeStreets, MapType.BreeToImladris);

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

            Room oPathThroughForest = AddRoom("Forest Path", "Path through Forest");
            AddBidirectionalExits(oGreatEastRoad12, oPathThroughForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oPathThroughForest] = new System.Windows.Point(15, 5);

            Room oForestUnmapped = AddRoom("Forest", Room.UNKNOWN_ROOM);
            AddExit(oPathThroughForest, oForestUnmapped, "forest");
            breeToImladrisGraph.Rooms[oForestUnmapped] = new System.Windows.Point(15, 6);

            Room oGreatEastRoad13 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad12, oGreatEastRoad13, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad13] = new System.Windows.Point(16, 4);

            Room oGreatEastRoad14 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad13, oGreatEastRoad14, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad14] = new System.Windows.Point(17, 4);

            Room oThickFog = AddRoom("Thick Fog", "Thick Fog");
            AddExit(oGreatEastRoad14, oThickFog, "north");
            e = AddExit(oThickFog, oGreatEastRoad12, "east");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oThickFog] = new System.Windows.Point(17, 3);

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
            e = AddExit(oGreatEastRoad10, oDarkFootpath, "north");
            e.Hidden = true;
            AddExit(oDarkFootpath, oGreatEastRoad10, "south");
            AddExit(oNorthBrethilForest2, oDarkFootpath, "east");
            e = AddExit(oDarkFootpath, oNorthBrethilForest2, "west");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oDarkFootpath] = new System.Windows.Point(13, 2);

            Room oNorthBrethilForest3 = AddRoom("Brethil Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest3, oDarkFootpath, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest3] = new System.Windows.Point(13, 1);

            Room oNorthBrethilForest4 = AddRoom("Brethil Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest4, oNorthBrethilForest3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest4] = new System.Windows.Point(13, 0);

            Room oNorthBrethilForest5GobAmbush = AddRoom("Gob Ambush #2", "North Brethil Forest");
            oNorthBrethilForest5GobAmbush.AddPermanentMobs(MobTypeEnum.GoblinWarrior, MobTypeEnum.GoblinWarrior, MobTypeEnum.LargeGoblin);
            AddBidirectionalExits(oNorthBrethilForest4, oNorthBrethilForest5GobAmbush, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest5GobAmbush] = new System.Windows.Point(14, 0);

            //South Brethil Forest
            Room oDeepForest = AddRoom("Deep Forest", "Deep Forest");
            e = AddExit(oGreatEastRoad9, oDeepForest, "south");
            e.Hidden = true;
            AddExit(oDeepForest, oGreatEastRoad9, "north");
            breeToImladrisGraph.Rooms[oDeepForest] = new System.Windows.Point(12, 5);

            Room oNathalin = AddRoom("Nathalin", "Trading Post");
            oNathalin.AddPermanentMobs(MobTypeEnum.NathalinTheTrader);
            AddBidirectionalExitsWithOut(oDeepForest, oNathalin, "tree");
            breeToImladrisGraph.Rooms[oNathalin] = new System.Windows.Point(13, 5);

            Room oBrethilForest = AddRoom("Brethil Forest", "Brethil Forest");
            AddBidirectionalExits(oDeepForest, oBrethilForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oBrethilForest] = new System.Windows.Point(12, 6);

            Room oSpriteGuards = AddRoom("Sprite Guards", "Brethil Forest");
            oSpriteGuards.AddPermanentMobs(MobTypeEnum.SpriteGuard, MobTypeEnum.SpriteGuard);
            e = AddExit(oBrethilForest, oSpriteGuards, "brush");
            e.Hidden = true;
            AddExit(oSpriteGuards, oBrethilForest, "east");
            breeToImladrisGraph.Rooms[oSpriteGuards] = new System.Windows.Point(11, 6);
        }

        private void AddToFarmHouseAndUglies(Room oGreatEastRoad1, out Room oOuthouse, RoomGraph breeToImladrisGraph)
        {
            Room oRoadToFarm1 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oGreatEastRoad1, oRoadToFarm1, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm1] = new System.Windows.Point(4, 5);

            Room oRoadToFarm2 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oRoadToFarm1, oRoadToFarm2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm2] = new System.Windows.Point(4, 6);

            Room oWheatField = AddRoom("Wheat Field", "Wheat Field");
            AddBidirectionalExits(oWheatField, oRoadToFarm2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oWheatField] = new System.Windows.Point(3, 6);

            Room oCornField = AddRoom("Corn Field", "Corn Field");
            AddBidirectionalExits(oCornField, oWheatField, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oCornField] = new System.Windows.Point(2, 5.75);

            Room oLembasField = AddRoom("Lembas Field", "Lembas Field");
            AddBidirectionalExits(oWheatField, oLembasField, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oLembasField] = new System.Windows.Point(2, 6.25);

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

            Room oGrainSilo = AddRoom("Grain Silo", "Grain Silo");
            AddBidirectionalExits(oRoadToFarm6, oGrainSilo, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGrainSilo] = new System.Windows.Point(2, 9);

            oOuthouse = AddRoom("Outhouse", "Outhouse");
            oOuthouse.AddPermanentItems(ItemTypeEnum.OutOfOrderSign);
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
            e = AddBidirectionalExitsWithOut(oUglyKidSchoolEntrance, oMuddyFoyer, "front");
            e.MaximumLevel = 10;
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
            breeToImladrisGraph.Rooms[oRoadToFarm7HoundDog] = new System.Windows.Point(2, 7.5);

            Room oFarmParlorManagerMulloyThreshold = AddRoom("Farm Parlor", "Parlor");
            AddBidirectionalSameNameExit(oFarmParlorManagerMulloyThreshold, oRoadToFarm7HoundDog, "door", true);
            breeToImladrisGraph.Rooms[oFarmParlorManagerMulloyThreshold] = new System.Windows.Point(2, 7);

            Room oManagerMulloy = AddRoom("Manager Mulloy", "Study");
            oManagerMulloy.AddPermanentMobs(MobTypeEnum.ManagerMulloy);
            AddBidirectionalExitsWithOut(oFarmParlorManagerMulloyThreshold, oManagerMulloy, "study");
            breeToImladrisGraph.Rooms[oManagerMulloy] = new System.Windows.Point(2, 6.5);

            Room oFarmKitchen = AddRoom("Kitchen", "Kitchen");
            AddExit(oFarmParlorManagerMulloyThreshold, oFarmKitchen, "kitchen");
            AddExit(oFarmKitchen, oFarmParlorManagerMulloyThreshold, "parlor");
            breeToImladrisGraph.Rooms[oFarmKitchen] = new System.Windows.Point(1, 6.5);

            Room oFarmBackPorch = AddRoom("Back Porch", "Back Porch");
            AddExit(oFarmKitchen, oFarmBackPorch, "backdoor");
            AddExit(oFarmBackPorch, oFarmKitchen, "kitchen");
            breeToImladrisGraph.Rooms[oFarmBackPorch] = new System.Windows.Point(1, 7);

            Room oFarmCat = AddRoom("Farm Cat", "The Woodshed");
            oFarmCat.AddPermanentMobs(MobTypeEnum.FarmCat);
            oFarmCat.NoFlee = true;
            AddExit(oFarmBackPorch, oFarmCat, "woodshed");
            e = AddExit(oFarmCat, oFarmBackPorch, "out");
            breeToImladrisGraph.Rooms[oFarmCat] = new System.Windows.Point(1, 7.5);

            Room oCrabbe = AddRoom("Crabbe", "Detention Room");
            oCrabbe.AddPermanentMobs(MobTypeEnum.CrabbeTheClassBully);
            AddBidirectionalExitsWithOut(oHallway, oCrabbe, "detention");
            breeToImladrisGraph.Rooms[oCrabbe] = new System.Windows.Point(7.5, 6.75);

            Room oMrWartnose = AddRoom("Mr. Wartnose", "Mr. Wartnose's Office");
            oMrWartnose.AddPermanentMobs(MobTypeEnum.MrWartnose);
            AddBidirectionalExitsWithOut(oUglyKidClassroomK7, oMrWartnose, "office");
            breeToImladrisGraph.Rooms[oMrWartnose] = new System.Windows.Point(7.5, 7);
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
            AddBidirectionalExits(oGalbasiDownsEntrance, oGalbasiDownsSoutheast, BidirectionalExitType.WestEast);
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
            e.IsTrapExit = true;
            //This always fails is this always the case or am I just using a too low dexterity/level character?
            //e = AddExit(oBarrow, oTopOfHill, "up");
            //e.IsTrapExit = true;
            breeToImladrisGraph.Rooms[oBarrow] = new System.Windows.Point(5, 2);

            Room oAntechamber = AddRoom("Antechamber DMG", "Antechamber");
            AddExit(oBarrow, oAntechamber, "altar");
            AddExit(oAntechamber, oBarrow, "up");
            oAntechamber.DamageType = RoomDamageType.Fire;
            breeToImladrisGraph.Rooms[oAntechamber] = new System.Windows.Point(5, 1.75);

            Room oGalbasiHalls = AddRoom("Galbasi Halls", "Galbasi Halls");
            e = AddExit(oAntechamber, oGalbasiHalls, "stairway");
            e.Hidden = true;
            AddExit(oGalbasiHalls, oAntechamber, "stairway");
            breeToImladrisGraph.Rooms[oGalbasiHalls] = new System.Windows.Point(5, 1.5);

            Room oUnderHallsCorridorsGreenSlime = AddRoom("Green Slime", "Underhalls Corridors");
            oUnderHallsCorridorsGreenSlime.AddPermanentMobs(MobTypeEnum.GreenSlime, MobTypeEnum.GreenSlime);
            AddBidirectionalExits(oUnderHallsCorridorsGreenSlime, oGalbasiHalls, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderHallsCorridorsGreenSlime] = new System.Windows.Point(5, 1.25);

            Room oUnderHallsCorridorsBaseJunction = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderHallsCorridorsBaseJunction, oUnderHallsCorridorsGreenSlime, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderHallsCorridorsBaseJunction] = new System.Windows.Point(5, 1);

            Room oUnderhallsCorridorsWest = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsWest, oUnderHallsCorridorsBaseJunction, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsWest] = new System.Windows.Point(4.33, 1);

            Room oDarkCorner = AddRoom("Skeleton", "Dark Corner");
            oDarkCorner.AddPermanentMobs(MobTypeEnum.Skeleton);
            AddBidirectionalExits(oDarkCorner, oUnderhallsCorridorsWest, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDarkCorner] = new System.Windows.Point(3.67, 1);

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
            oBlackEyeOrcDwelling.AddPermanentMobs(MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc);
            e = AddBidirectionalExitsWithOut(oUnderhallsIronDoor, oBlackEyeOrcDwelling, "iron");
            e.MustOpen = true;
            e.IsUnknownKnockableKeyType = true;
            e.IsTrapExit = true;
            breeToImladrisGraph.Rooms[oBlackEyeOrcDwelling] = new System.Windows.Point(9, -0.5);

            Room oUnderhallsCorridorsNE = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsNE, oUnderHallsCorridorsBaseJunction, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsNE] = new System.Windows.Point(6, 0);

            Room oUnderhallsCorridorsN = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsN, oUnderhallsCorridorsNE, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsN] = new System.Windows.Point(6, -0.5);

            Room oOrcsQuarry = AddRoom("Orcs' Quarry", "Orcs' Quarry");
            AddBidirectionalExits(oUnderhallsCorridorsNE, oOrcsQuarry, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oOrcsQuarry] = new System.Windows.Point(8, 0);

            Room oOrcsQuarry2 = AddRoom("Orc Guard", "Orcs' Quarry");
            oOrcsQuarry2.AddPermanentMobs(MobTypeEnum.OrcGuard, MobTypeEnum.OrcMiner, MobTypeEnum.OrcMiner, MobTypeEnum.OrcMiner);
            AddBidirectionalExits(oOrcsQuarry, oOrcsQuarry2, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oUnderhallsCorridorsToQuarry, oOrcsQuarry2, BidirectionalExitType.UpDown);
            breeToImladrisGraph.Rooms[oOrcsQuarry2] = new System.Windows.Point(8, 0.5);

            Room oUnderhallsCorridorsFromGreenSlime = AddRoom("Coridor", "Underhalls Corridors");
            e = AddExit(oUnderHallsCorridorsGreenSlime, oUnderhallsCorridorsFromGreenSlime, "west");
            e.Hidden = true;
            AddExit(oUnderhallsCorridorsFromGreenSlime, oUnderHallsCorridorsGreenSlime, "east");
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsFromGreenSlime] = new System.Windows.Point(3, 1.25);

            Room oUnderhallsCorridorsToStoneDoor1 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToStoneDoor1, oUnderhallsCorridorsFromGreenSlime, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToStoneDoor1] = new System.Windows.Point(3, 0);

            Room oUnderhallsCorridorsToStoneDoor2 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToStoneDoor2, oUnderhallsCorridorsToStoneDoor1, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToStoneDoor2] = new System.Windows.Point(3, -1);

            Room oAmbushedParty = AddRoom("Ambushed Party", "Ambushed Party");
            oAmbushedParty.AddPermanentMobs(MobTypeEnum.Ghoul, MobTypeEnum.Ghoul, MobTypeEnum.Ghoul);
            e = AddExit(oUnderhallsCorridorsToStoneDoor2, oAmbushedParty, "east");
            e.Hidden = true;
            AddExit(oAmbushedParty, oUnderhallsCorridorsToStoneDoor2, "west");
            breeToImladrisGraph.Rooms[oAmbushedParty] = new System.Windows.Point(4, -1);

            Room oGhostOfMuzgash = AddRoom("Muzgash ghost", "Damp Cell");
            oGhostOfMuzgash.AddPermanentMobs(MobTypeEnum.GhostOfMuzgash);
            e = AddExit(oAmbushedParty, oGhostOfMuzgash, "door");
            e.Hidden = true;
            AddExit(oGhostOfMuzgash, oAmbushedParty, "door");
            breeToImladrisGraph.Rooms[oGhostOfMuzgash] = new System.Windows.Point(4, -1.5);

            Room oUndeadFeastingGrounds = AddRoom("Feasting Grounds", "Undead Feasting Grounds");
            e = AddExit(oAmbushedParty, oUndeadFeastingGrounds, "east");
            e.Hidden = true;
            AddExit(oUndeadFeastingGrounds, oAmbushedParty, "west");
            breeToImladrisGraph.Rooms[oUndeadFeastingGrounds] = new System.Windows.Point(5, -1);

            Room oHallOfTheDead = AddRoom("Hall of the Dead", "Hall of the Dead");
            AddBidirectionalExits(oUndeadFeastingGrounds, oHallOfTheDead, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oHallOfTheDead] = new System.Windows.Point(6, -1);

            Room oLichsLair = AddRoom("Lich's Lair", "Lich's Lair");
            oLichsLair.AddPermanentMobs(MobTypeEnum.MinorLich, MobTypeEnum.MinorLich);
            e = AddExit(oHallOfTheDead, oLichsLair, "hidden");
            e.Hidden = true;
            e.FloatRequirement = FloatRequirement.Levitation;
            e.IsTrapExit = true;
            AddExit(oLichsLair, oHallOfTheDead, "walkway");
            breeToImladrisGraph.Rooms[oLichsLair] = new System.Windows.Point(6, -1.5);

            Room oBottomOfAbyss = AddRoom("Bottom of Abyss", "Bottom of Abyss");
            e = AddExit(oHallOfTheDead, oBottomOfAbyss, "hidden");
            e.Hidden = true;
            e.FloatRequirement = FloatRequirement.NoLevitation;
            e.IsTrapExit = true;
            breeToImladrisGraph.Rooms[oBottomOfAbyss] = new System.Windows.Point(7, -1.5);

            Room oUnderhallsCorridorsStoneDoor = AddRoom("To Stone Door", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsStoneDoor, oUnderhallsCorridorsToStoneDoor2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsStoneDoor] = new System.Windows.Point(3, -2);

            Room oUnderhallsBugbearsLair = AddRoom("Bugbear Lair", "Bugbears' Lair");
            oUnderhallsBugbearsLair.AddPermanentMobs(MobTypeEnum.Bugbear, MobTypeEnum.Bugbear, MobTypeEnum.Bugbear);
            e = AddExit(oUnderhallsCorridorsStoneDoor, oUnderhallsBugbearsLair, "stone");
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            e = AddExit(oUnderhallsBugbearsLair, oUnderhallsCorridorsStoneDoor, "stone");
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            breeToImladrisGraph.Rooms[oUnderhallsBugbearsLair] = new System.Windows.Point(4, -2);

            Room oHiddenCubicle = AddRoom("Hidden Cubicle", "Hidden Cubicle");
            e = AddBidirectionalExitsWithOut(oUnderhallsBugbearsLair, oHiddenCubicle, "hidden");
            e.Hidden = true;
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            breeToImladrisGraph.Rooms[oHiddenCubicle] = new System.Windows.Point(5, -2);

            Room oUnderhallsCorridorsToOtherDoor1 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor1, oUnderhallsCorridorsFromGreenSlime, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor1] = new System.Windows.Point(2, 0);

            Room oUnderhallsCorridorsToOtherDoor2 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor2, oUnderhallsCorridorsToOtherDoor1, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor2] = new System.Windows.Point(1, 0);

            Room oDenseFog = AddRoom("Dense Fog", "Dense Fog");
            AddBidirectionalExits(oDenseFog, oUnderhallsCorridorsToOtherDoor2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDenseFog] = new System.Windows.Point(0, 0);

            Room oUnderhallsCorridorsToOtherDoor3 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor3, oDenseFog, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor3] = new System.Windows.Point(-1, 0);

            Room oUnderhallsCorridorsToOtherDoor4 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor4, oUnderhallsCorridorsToOtherDoor3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor4] = new System.Windows.Point(-1, -1);

            Room oUnderhallsCorridorsOtherDoor = AddRoom("To Door", "Underhalls Corridors");
            oUnderhallsCorridorsOtherDoor.AddPermanentMobs(MobTypeEnum.DoorMimic);
            AddBidirectionalExits(oUnderhallsCorridorsOtherDoor, oUnderhallsCorridorsToOtherDoor4, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsOtherDoor] = new System.Windows.Point(-1, -2);

            Room oDisposalPit = AddRoom("Disposal Pit", "Disposal Pit");
            AddBidirectionalExitsWithOut(oUnderhallsCorridorsOtherDoor, oDisposalPit, "door");
            breeToImladrisGraph.Rooms[oDisposalPit] = new System.Windows.Point(0, -2);

            Room oInsideThePit = AddRoom("Inside Pit", "Inside the Pit");
            oInsideThePit.AddPermanentMobs(MobTypeEnum.Otyugh, MobTypeEnum.Otyugh);
            AddExit(oDisposalPit, oInsideThePit, "trapdoor");
            AddExit(oInsideThePit, oDisposalPit, "up");
            breeToImladrisGraph.Rooms[oInsideThePit] = new System.Windows.Point(1, -2);

            Room oUnderhallsToAntechamber = AddRoom("To Antechamber", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsFromGreenSlime, oUnderhallsToAntechamber, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsToAntechamber] = new System.Windows.Point(3, 2);

            Room oUnderhallsAntechamber = AddRoom("Antechamber", "Antechamber");
            oUnderhallsAntechamber.AddPermanentMobs(MobTypeEnum.Dervish);
            AddBidirectionalExits(oUnderhallsToAntechamber, oUnderhallsAntechamber, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsAntechamber] = new System.Windows.Point(4, 2);
        }

        private void AddImladrisCity(out Room oImladrisSouthGateInside, out Room oEastGateOfImladrisOutside, Room imladrisWestGateOutside, out Room healingHand, out Room oEastGateOfImladrisInside)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];
            RoomGraph breeToImladrisGraph = _graphs[MapType.BreeToImladris];

            Room imladrisWestGateInside = AddRoom("West Gate Inside", "West Gate of Imladris");
            AddExit(imladrisWestGateInside, imladrisWestGateOutside, "gate");
            Exit e = AddExit(imladrisWestGateOutside, imladrisWestGateInside, "gate");
            e.RequiresDay = true;
            imladrisGraph.Rooms[imladrisWestGateOutside] = new System.Windows.Point(-1, 5);
            imladrisGraph.Rooms[imladrisWestGateInside] = new System.Windows.Point(0, 5);
            breeToImladrisGraph.Rooms[imladrisWestGateInside] = new System.Windows.Point(19, 4);
            AddMapBoundaryPoint(imladrisWestGateOutside, imladrisWestGateInside, MapType.BreeToImladris, MapType.Imladris);

            Room oImladrisCircle1 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle1, imladrisWestGateInside, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle1] = new System.Windows.Point(5D / 3, 5 - (4D / 3));

            Room oImladrisCircle2 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle2, oImladrisCircle1, BidirectionalExitType.SouthwestNortheast);
            System.Windows.Point p = new System.Windows.Point(10D / 3, 5 - (8D / 3));
            imladrisGraph.Rooms[oImladrisCircle2] = p;

            Room oAsylumCourtyard = AddRoom("Asylum Courtyard", "Asylum Courtyard");
            AddExit(oImladrisCircle2, oAsylumCourtyard, "east");
            AddExit(oAsylumCourtyard, oImladrisCircle2, "road");
            imladrisGraph.Rooms[oAsylumCourtyard] = new System.Windows.Point(p.X + 1, p.Y);
            AddImladrisAsylum(oAsylumCourtyard);

            Room oImladrisCircle3 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle2, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle3] = new System.Windows.Point(5, 1);

            Room oLoreElesStronghold = AddRoom("Lore Ele's Stronghold", "Lore Ele's Stronghold");
            oLoreElesStronghold.AddPermanentMobs(MobTypeEnum.EleHonorGuard, MobTypeEnum.EleHonorGuard);
            AddBidirectionalSameNameExit(oImladrisCircle3, oLoreElesStronghold, "gate");
            imladrisGraph.Rooms[oLoreElesStronghold] = new System.Windows.Point(5, 0);

            Room oImladrisCircle4 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle4, BidirectionalExitType.SoutheastNorthwest);
            p = new System.Windows.Point(5 + (4D / 3), 1 + (4D / 3));
            imladrisGraph.Rooms[oImladrisCircle4] = p;

            Room oSmithy = AddRoom("Smithy", "Axel's Repair Shoppe");
            AddBidirectionalExits(oSmithy, oImladrisCircle4, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oSmithy] = new System.Windows.Point(p.X - 1, p.Y);

            Room oImladrisCircle5 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle4, oImladrisCircle5, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle5] = new System.Windows.Point(5 + (8D / 3), 1 + (8D / 3));

            Room oImladrisMainStreet1 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(imladrisWestGateInside, oImladrisMainStreet1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet1] = new System.Windows.Point(1, 5);

            Room oImladrisMainStreet2 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet1, oImladrisMainStreet2, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet2] = new System.Windows.Point(2.3, 5);

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
            e = AddExit(oImladrisAlley3, oImladrisAlley4, "south");
            e.Hidden = true;
            AddExit(oImladrisAlley4, oImladrisAlley3, "north");
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

            Room oImladrisPawnShop = AddPawnShoppeRoom("Sharkey's Pawn Shop", "Sharkey's Pawn Shoppe", PawnShoppe.Imladris);
            AddBidirectionalSameNameExit(oImladrisPawnShop, oImladrisSmallAlley2, "door");
            imladrisGraph.Rooms[oImladrisPawnShop] = new System.Windows.Point(5, 3);

            Room oImladrisTownCircle = AddRoom("Town Circle", "Imladris Town Circle");
            AddBidirectionalExits(oImladrisAlley3, oImladrisTownCircle, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisTownCircle] = new System.Windows.Point(7, 5);

            Room oThenardiersInn = AddRoom("Thenardier's Inn", "Thenardier's Inn");
            AddBidirectionalExits(oThenardiersInn, oImladrisTownCircle, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oThenardiersInn] = new System.Windows.Point(7, 4.5);

            Room oImladrisMainStreet6 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisTownCircle, oImladrisMainStreet6, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet6] = new System.Windows.Point(8, 5);

            Room oJewelry = AddRoom("Jewelry", "Zarlow's Fine Jewelry");
            AddBidirectionalExits(oJewelry, oImladrisMainStreet6, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oJewelry] = new System.Windows.Point(8, 4.5);

            Room oPostOffice = AddRoom("Post Office", "Imladris Post Office");
            AddBidirectionalExits(oImladrisMainStreet6, oPostOffice, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oPostOffice] = new System.Windows.Point(8, 5.5);

            oEastGateOfImladrisInside = AddRoom("East Gate Inside", "East Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle5, oEastGateOfImladrisInside, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisMainStreet6, oEastGateOfImladrisInside, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oEastGateOfImladrisInside] = new System.Windows.Point(9, 5);

            oEastGateOfImladrisOutside = AddRoom("East Gate Outside", "Gates of Imladris");
            e = AddExit(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, "gate");
            e.MinimumLevel = 3;
            AddExit(oEastGateOfImladrisOutside, oEastGateOfImladrisInside, "gate");
            imladrisGraph.Rooms[oEastGateOfImladrisOutside] = new System.Windows.Point(10, 5);
            AddMapBoundaryPoint(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, MapType.Imladris, MapType.EastOfImladris);

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
            e = AddExit(oImladrisCircle10, oRearAlley, "east");
            e.Hidden = true;
            AddExit(oRearAlley, oImladrisCircle10, "west");
            AddBidirectionalExits(oRearAlley, oImladrisAlley5, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oRearAlley] = new System.Windows.Point(5, 7);

            Room oPoisonedDagger = AddRoom("Master Assassins", "The Poisoned Dagger");
            oPoisonedDagger.AddPermanentMobs(MobTypeEnum.MasterAssassin, MobTypeEnum.MasterAssassin);
            e = AddExit(oRearAlley, oPoisonedDagger, "door");
            e.Hidden = true;
            AddExit(oPoisonedDagger, oRearAlley, "door");
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

            Room oImladrisArmory = AddRoom("Armory", "Imladris Armory");
            AddBidirectionalExits(oImladrisArmory, oImladrisMainStreet4, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisArmory] = new System.Windows.Point(4, 4.5);

            Room oDriscollsPoolHall = AddRoom("Pool Hall", "Driscoll's Pool Hall");
            oDriscollsPoolHall.AddPermanentMobs(MobTypeEnum.Innkeeper);
            AddBidirectionalExits(oImladrisMainStreet4, oDriscollsPoolHall, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oDriscollsPoolHall] = new System.Windows.Point(4, 5.5);

            Room oImladrisGeneralStore = AddRoom("General Store", "Imladris General Store");
            AddBidirectionalExits(oImladrisGeneralStore, oImladrisMainStreet3, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisGeneralStore] = new System.Windows.Point(3, 4.5);

            Room oImladrisMageClub = AddRoom("Mage Club", "Imladris Mage's Club");
            AddBidirectionalExits(oImladrisMainStreet3, oImladrisMageClub, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisMageClub] = new System.Windows.Point(3, 5.5);

            Room oImladrisBank = AddRoom("Bank", "Bank of Imladris");
            AddBidirectionalExits(oImladrisMainStreet2, oImladrisBank, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisBank] = new System.Windows.Point(2.3, 5.5);

            Room oCombatMall = AddRoom("Combat Mall", "Combat Mall");
            AddBidirectionalExits(oCombatMall, oImladrisMainStreet2, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oCombatMall] = new System.Windows.Point(2.3, 4.2);

            Room oParmasPolearms = AddRoom("Polearms", "Parma's Polearms");
            AddBidirectionalExits(oParmasPolearms, oCombatMall, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oParmasPolearms] = new System.Windows.Point(1.5, 4.2);

            Room oWeaponsmith = AddRoom("Weaponsmith", "Elven Weaponsmith");
            AddBidirectionalExits(oWeaponsmith, oCombatMall, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oWeaponsmith] = new System.Windows.Point(2.3, 3.7);

            Room oArchery = AddRoom("Archery", "Feanaro's Archery Shoppe");
            AddBidirectionalExits(oCombatMall, oArchery, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oArchery] = new System.Windows.Point(3.3, 4.2);

            Room oImladrisCityJail = AddRoom("City Jail", "Imladris City Jail");
            e = AddBidirectionalExitsWithOut(oImladrisAlley4, oImladrisCityJail, "door");
            e.MustOpen = true;
            imladrisGraph.Rooms[oImladrisCityJail] = new System.Windows.Point(7, 6);

            Room oCaveTrollCell = AddRoom("Troll Cell", "Cave Troll Cell");
            oCaveTrollCell.AddPermanentMobs(MobTypeEnum.CaveTroll);
            e = AddBidirectionalExitsWithOut(oImladrisCityJail, oCaveTrollCell, "grate");
            e.MustOpen = true;
            imladrisGraph.Rooms[oCaveTrollCell] = new System.Windows.Point(7, 5.5);
        }

        private void AddImladrisAsylum(Room asylumCourtyard)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];

            Room oAsylumFoyer = AddRoom("Asylum Foyer", "Asylum Foyer");
            AddExit(asylumCourtyard, oAsylumFoyer, "asylum");
            AddExit(oAsylumFoyer, asylumCourtyard, "doors");
            imladrisGraph.Rooms[oAsylumFoyer] = new System.Windows.Point(1, 1.5);

            Room oAsylumWestHallway = AddRoom("West Hallway", "West Hallway");
            AddBidirectionalExits(oAsylumWestHallway, oAsylumFoyer, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oAsylumWestHallway] = new System.Windows.Point(0, 1.5);

            Room oAsylumEastHallway = AddRoom("East Hallway", "East Hallway");
            AddBidirectionalExits(oAsylumFoyer, oAsylumEastHallway, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oAsylumEastHallway] = new System.Windows.Point(2, 1.5);

            Room oNorthCourt = AddRoom("North Court", "North Court");
            AddBidirectionalExits(oNorthCourt, oAsylumFoyer, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oNorthCourt] = new System.Windows.Point(1, 1);

            Room oWestStairwayDownstairs = AddRoom("Stairway", "Stairway");
            AddExit(oAsylumWestHallway, oWestStairwayDownstairs, "staircase");
            AddExit(oWestStairwayDownstairs, oAsylumWestHallway, "hallway");
            imladrisGraph.Rooms[oWestStairwayDownstairs] = new System.Windows.Point(0, 1);

            Room oWestStairwayUpstairs = AddRoom("Stairway", "Stairway");
            AddBidirectionalExits(oWestStairwayUpstairs, oWestStairwayDownstairs, BidirectionalExitType.UpDown);
            imladrisGraph.Rooms[oWestStairwayUpstairs] = new System.Windows.Point(0, 0.5);

            Room oWestHallwayUpstairs = AddRoom("Hallway", "West Hallway");
            AddExit(oWestStairwayUpstairs, oWestHallwayUpstairs, "hallway");
            AddExit(oWestHallwayUpstairs, oWestStairwayUpstairs, "staircase");
            imladrisGraph.Rooms[oWestHallwayUpstairs] = new System.Windows.Point(0, 0);

            Room oUpstairsLobby = AddRoom("Upstairs Lobby", "Upstairs Lobby");
            AddBidirectionalExits(oWestHallwayUpstairs, oUpstairsLobby, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oUpstairsLobby] = new System.Windows.Point(1, 0);

            Room oNorthHallway = AddRoom("North Hallway", "North Hallway");
            Exit e = AddExit(oUpstairsLobby, oNorthHallway, "door");
            e.MustOpen = true;
            AddExit(oNorthHallway, oUpstairsLobby, "door");
            imladrisGraph.Rooms[oNorthHallway] = new System.Windows.Point(1, -0.5);

            Room oAsylumTop = AddRoom("Asylum Top", "The Top of the Asylum");
            e = AddExit(oNorthHallway, oAsylumTop, "ladder");
            e.Hidden = true;
            imladrisGraph.Rooms[oAsylumTop] = new System.Windows.Point(1, -1);

            Room oEastHallwayUpstairs = AddRoom("Hallway", "East Hallway");
            AddBidirectionalExits(oUpstairsLobby, oEastHallwayUpstairs, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oEastHallwayUpstairs] = new System.Windows.Point(2, 0);

            Room oEastStairwayUpstairs = AddRoom("Stairway", "Stairway");
            AddExit(oEastHallwayUpstairs, oEastStairwayUpstairs, "staircase");
            AddExit(oEastStairwayUpstairs, oEastHallwayUpstairs, "hallway");
            imladrisGraph.Rooms[oEastStairwayUpstairs] = new System.Windows.Point(2, 0.5);

            Room oEastStairwayDownstairs = AddRoom("Stairway", "Stairway");
            AddBidirectionalExits(oEastStairwayUpstairs, oEastStairwayDownstairs, BidirectionalExitType.UpDown);
            AddExit(oEastStairwayDownstairs, oAsylumEastHallway, "hallway");
            AddExit(oAsylumEastHallway, oEastStairwayDownstairs, "staircase");
            imladrisGraph.Rooms[oEastStairwayDownstairs] = new System.Windows.Point(2, 1);
        }

        private void AddEastOfImladris(Room oEastGateOfImladrisOutside, Room oEastGateOfImladrisInside, out Room westGateOfEsgaroth)
        {
            RoomGraph eastOfImladrisGraph = _graphs[MapType.EastOfImladris];

            eastOfImladrisGraph.Rooms[oEastGateOfImladrisOutside] = new System.Windows.Point(0, 6);
            eastOfImladrisGraph.Rooms[oEastGateOfImladrisInside] = new System.Windows.Point(-1, 6);

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
            oIorlas.AddPermanentItems(ItemTypeEnum.LittleBrownJug);
            AddExit(oIorlasThreshold, oIorlas, "shack");
            AddExit(oIorlas, oIorlasThreshold, "door");
            eastOfImladrisGraph.Rooms[oIorlas] = new System.Windows.Point(2, 3);
        }

        private void AddBreeToHobbiton(Room oBreeWestGateInside, Room oSmoulderingVillage)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];
            RoomGraph westOfBreeMap = _graphs[MapType.WestOfBree];
            RoomGraph breeSewersMap = _graphs[MapType.BreeSewers];

            westOfBreeMap.Rooms[oBreeWestGateInside] = new System.Windows.Point(15, 0);

            Room oBreeWestGateOutside = AddRoom("West Gate Outside", "West Gate of Bree");
            AddBidirectionalSameNameExit(oBreeWestGateInside, oBreeWestGateOutside, "gate");
            breeStreetsGraph.Rooms[oBreeWestGateOutside] = new System.Windows.Point(-1, 3);
            westOfBreeMap.Rooms[oBreeWestGateOutside] = new System.Windows.Point(14, 0);
            AddMapBoundaryPoint(oBreeWestGateInside, oBreeWestGateOutside, MapType.BreeStreets, MapType.WestOfBree);

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
            oGreatHallOfHeroes.AddPermanentMobs(MobTypeEnum.DenethoreTheWise);
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
            breeSewersMap.Rooms[oShepherd] = new System.Windows.Point(1, -2);

            AddExit(oSmoulderingVillage, oShepherd, "gate");
            e = AddExit(oShepherd, oSmoulderingVillage, "gate");
            e.KeyType = ItemTypeEnum.GateKey; //not actually a usable exit since full key support is not there yet
            westOfBreeMap.Rooms[oSmoulderingVillage] = new System.Windows.Point(13, -2.5);
            AddMapBoundaryPoint(oShepherd, oSmoulderingVillage, MapType.WestOfBree, MapType.BreeSewers);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside, out Room oTharbadGateOutside)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];
            RoomGraph imladrisToTharbadGraph = _graphs[MapType.ImladrisToTharbad];
            RoomGraph spindrilsCastleLevel1Graph = _graphs[MapType.SpindrilsCastleLevel1];
            RoomGraph tharbadGraph = _graphs[MapType.Tharbad];

            Room oMistyTrail1 = AddRoom("South Gate Outside", "Misty Trail");
            AddBidirectionalSameNameExit(oImladrisSouthGateInside, oMistyTrail1, "gate");
            imladrisGraph.Rooms[oMistyTrail1] = new System.Windows.Point(5, 11);
            imladrisToTharbadGraph.Rooms[oMistyTrail1] = new System.Windows.Point(5, 0);
            imladrisToTharbadGraph.Rooms[oImladrisSouthGateInside] = new System.Windows.Point(5, -1);
            AddMapBoundaryPoint(oImladrisSouthGateInside, oMistyTrail1, MapType.Imladris, MapType.ImladrisToTharbad);

            Room oBrunskidTradersGuild1 = AddRoom("Brunskid Guild", "Brunskid Trader's Guild Store Front");
            AddBidirectionalExits(oBrunskidTradersGuild1, oMistyTrail1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oBrunskidTradersGuild1] = new System.Windows.Point(4, 11);
            imladrisToTharbadGraph.Rooms[oBrunskidTradersGuild1] = new System.Windows.Point(4, 0);
            AddRoomMapDisambiguation(oBrunskidTradersGuild1, MapType.ImladrisToTharbad); //on imladris graph for convenience

            Room oGuildmaster = AddRoom("Guildmaster", "Brunskid Trader's Guild Office");
            oGuildmaster.AddPermanentMobs(MobTypeEnum.Guildmaster);
            AddBidirectionalExits(oGuildmaster, oBrunskidTradersGuild1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oGuildmaster] = new System.Windows.Point(3, 11);
            imladrisToTharbadGraph.Rooms[oGuildmaster] = new System.Windows.Point(3, 0);
            AddRoomMapDisambiguation(oGuildmaster, MapType.ImladrisToTharbad); //on imladris graph for convenience

            Room oCutthroatAssassin = AddRoom("Hiester", "Brunskid Trader's Guild Acquisitions Room");
            AddBidirectionalExits(oCutthroatAssassin, oGuildmaster, BidirectionalExitType.WestEast);
            oCutthroatAssassin.AddPermanentMobs(MobTypeEnum.GregoryHiester, MobTypeEnum.MasterAssassin, MobTypeEnum.Cutthroat);
            imladrisGraph.Rooms[oCutthroatAssassin] = new System.Windows.Point(2, 11);
            imladrisToTharbadGraph.Rooms[oCutthroatAssassin] = new System.Windows.Point(2, 0);
            AddRoomMapDisambiguation(oCutthroatAssassin, MapType.ImladrisToTharbad); //on imladris graph for convenience

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
            imladrisToTharbadGraph.Rooms[oMarkFrey] = new System.Windows.Point(3, 4.5);

            Room oMistyTrail5 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail4, oMistyTrail5, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail5] = new System.Windows.Point(4, 4);

            Room oMistyTrail6 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail5, oMistyTrail6, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail6] = new System.Windows.Point(4, 5);

            Room oMistyTrailForest = AddRoom("Forest", Room.UNKNOWN_ROOM);
            AddExit(oMistyTrail6, oMistyTrailForest, "forest");
            imladrisToTharbadGraph.Rooms[oMistyTrailForest] = new System.Windows.Point(3, 5);

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
            tharbadGraph.Rooms[oMistyTrail14] = new System.Windows.Point(3, 0);

            Room oGrassyField = AddRoom("Grassy Field", "Grassy Field");
            oGrassyField.AddNonPermanentMobs(MobTypeEnum.Griffon);
            AddBidirectionalExits(oGrassyField, oMistyTrail14, BidirectionalExitType.SoutheastNorthwest);
            imladrisToTharbadGraph.Rooms[oGrassyField] = new System.Windows.Point(-1, 12);
            spindrilsCastleLevel1Graph.Rooms[oGrassyField] = new System.Windows.Point(11, 10.5);

            Room spindrilsCastleOutside = AddRoom("Dark Clouds", "Dark Clouds");
            Exit e = AddExit(oGrassyField,spindrilsCastleOutside, "up");
            e.Hidden = true;
            e.PresenceType = ExitPresenceType.RequiresSearch;
            AddExit(spindrilsCastleOutside, oGrassyField, "down");
            imladrisToTharbadGraph.Rooms[spindrilsCastleOutside] = new System.Windows.Point(-1, 11);
            AddMapBoundaryPoint(oGrassyField, spindrilsCastleOutside, MapType.ImladrisToTharbad, MapType.SpindrilsCastleLevel1);

            AddSpindrilsCastle(spindrilsCastleOutside);

            oTharbadGateOutside = AddRoom("North Gate", "North Gate of Tharbad");
            AddBidirectionalExits(oMistyTrail14, oTharbadGateOutside, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oTharbadGateOutside] = new System.Windows.Point(0, 14);
            AddMapBoundaryPoint(oMistyTrail14, oTharbadGateOutside, MapType.ImladrisToTharbad, MapType.Tharbad);
        }

        private void AddNorthOfEsgaroth(Room esgarothNorthGateOutside)
        {
            RoomGraph northOfEsgarothGraph = _graphs[MapType.NorthOfEsgaroth];

            northOfEsgarothGraph.Rooms[esgarothNorthGateOutside] = new System.Windows.Point(5, 10);

            Room mountainTrail1 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail1, esgarothNorthGateOutside, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail1] = new System.Windows.Point(6, 9);

            Room mountainTrail2 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail2, mountainTrail1, BidirectionalExitType.NorthSouth);
            northOfEsgarothGraph.Rooms[mountainTrail2] = new System.Windows.Point(6, 8);

            Room mountainTrail3 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail3, mountainTrail2, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail3] = new System.Windows.Point(5, 7);

            Room mountainTrail4 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail4, mountainTrail3, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail4] = new System.Windows.Point(6, 6);

            Room mountainTrail5 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail5, mountainTrail4, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail5] = new System.Windows.Point(5, 5);

            Room mountainTrail6 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail6, mountainTrail5, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail6] = new System.Windows.Point(6, 4);

            Room mountainTrail7 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail6, mountainTrail7, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrail7] = new System.Windows.Point(7, 4);

            Room mountainTrail8 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail7, mountainTrail8, BidirectionalExitType.NorthSouth);
            northOfEsgarothGraph.Rooms[mountainTrail8] = new System.Windows.Point(7, 5);

            Room mountainTrail9 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail8, mountainTrail9, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail9] = new System.Windows.Point(10, 6);

            Room mountainTrail10 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail9, mountainTrail10, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail10] = new System.Windows.Point(9, 7);

            Room mountainTrail11 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail11, mountainTrail10, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrail11] = new System.Windows.Point(8, 7);

            Room mountainTrail12 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail11, mountainTrail12, BidirectionalExitType.NorthSouth);
            northOfEsgarothGraph.Rooms[mountainTrail12] = new System.Windows.Point(8, 9);

            Room mountainTrail13 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail13, mountainTrail12, BidirectionalExitType.WestEast);
            AddBidirectionalExits(mountainTrail2, mountainTrail13, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail13] = new System.Windows.Point(7, 9);

            Room mountainTrailWest1 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest1, mountainTrail3, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrailWest1] = new System.Windows.Point(2, 7);

            Room mountainTrailWest2 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest2, mountainTrailWest1, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrailWest2] = new System.Windows.Point(2.5, 6.5);

            Room mountainTrailWest3 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest3, mountainTrailWest2, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrailWest3] = new System.Windows.Point(3, 6);

            Room mountainTrailWest4 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest3, mountainTrailWest4, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrailWest4] = new System.Windows.Point(4, 6);

            Room mountainTrailWest5 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest5, mountainTrailWest3, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(mountainTrailWest5, mountainTrail5, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrailWest5] = new System.Windows.Point(4, 5);

            Room ambush = AddRoom("Ambush", "Ambush!");
            ambush.AddPermanentMobs(MobTypeEnum.BarbarianGuard, MobTypeEnum.BarbarianGuard, MobTypeEnum.HillGiant, MobTypeEnum.HillGiant, MobTypeEnum.EvilSorcerer, MobTypeEnum.MercenaryCaptain);
            Exit e = AddBidirectionalExitsWithOut(mountainTrailWest4, ambush, "brush");
            e.Hidden = true;
            northOfEsgarothGraph.Rooms[ambush] = new System.Windows.Point(4, 5.5);

            Room disfiguredStatue = AddRoom("Disfigured Statue", "Disfigured Statue");
            AddBidirectionalExitsWithOut(mountainTrail3, disfiguredStatue, "disfigured"); //says disfigured statue but statue doesn't work
            northOfEsgarothGraph.Rooms[disfiguredStatue] = new System.Windows.Point(6, 7);
        }

        private void AddEsgaroth(Room westGateOfEsgaroth, out Room northGateOutside)
        {
            RoomGraph esgarothGraph = _graphs[MapType.Esgaroth];
            RoomGraph eastOfImladrisGraph = _graphs[MapType.EastOfImladris];
            RoomGraph northOfEsgarothGraph = _graphs[MapType.NorthOfEsgaroth];

            esgarothGraph.Rooms[westGateOfEsgaroth] = new System.Windows.Point(0, 7);

            Room plymouthIndigo = AddRoom("Plymouth/Indigo", "Plymouth Road/Indigo Avenue Intersection");
            AddBidirectionalSameNameExit(westGateOfEsgaroth, plymouthIndigo, "gate");
            esgarothGraph.Rooms[plymouthIndigo] = new System.Windows.Point(1, 6);
            eastOfImladrisGraph.Rooms[plymouthIndigo] = new System.Windows.Point(10, 4);
            AddMapBoundaryPoint(westGateOfEsgaroth, plymouthIndigo, MapType.EastOfImladris, MapType.Esgaroth);

            Room cathedralEntrance = AddRoom("Cathedral Entrance", "Cathedral of Worldly Bliss Court");
            AddExit(plymouthIndigo, cathedralEntrance, "cathedral");
            AddExit(cathedralEntrance, plymouthIndigo, "east");
            esgarothGraph.Rooms[cathedralEntrance] = new System.Windows.Point(0, 6);

            Room cathedralNorthFoyer = AddRoom("Foyer", "Northern Entrance Foyer");
            AddBidirectionalExits(cathedralNorthFoyer, cathedralEntrance, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[cathedralNorthFoyer] = new System.Windows.Point(0, 5.5);

            Room cathedralSouthFoyer = AddRoom("Foyer", "Southern Entrance Foyer");
            AddBidirectionalExits(cathedralEntrance, cathedralSouthFoyer, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[cathedralSouthFoyer] = new System.Windows.Point(0, 6.5);

            Room insideCathedral = AddRoom("Cathedral", "Cathedral of Worldly Bliss");
            AddExit(cathedralNorthFoyer, insideCathedral, "door");
            AddExit(cathedralSouthFoyer, insideCathedral, "door");
            AddExit(insideCathedral, cathedralNorthFoyer, "north door");
            AddExit(insideCathedral, cathedralSouthFoyer, "south door");
            esgarothGraph.Rooms[insideCathedral] = new System.Windows.Point(-2, 6);

            Room alliyana = AddRoom("Alliyana", "Matriarch Alliyana's Quarters");
            alliyana.AddPermanentMobs(MobTypeEnum.MatriarchAlliyanaOfIsengard);
            Exit e = AddExit(insideCathedral, alliyana, "east door");
            e.Hidden = true;
            e.MustOpen = true;
            e = AddExit(alliyana, insideCathedral, "out");
            e.MustOpen = true;
            esgarothGraph.Rooms[alliyana] = new System.Windows.Point(-1, 6);

            Room holyBankOfEsgaroth = AddRoom("Holy Bank", "Holy Bank of Esgaroth");
            AddBidirectionalExits(plymouthIndigo, holyBankOfEsgaroth, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[holyBankOfEsgaroth] = new System.Windows.Point(1, 7);

            Room plymouthMagenta = AddRoom("Plymouth/Magenta", "Plymouth Road/Magenta Avenue Intersection");
            AddBidirectionalExits(plymouthIndigo, plymouthMagenta, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthMagenta] = new System.Windows.Point(2, 6);

            Room plymouthGardenCir = AddRoom("Plymouth/Garden", "Plymouth Road/Garden Circle West");
            AddBidirectionalExits(plymouthMagenta, plymouthGardenCir, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthGardenCir] = new System.Windows.Point(3, 6);

            Room plymouthAquamarine = AddRoom("Plymouth/Aquamarine", "Plymouth Road/Aquamarine Way");
            AddBidirectionalExits(plymouthGardenCir, plymouthAquamarine, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthAquamarine] = new System.Windows.Point(4, 6);

            Room plymouthFuchsia = AddRoom("Plymouth/Fuchsia", "Plymouth Road/Fuchsia Way");
            AddBidirectionalExits(plymouthAquamarine, plymouthFuchsia, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthFuchsia] = new System.Windows.Point(8, 6);

            Room theaterGrassSeating = AddRoom("Theater Grass Seating", "Esgaroth Amphitheather Grass Seating");
            AddBidirectionalExits(plymouthFuchsia, theaterGrassSeating, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[theaterGrassSeating] = new System.Windows.Point(9, 6);

            Room briarIndigo = AddRoom("Briar/Indigo", "Briar Lane/Indigo Avenue Intersection");
            AddBidirectionalExits(briarIndigo, plymouthIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarIndigo] = new System.Windows.Point(1, 5);

            Room briarMagenta = AddRoom("Briar/Magenta", "Briar Lane/Magenta Avenue Intersection");
            AddBidirectionalExits(briarIndigo, briarMagenta, BidirectionalExitType.WestEast);
            AddBidirectionalExits(briarMagenta, plymouthMagenta, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarMagenta] = new System.Windows.Point(2, 5);

            Room briarLane = AddRoom("Briar", "Briar Lane");
            AddBidirectionalExits(briarMagenta, briarLane, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[briarLane] = new System.Windows.Point(3, 5);

            Room parthTowers = AddRoom("Parth Towers", "Entrance to the Parth Towers");
            parthTowers.AddPermanentItems(ItemTypeEnum.BlackRune, ItemTypeEnum.BlueRune, ItemTypeEnum.GreenRune, ItemTypeEnum.GreyRune);
            AddBidirectionalExits(parthTowers, briarLane, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[parthTowers] = new System.Windows.Point(3, 4.5);
            //CSRTODO: drawbridge

            Room briarAquamarine = AddRoom("Briar/Aquamarine", "Briar Lane/Aquamarine Way Intersection");
            AddBidirectionalExits(briarLane, briarAquamarine, BidirectionalExitType.WestEast);
            AddBidirectionalExits(briarAquamarine, plymouthAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarAquamarine] = new System.Windows.Point(4, 5);

            Room briarLane2 = AddRoom("Briar", "Briar Lane");
            AddBidirectionalExits(briarAquamarine, briarLane2, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[briarLane2] = new System.Windows.Point(6, 5);
            AddEsgarothMuseum(briarLane2, esgarothGraph);

            Room briarFuchsia = AddRoom("Briar/Fuchsia", "Briar Lane/Fuchsia Way Intersection");
            AddBidirectionalExits(briarLane2, briarFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(briarFuchsia, plymouthFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarFuchsia] = new System.Windows.Point(8, 5);

            Room theaterMainSeating = AddRoom("Theater Main Seating", "Esgaroth Amphitheater Main Seating");
            theaterMainSeating.AddPermanentMobs(MobTypeEnum.MinstrelOfEsgaroth);
            AddBidirectionalExits(briarFuchsia, theaterMainSeating, BidirectionalExitType.WestEast);
            AddBidirectionalExits(theaterGrassSeating, theaterMainSeating, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[theaterMainSeating] = new System.Windows.Point(9, 5);

            Room dragonpawFuchsia = AddRoom("Dragonpaw/Fuchsia", "Dragonpaw Lane/Fuchsia Way Intersection");
            AddBidirectionalExits(dragonpawFuchsia, briarFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[dragonpawFuchsia] = new System.Windows.Point(8, 4.5);

            Room dragonpaw = AddRoom("Dragonpaw", "Dragonpaw Lane");
            AddBidirectionalExits(dragonpawFuchsia, dragonpaw, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[dragonpaw] = new System.Windows.Point(9, 4.5);

            Room theaterStage = AddRoom("Theater Stage", "Esgaroth Amphitheater Stage");
            AddBidirectionalExits(dragonpaw, theaterStage, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(theaterStage, theaterMainSeating, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[theaterStage] = new System.Windows.Point(9, 4.75);

            Room sweetwaterIndigo = AddRoom("Sweetwater/Indigo", "Sweetwater Lane/Indigo Avenue Intersection");
            AddBidirectionalExits(sweetwaterIndigo, briarIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterIndigo] = new System.Windows.Point(1, 4);

            Room sweetwaterMagenta = AddRoom("Sweetwater/Magenta", "Sweetwater Lane/Magenta Avenue Intersection");
            AddBidirectionalExits(sweetwaterIndigo, sweetwaterMagenta, BidirectionalExitType.WestEast);
            AddBidirectionalExits(sweetwaterMagenta, briarMagenta, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterMagenta] = new System.Windows.Point(2, 4);

            Room sweetwaterAquamarine = AddRoom("Sweetwater/Aquamarine", "Sweetwater Lane/Aquamarine Way Intersection");
            AddBidirectionalExits(sweetwaterMagenta, sweetwaterAquamarine, BidirectionalExitType.WestEast);
            AddBidirectionalExits(sweetwaterAquamarine, briarAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterAquamarine] = new System.Windows.Point(4, 4);

            Room sweetwaterLane = AddRoom("Sweetwater", "Sweetwater Lane");
            AddBidirectionalExits(sweetwaterAquamarine, sweetwaterLane, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[sweetwaterLane] = new System.Windows.Point(6, 4);

            Room marketplaceLane = AddRoom("Marketplace", "Marketplace Lane");
            AddBidirectionalExits(sweetwaterLane, marketplaceLane, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(marketplaceLane, briarLane2, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[marketplaceLane] = new System.Windows.Point(6, 4.5);

            Room marketplaceAlley = AddRoom("Alley", "Marketplace Alley");
            AddBidirectionalExits(marketplaceAlley, marketplaceLane, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[marketplaceAlley] = new System.Windows.Point(5, 4.5);

            Room gypsyRow = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(marketplaceLane, gypsyRow, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[gypsyRow] = new System.Windows.Point(7, 4.5);

            Room sweetwaterFuchsia = AddRoom("Sweetwater/Fuchsia", "Sweetwater Lane/Fuchsia Way");
            AddBidirectionalExits(sweetwaterLane, sweetwaterFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(sweetwaterFuchsia, dragonpawFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterFuchsia] = new System.Windows.Point(8, 4);

            Room oEsgarothPawnShop = AddPawnShoppeRoom("Pawnshop", "Esgaroth Pawnshop", PawnShoppe.Esgaroth);
            AddBidirectionalSameNameExit(sweetwaterFuchsia, oEsgarothPawnShop, "door");
            esgarothGraph.Rooms[oEsgarothPawnShop] = new System.Windows.Point(7, 3.5);

            Room parthRecreationField = AddRoom("Recreation Field", "Parth Recreation Field");
            AddBidirectionalExits(sweetwaterFuchsia, parthRecreationField, BidirectionalExitType.WestEast);
            AddBidirectionalExits(parthRecreationField, dragonpaw, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[parthRecreationField] = new System.Windows.Point(9, 4);

            Room frostIndigo = AddRoom("Frost/Indigo", "Frost Lane/Indigo Avenue Intersection");
            AddBidirectionalExits(frostIndigo, sweetwaterIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostIndigo] = new System.Windows.Point(1, 3);

            Room frostMagenta = AddRoom("Frost/Magenta", "Frost Lane/Magenta Avenue Intersection");
            AddBidirectionalExits(frostIndigo, frostMagenta, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostMagenta, sweetwaterMagenta, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostMagenta] = new System.Windows.Point(2, 3);

            Room frostWestLake = AddRoom("Frost/WestLake", "Frost Lane/West Lake Circle");
            AddBidirectionalExits(frostMagenta, frostWestLake, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[frostWestLake] = new System.Windows.Point(3, 3);

            Room frostAquamarine = AddRoom("Frost/Aquamarine", "Frost Lane/Aquamarine Way Intersection");
            AddBidirectionalExits(frostWestLake, frostAquamarine, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostAquamarine, sweetwaterAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostAquamarine] = new System.Windows.Point(4, 3);

            Room frostFuchsia = AddRoom("Frost/Fuchsia", "Frost Lane/Fuchsia Way Intersection");
            AddBidirectionalExits(frostAquamarine, frostFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostFuchsia, sweetwaterFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostFuchsia] = new System.Windows.Point(8, 3);

            Room frostEastLake = AddRoom("Frost/EastLake", "Frost Lane/East Lake Circle");
            AddBidirectionalExits(frostFuchsia, frostEastLake, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostEastLake, parthRecreationField, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostEastLake] = new System.Windows.Point(9, 3);

            Room northEntranceInside = AddRoom("North Gate Inside", "North Entrance to Esgaroth");
            AddBidirectionalExits(northEntranceInside, frostIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[northEntranceInside] = new System.Windows.Point(1, 2);
            northOfEsgarothGraph.Rooms[northEntranceInside] = new System.Windows.Point(5, 11);

            northGateOutside = AddRoom("North Gate Outside", "North Entrance to Esgaroth");
            AddBidirectionalSameNameExit(northEntranceInside, northGateOutside, "gate");
            esgarothGraph.Rooms[northGateOutside] = new System.Windows.Point(1, 1.5);
            AddMapBoundaryPoint(northEntranceInside, northGateOutside, MapType.Esgaroth, MapType.NorthOfEsgaroth);

            Room stablesExerciseYard = AddRoom("Stables/Exercise", "Esgaroth Stables and Exercise Yard");
            AddBidirectionalExits(northEntranceInside, stablesExerciseYard, BidirectionalExitType.WestEast);
            AddExit(stablesExerciseYard, frostMagenta, "south");
            esgarothGraph.Rooms[stablesExerciseYard] = new System.Windows.Point(2, 2);

            Room lakeCircle = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(stablesExerciseYard, lakeCircle, BidirectionalExitType.WestEast);
            AddBidirectionalExits(lakeCircle, frostWestLake, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[lakeCircle] = new System.Windows.Point(3, 2);

            Room dignitaryStables = AddRoom("Dignitary Stables", "Dignitary Stables");
            AddBidirectionalExits(lakeCircle, dignitaryStables, BidirectionalExitType.WestEast);
            AddBidirectionalExits(dignitaryStables, frostAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[dignitaryStables] = new System.Windows.Point(4, 2);

            Room dairyProduction = AddRoom("Dairy Facility", "Dairy Production and Storage Facility");
            AddBidirectionalExits(dignitaryStables, dairyProduction, BidirectionalExitType.WestEast);
            AddBidirectionalExits(dairyProduction, frostFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[dairyProduction] = new System.Windows.Point(8, 2);

            Room lakeCircle2 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(dairyProduction, lakeCircle2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(lakeCircle2, frostEastLake, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[lakeCircle2] = new System.Windows.Point(9, 2);

            Room lakeParthShore = AddHealingRoom("Shore", "Lake Parth Shore", HealingRoom.Esgaroth);
            AddBidirectionalExits(lakeParthShore, lakeCircle2, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(lakeParthShore, dairyProduction, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(lakeParthShore, lakeCircle, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeParthShore] = new System.Windows.Point(8, 1);

            Room lakeViewDrive1 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive1, lakeParthShore, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeViewDrive1] = new System.Windows.Point(9, 0);

            Room lakeViewDrive2 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive2, lakeViewDrive1, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeViewDrive2] = new System.Windows.Point(10, -1);

            Room lakeViewDrive3 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive3, lakeViewDrive2, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[lakeViewDrive3] = new System.Windows.Point(10, -2);

            Room lakeViewDrive4 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive4, lakeViewDrive3, BidirectionalExitType.SoutheastNorthwest);
            esgarothGraph.Rooms[lakeViewDrive4] = new System.Windows.Point(7, -3);

            Room lakeViewDrive5 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive5, lakeViewDrive4, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[lakeViewDrive5] = new System.Windows.Point(6, -3);

            Room lakeCircle3 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeViewDrive5, lakeCircle3, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeCircle3] = new System.Windows.Point(5, -2);

            Room lakeCircle4 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeCircle3, lakeCircle4, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeCircle4] = new System.Windows.Point(1.5, 0.5);

            Room lakeCircle5 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeCircle4, lakeCircle5, BidirectionalExitType.SoutheastNorthwest);
            esgarothGraph.Rooms[lakeCircle5] = new System.Windows.Point(2, 1);

            Room lakeCircle6 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeCircle5, lakeCircle6, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(lakeCircle6, lakeCircle, BidirectionalExitType.SoutheastNorthwest);
            esgarothGraph.Rooms[lakeCircle6] = new System.Windows.Point(2.5, 1.5);

            Room prospectorInn = AddRoom("Prospector Inn", "Prospector Inn");
            AddBidirectionalSameNameExit(frostWestLake, prospectorInn, "door");
            esgarothGraph.Rooms[prospectorInn] = new System.Windows.Point(3, 3.25);

            Room redDragonTavern = AddRoom("Red Dragon Tavern", "The Red Dragon Tavern");
            AddBidirectionalSameNameExit(prospectorInn, redDragonTavern, "red"); //says red door but door doesn't work
            AddBidirectionalSameNameExit(redDragonTavern, sweetwaterAquamarine, "door");
            esgarothGraph.Rooms[redDragonTavern] = new System.Windows.Point(3, 3.5);

            Room jaysSmithShoppe = AddRoom("Smithy", "Jay's Smith Shoppe");
            jaysSmithShoppe.AddPermanentMobs(MobTypeEnum.Smithy);
            AddBidirectionalSameNameExit(frostMagenta, jaysSmithShoppe, "door");
            esgarothGraph.Rooms[jaysSmithShoppe] = new System.Windows.Point(2.33, 2.75);

            Room foundry = AddRoom("Foundry", "Foundry");
            foundry.AddPermanentMobs(MobTypeEnum.SivalTheArtificer);
            AddBidirectionalExitsWithOut(jaysSmithShoppe, foundry, "foundry");
            esgarothGraph.Rooms[foundry] = new System.Windows.Point(2.33, 2.5);

            Room archeryRange = AddRoom("Archery Range", "Archery Range");
            AddBidirectionalExits(plymouthMagenta, archeryRange, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[archeryRange] = new System.Windows.Point(2, 7);

            Room gardenCircle = AddRoom("Garden Circle", "Garden Circle Road");
            AddBidirectionalExits(archeryRange, gardenCircle, BidirectionalExitType.WestEast);
            AddBidirectionalExits(plymouthGardenCir, gardenCircle, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[gardenCircle] = new System.Windows.Point(3, 7);

            Room gardenCircle2 = AddRoom("Garden Circle", "Garden Circle Road");
            AddBidirectionalExits(gardenCircle, gardenCircle2, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[gardenCircle2] = new System.Windows.Point(6, 7);

            Room gardenCirFuchsia = AddRoom("Garden/Fuchsia", "Garden Circle Road/Fuchsia Way");
            AddBidirectionalExits(gardenCircle2, gardenCirFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(plymouthFuchsia, gardenCirFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[gardenCirFuchsia] = new System.Windows.Point(8, 7);

            Room southGate = AddRoom("South Gate", "South Gate of Esgaroth");
            AddBidirectionalSameNameExit(gardenCirFuchsia, southGate, "gate");
            esgarothGraph.Rooms[southGate] = new System.Windows.Point(8, 8);

            Room archeryEmporium = AddRoom("Archery Emporium", "Elena's Archery Emporium");
            AddExit(archeryRange, archeryEmporium, "door");
            AddExit(archeryEmporium, archeryRange, "range");
            AddBidirectionalSameNameExit(archeryEmporium, gardenCircle, "door");
            esgarothGraph.Rooms[archeryEmporium] = new System.Windows.Point(2, 8);

            Room libraryEntrance = AddRoom("Front Entry", "Front Entry of the Great Library");
            AddExit(briarMagenta, libraryEntrance, "library");
            AddExit(libraryEntrance, briarMagenta, "oak door");
            esgarothGraph.Rooms[libraryEntrance] = new System.Windows.Point(0, 4);

            Room referenceDesk = AddRoom("Reference Desk", "Reference Desk");
            referenceDesk.AddPermanentMobs(MobTypeEnum.RanierTheLibrarian);
            AddExit(libraryEntrance, referenceDesk, "blue door");
            AddExit(referenceDesk, libraryEntrance, "door");
            esgarothGraph.Rooms[referenceDesk] = new System.Windows.Point(0, 5);

            Room colloquiaRoom = AddRoom("Colloquia Room", "Colloquia Room");
            colloquiaRoom.AddPermanentMobs(MobTypeEnum.BlindCrone);
            e = AddExit(libraryEntrance, colloquiaRoom, "golden door");
            e.MustOpen = true;
            AddExit(colloquiaRoom, libraryEntrance, "door");
            esgarothGraph.Rooms[colloquiaRoom] = new System.Windows.Point(-1, 5);

            Room researchRoom = AddRoom("Research Room", "The Research Room");
            AddExit(libraryEntrance, researchRoom, "green door");
            AddExit(researchRoom, libraryEntrance, "door");
            esgarothGraph.Rooms[researchRoom] = new System.Windows.Point(-1, 4);

            Room languageStudies = AddRoom("Language Studies", "Language studies room");
            AddExit(researchRoom, languageStudies, "small door");
            AddExit(languageStudies, researchRoom, "door");
            esgarothGraph.Rooms[languageStudies] = new System.Windows.Point(-1, 3);
        }

        private void AddEsgarothMuseum(Room briarLane2, RoomGraph esgarothGraph)
        {
            RoomGraph esgarothMuseumGraph = _graphs[MapType.EsgarothMuseum];

            esgarothMuseumGraph.Rooms[briarLane2] = new System.Windows.Point(0, 0);

            Room giftShoppe = AddRoom("Gift Shoppe", "Museum Gift Shoppe");
            AddExit(briarLane2, giftShoppe, "museum");
            AddExit(giftShoppe, briarLane2, "door");
            esgarothGraph.Rooms[giftShoppe] = new System.Windows.Point(6, 5.5);
            esgarothMuseumGraph.Rooms[giftShoppe] = new System.Windows.Point(0, 1);
            AddMapBoundaryPoint(briarLane2, giftShoppe, MapType.Esgaroth, MapType.EsgarothMuseum);

            Room foyer = AddRoom("Foyer", "Adrilite Museum Entrance Foyer");
            AddBidirectionalExits(giftShoppe, foyer, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[foyer] = new System.Windows.Point(1, 1);

            Room childsExhibition = AddRoom("Child's Exhibition", "Adrilite Child's Exhibition");
            AddBidirectionalExits(childsExhibition, foyer, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[childsExhibition] = new System.Windows.Point(1, 0);

            Room oForestExhibit = AddRoom("Forest", "Adrilite Museum Forest Exhibit");
            AddBidirectionalExits(foyer, oForestExhibit, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[oForestExhibit] = new System.Windows.Point(2, 1);

            Room oDarkForestExhibit = AddRoom("Dark Forest", "Dark Forest Exhibit");
            AddBidirectionalExits(oForestExhibit, oDarkForestExhibit, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[oDarkForestExhibit] = new System.Windows.Point(3, 1);

            Room hallOfDoomNorth = AddRoom("Hall of Doom", "Adrilite Museum Hall of Doom");
            AddBidirectionalExits(foyer, hallOfDoomNorth, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[hallOfDoomNorth] = new System.Windows.Point(1, 2);

            Room hallOfDoomSouth = AddRoom("Hall of Doom", "Adrilite Museum Hall of Doom");
            AddBidirectionalExits(hallOfDoomNorth, hallOfDoomSouth, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[hallOfDoomSouth] = new System.Windows.Point(1, 3);

            Room dragonsAndBrimstone = AddRoom("Dragons/Brimstone", "Dragons and Brimstone Exhibit");
            AddExit(hallOfDoomNorth, dragonsAndBrimstone, "west");
            AddExit(dragonsAndBrimstone, hallOfDoomNorth, "northeast");
            AddExit(hallOfDoomSouth, dragonsAndBrimstone, "west");
            AddExit(dragonsAndBrimstone, hallOfDoomSouth, "southeast");
            esgarothMuseumGraph.Rooms[dragonsAndBrimstone] = new System.Windows.Point(0, 2.5);

            Room theatreOfDoom = AddRoom("Theatre of Doom", "Adrilite Museum Theatre of Doom");
            AddBidirectionalExits(hallOfDoomNorth, theatreOfDoom, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[theatreOfDoom] = new System.Windows.Point(2, 2);

            Room cavernsOfDoom = AddRoom("Caverns of Doom", "Adrilite Museum Caverns of Doom Exhibit");
            AddBidirectionalExits(hallOfDoomSouth, cavernsOfDoom, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[cavernsOfDoom] = new System.Windows.Point(2, 3);

            Room kTralDesertExhibit = AddRoom("K'Tral Desert", "Adrilite K'Tral Desert Exhibit");
            AddBidirectionalExits(hallOfDoomSouth, kTralDesertExhibit, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[kTralDesertExhibit] = new System.Windows.Point(1, 4);

            Room hallOfPrehistory = AddRoom("Hall of Prehistory", "Adrilite Museum Hall of Prehistory");
            AddBidirectionalSameNameExit(foyer, hallOfPrehistory, "stairs");
            esgarothMuseumGraph.Rooms[hallOfPrehistory] = new System.Windows.Point(4, 2);

            Room hallOfPrehistory2 = AddRoom("Hall of Prehistory", "Adrilite Museum Hall of Prehistory");
            AddBidirectionalExits(hallOfPrehistory, hallOfPrehistory2, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[hallOfPrehistory2] = new System.Windows.Point(4, 3);

            Room carnivoreExhibit = AddRoom("Carnivores", "Adrilite Museum Prehistoric Carnivore Exhibit");
            AddBidirectionalExits(carnivoreExhibit, hallOfPrehistory2, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[carnivoreExhibit] = new System.Windows.Point(3, 3);

            Room herbivoresExhibit = AddRoom("Herbivores", "Adrilite Museum Prehistoric Herbivore Exhibit");
            AddBidirectionalExits(hallOfPrehistory2, herbivoresExhibit, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[herbivoresExhibit] = new System.Windows.Point(5, 3);

            Room mammalsExhibit = AddRoom("Mammals", "Adrilite Museum Prehistoric Mammal Exhibit");
            AddBidirectionalExits(hallOfPrehistory2, mammalsExhibit, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[mammalsExhibit] = new System.Windows.Point(4, 4);
        }

        private void AddSpindrilsCastle(Room spindrilsCastleOutside)
        {
            RoomGraph spindrilsCastleLevel1Graph = _graphs[MapType.SpindrilsCastleLevel1];

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
            e = AddBidirectionalExitsWithOut(oGnimbelleGninbalArmory, oGniPawnShop, "passage");
            e.Hidden = true;
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
            RoomGraph oShantyTownGraph = _graphs[MapType.ShantyTown];

            oShantyTownGraph.Rooms[oMistyTrail8] = new System.Windows.Point(5, 0);

            Room oRuttedDirtRoad = AddRoom("Dirt Road", "Rutted Dirt Road");
            AddBidirectionalExits(oRuttedDirtRoad, oMistyTrail8, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oRuttedDirtRoad] = new System.Windows.Point(4, 0);
            imladrisToTharbadGraph.Rooms[oRuttedDirtRoad] = new System.Windows.Point(3, 7);
            AddMapBoundaryPoint(oMistyTrail8, oRuttedDirtRoad, MapType.ImladrisToTharbad, MapType.ShantyTown);

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

            Room oGarbagePit = AddRoom("Garbage Pit", "Garbage Pit");
            AddBidirectionalExitsWithOut(oShantyTownDump, oGarbagePit, "garbage");
            oShantyTownGraph.Rooms[oGarbagePit] = new System.Windows.Point(0, 3);

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

            Room oNaugrimsWineCellar = AddRoom("Wine Cellar", "Naugrim's Wine Cellar");
            AddBidirectionalExitsWithOut(oNaugrim, oNaugrimsWineCellar, "cellar");
            oShantyTownGraph.Rooms[oNaugrimsWineCellar] = new System.Windows.Point(0, 1);

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
        }

        private void AddIntangible(Room oBreeTownSquare, Room healingHand, Room nindamosVillageCenter, Room accursedGuildHall, Room crusaderGuildHall, Room thiefGuildHall)
        {
            RoomGraph intangibleGraph = _graphs[MapType.Intangible];
            RoomGraph nindamosGraph = _graphs[MapType.Nindamos];
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            intangibleGraph.Rooms[oBreeTownSquare] = new System.Windows.Point(0, 0);
            intangibleGraph.Rooms[healingHand] = new System.Windows.Point(1, 0);
            intangibleGraph.Rooms[nindamosVillageCenter] = new System.Windows.Point(2, 0);

            Room treeOfLife = AddRoom("Tree of Life", "The Tree of Life");
            treeOfLife.AddPermanentItems(ItemTypeEnum.BookOfKnowledge);
            treeOfLife.Intangible = true;
            AddExit(treeOfLife, oBreeTownSquare, "down");
            intangibleGraph.Rooms[treeOfLife] = new System.Windows.Point(0, 1);
            breeStreetsGraph.Rooms[treeOfLife] = new System.Windows.Point(5.5, 3.3);
            AddMapBoundaryPoint(treeOfLife, oBreeTownSquare, MapType.Intangible, MapType.BreeStreets);

            Room branch = AddRoom("Branch", Room.UNKNOWN_ROOM);
            AddExit(treeOfLife, branch, "branch");
            intangibleGraph.Rooms[branch] = new System.Windows.Point(-1, 1);

            Room oLimbo = AddRoom("Limbo", "Limbo");
            oLimbo.Intangible = true;
            Exit e = AddExit(oLimbo, treeOfLife, "green door");
            e.MustOpen = true;
            intangibleGraph.Rooms[oLimbo] = new System.Windows.Point(1, 2);

            Room oDarkTunnel = AddRoom("Dark Tunnel", "Dark Tunnel");
            oDarkTunnel.Intangible = true;
            e = AddExit(oLimbo, oDarkTunnel, "blue door");
            e.MustOpen = true;
            e.MinimumLevel = 4;
            AddExit(oDarkTunnel, healingHand, "light");
            intangibleGraph.Rooms[oDarkTunnel] = new System.Windows.Point(1, 1);
            imladrisGraph.Rooms[oDarkTunnel] = new System.Windows.Point(5, 4);
            AddMapBoundaryPoint(oDarkTunnel, healingHand, MapType.Intangible, MapType.Imladris);

            Room oFluffyCloudsAboveNindamos = AddRoom("Fluffy Clouds", "Fluffy clouds above Nindamos");
            oFluffyCloudsAboveNindamos.Intangible = false;
            e = AddExit(oLimbo, oFluffyCloudsAboveNindamos, "white door");
            e.MustOpen = true;
            AddExit(oFluffyCloudsAboveNindamos, nindamosVillageCenter, "green");
            intangibleGraph.Rooms[oFluffyCloudsAboveNindamos] = new System.Windows.Point(2, 1);
            nindamosGraph.Rooms[oFluffyCloudsAboveNindamos] = new System.Windows.Point(7, 3);
            AddMapBoundaryPoint(oFluffyCloudsAboveNindamos, nindamosVillageCenter, MapType.Intangible, MapType.Nindamos);

            Room oGuildedTunnel = AddRoom("Guilded Tunnel", "Guilded Tunnel");
            e = AddExit(treeOfLife, oGuildedTunnel, "guild halls");
            e.MustOpen = true;
            intangibleGraph.Rooms[oGuildedTunnel] = new System.Windows.Point(-1, 0);
            intangibleGraph.Rooms[accursedGuildHall] = new System.Windows.Point(-2, 0);
            breeStreetsGraph.Rooms[oGuildedTunnel] = new System.Windows.Point(6, -2);
            AddMapBoundaryPoint(oGuildedTunnel, accursedGuildHall, MapType.Intangible, MapType.BreeStreets);
            AddExit(oGuildedTunnel, accursedGuildHall, "accursed guild");

            Room oGuildedTunnel2 = AddRoom("Guilded Tunnel", "Guilded Tunnel");
            AddBidirectionalExits(oGuildedTunnel2, oGuildedTunnel, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildedTunnel2] = new System.Windows.Point(-1, -1);

            Room oGuildedTunnel3 = AddRoom("Guilded Tunnel", "Guilded Tunnel");
            AddBidirectionalExits(oGuildedTunnel3, oGuildedTunnel2, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildedTunnel3] = new System.Windows.Point(-1, -2);
            intangibleGraph.Rooms[crusaderGuildHall] = new System.Windows.Point(-2, -2);
            breeStreetsGraph.Rooms[oGuildedTunnel3] = new System.Windows.Point(8, -2);
            AddMapBoundaryPoint(oGuildedTunnel3, crusaderGuildHall, MapType.Intangible, MapType.BreeStreets);
            AddExit(oGuildedTunnel3, crusaderGuildHall, "crusader guild");

            Room oGuildStreet1 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet1, oGuildedTunnel3, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet1] = new System.Windows.Point(-1, -3);

            Room oGuildStreet2 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet2, oGuildStreet1, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet2] = new System.Windows.Point(-1, -4);

            Room oGuildStreet3 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet3, oGuildStreet2, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet3] = new System.Windows.Point(-1, -5);

            Room oGuildStreet4 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet4, oGuildStreet3, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet4] = new System.Windows.Point(-1, -6);
            intangibleGraph.Rooms[thiefGuildHall] = new System.Windows.Point(-2, -6);
            breeStreetsGraph.Rooms[oGuildStreet4] = new System.Windows.Point(9, -2);
            AddMapBoundaryPoint(oGuildStreet4, thiefGuildHall, MapType.Intangible, MapType.BreeStreets);
            AddExit(oGuildStreet4, thiefGuildHall, "thieves guild");

            Room oNorthGuildStreet = AddRoom("North Guild Street", "North Guild Street");
            AddBidirectionalExits(oNorthGuildStreet, oGuildStreet4, BidirectionalExitType.NorthSouth);
            AddExit(oNorthGuildStreet, oBreeTownSquare, "out");
            intangibleGraph.Rooms[oNorthGuildStreet] = new System.Windows.Point(-1, -7);
        }

        private void AddNindamos(out Room oArmenelosGatesOutside, out Room oSouthernJunction, out Room oPathThroughTheValleyHiddenPath, out Room nindamosDocks, out RoomGraph nindamosGraph, out Room nindamosVillageCenter)
        {
            nindamosGraph = _graphs[MapType.Nindamos];

            nindamosVillageCenter = AddRoom("Village Center", "Nindamos Village Center");
            nindamosVillageCenter.AddPermanentMobs(MobTypeEnum.MaxTheVegetableVendor);
            nindamosVillageCenter.AddPermanentItems(ItemTypeEnum.BoxOfStrawberries, ItemTypeEnum.BundleOfWheat, ItemTypeEnum.SackOfPotatoes);
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
            e = AddBidirectionalExitsWithOut(oSandstoneSouth2, oKKsIronWorksKosta, "path");
            e.RequiresDay = true;
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
            e = AddBidirectionalExitsWithOut(oElysia2, oHestasMarket, "market");
            e.RequiresDay = true;
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
            e = AddBidirectionalExitsWithOut(oGranitePath2, oAlasse, "south");
            e.RequiresDay = true;
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

            Room oPathToArmenelos1 = AddRoom("Valley Path", "Path Through the Valley");
            AddBidirectionalExits(oPathToArmenelos1, oSouthernJunction, BidirectionalExitType.SouthwestNortheast);
            nindamosGraph.Rooms[oPathToArmenelos1] = new System.Windows.Point(1, 0);

            oPathThroughTheValleyHiddenPath = AddRoom("Valley Path", "Path Through the Valley");
            AddBidirectionalExits(oPathThroughTheValleyHiddenPath, oPathToArmenelos1, BidirectionalExitType.SouthwestNortheast);
            nindamosGraph.Rooms[oPathThroughTheValleyHiddenPath] = new System.Windows.Point(2, -1);

            oArmenelosGatesOutside = AddRoom("Gate Outside", "Gates of Armenelos");
            oArmenelosGatesOutside.AddPermanentMobs(MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oArmenelosGatesOutside, oPathThroughTheValleyHiddenPath, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oArmenelosGatesOutside] = new System.Windows.Point(2, -2);
        }

        private void AddArmenelos(Room oArmenelosGatesOutside)
        {
            RoomGraph armenelosGraph = _graphs[MapType.Armenelos];
            RoomGraph nindamosGraph = _graphs[MapType.Nindamos];

            Room oAdrahilHirgon = AddRoom("Adrahil/Hirgon", "Hirgon Way/ Adrahil Road");
            armenelosGraph.Rooms[oAdrahilHirgon] = new System.Windows.Point(0, 0);

            Room oAdrahil1 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahilHirgon, oAdrahil1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil1] = new System.Windows.Point(1, 0);

            Room oAdrahilRivel = AddRoom("Adrahil/Rivel", "Adrahil Road/Rivel Way");
            AddBidirectionalExits(oAdrahil1, oAdrahilHirgon, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilRivel] = new System.Windows.Point(2, 0);

            Room oAdrahil2 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahilRivel, oAdrahil2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil2] = new System.Windows.Point(3, 0);

            Room oAdrahil3 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahil2, oAdrahil3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil3] = new System.Windows.Point(4, 0);

            Room oAdrahil4 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahil3, oAdrahil4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil4] = new System.Windows.Point(5, 0);

            Room oCityDump = AddRoom("City Dump", "Armenelos City Dump");
            AddBidirectionalExitsWithOut(oAdrahil4, oCityDump, "gate");
            armenelosGraph.Rooms[oCityDump] = new System.Windows.Point(5, 1);

            Room oDori = AddRoom("Dori", "Dori's Dump Shack");
            oDori.AddPermanentMobs(MobTypeEnum.Dori);
            AddBidirectionalExitsWithOut(oCityDump, oDori, "dump");
            armenelosGraph.Rooms[oDori] = new System.Windows.Point(4, 1);

            Room oAdrahilFolca = AddRoom("Adrahil/Folca", "Adrahil Road/Folca Street");
            AddBidirectionalExits(oAdrahil4, oAdrahilFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilFolca] = new System.Windows.Point(6, 0);

            Room oAdrahil5 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahilFolca, oAdrahil5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil5] = new System.Windows.Point(7, 0);

            Room oAdrahilWindfola = AddRoom("Adrahil/Windfola", "Adrahil Road/Windfola Avenue");
            AddBidirectionalExits(oAdrahil5, oAdrahilWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilWindfola] = new System.Windows.Point(8, 0);

            Room oHirgon1 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oAdrahilHirgon, oHirgon1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon1] = new System.Windows.Point(0, 1);

            Room oDoctorFaramir = AddRoom("Dr Faramir", "Dr. Faramir's Medical Supplies");
            oDoctorFaramir.AddPermanentMobs(MobTypeEnum.DrFaramir);
            AddBidirectionalExitsWithOut(oHirgon1, oDoctorFaramir, "door");
            armenelosGraph.Rooms[oDoctorFaramir] = new System.Windows.Point(1, 1);

            Room oRivel1 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oAdrahilRivel, oRivel1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel1] = new System.Windows.Point(2, 1);

            Room oFolca1 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oAdrahilFolca, oFolca1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca1] = new System.Windows.Point(6, 1);

            Room oWindfola1 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oAdrahilWindfola, oWindfola1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola1] = new System.Windows.Point(8, 1);

            Room oDorlasHirgon = AddRoom("Dorlas/Hirgon", "Hirgon Way/Dorlas Street");
            AddBidirectionalExits(oHirgon1, oDorlasHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oDorlasHirgon] = new System.Windows.Point(0, 2);

            Room oDorlas1 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlasHirgon, oDorlas1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas1] = new System.Windows.Point(1, 2);

            Room oDorlasRivel = AddRoom("Dorlas/Rivel", "Dorlas Street/Rivel Way");
            AddBidirectionalExits(oRivel1, oDorlasRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas1, oDorlasRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasRivel] = new System.Windows.Point(2, 2);

            Room oDorlas2 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlasRivel, oDorlas2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas2] = new System.Windows.Point(3, 2);

            Room oDorlas3 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlas2, oDorlas3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas3] = new System.Windows.Point(4, 2);

            Room oDorlas4 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlas3, oDorlas4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas4] = new System.Windows.Point(5, 2);

            Room oDorlasFolca = AddRoom("Dorlas/Folca", "Dorlas Street/Folca Avenue");
            AddBidirectionalExits(oFolca1, oDorlasFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas4, oDorlasFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasFolca] = new System.Windows.Point(6, 2);

            Room oDorlas5 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlasFolca, oDorlas5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas5] = new System.Windows.Point(7, 2);

            Room oTamar = AddRoom("Tamar", "Tamar of Armenelos");
            oTamar.AddPermanentMobs(MobTypeEnum.Tamar);
            AddBidirectionalExits(oDorlas5, oTamar, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oTamar] = new System.Windows.Point(7, 2.5);

            Room oDorlasWindfola = AddRoom("Dorlas/Windfola", "Windfola Avenue/Dorlas Street");
            AddBidirectionalExits(oWindfola1, oDorlasWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas5, oDorlasWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasWindfola] = new System.Windows.Point(8, 2);

            Room oHirgon2 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oDorlasHirgon, oHirgon2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon2] = new System.Windows.Point(0, 3);

            Room oRivel2 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oDorlasRivel, oRivel2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel2] = new System.Windows.Point(2, 3);

            Room oFolca2 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oDorlasFolca, oFolca2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca2] = new System.Windows.Point(6, 3);

            Room oWindfola2 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oDorlasWindfola, oWindfola2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola2] = new System.Windows.Point(8, 3);

            Room oAzgara = AddRoom("Azgara", "Azgara's Metalworking");
            oAzgara.AddPermanentMobs(MobTypeEnum.Azgara);
            AddBidirectionalExits(oAzgara, oWindfola2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAzgara] = new System.Windows.Point(7, 3);

            Room oOnlyArmor = AddRoom("Kali", "Only Armor");
            oOnlyArmor.AddPermanentMobs(MobTypeEnum.Kali);
            AddBidirectionalExitsWithOut(oAzgara, oOnlyArmor, "door");
            armenelosGraph.Rooms[oOnlyArmor] = new System.Windows.Point(6.5, 3.5);

            Room oSpecialtyShoppe = AddRoom("Specialty", "Azgara's Specialty Shoppe");
            AddBidirectionalExitsWithOut(oAzgara, oSpecialtyShoppe, "curtain");
            armenelosGraph.Rooms[oSpecialtyShoppe] = new System.Windows.Point(7.5, 3.5);

            Room oThalosHirgon = AddRoom("Hirgon/Thalos", "Hirgon Way/West Thalos Road");
            AddBidirectionalExits(oHirgon2, oThalosHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oThalosHirgon] = new System.Windows.Point(0, 4);

            Room oThalos1 = AddRoom("Thalos", "West Thalos Road");
            AddBidirectionalExits(oThalosHirgon, oThalos1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos1] = new System.Windows.Point(1, 4);

            Room oThalosRivel = AddRoom("Thalos/Rivel", "West Thalos Road/Rivel Way");
            AddBidirectionalExits(oRivel2, oThalosRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos1, oThalosRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosRivel] = new System.Windows.Point(2, 4);

            Room oThalos2 = AddRoom("Thalos", "West Thalos Road");
            AddBidirectionalExits(oThalosRivel, oThalos2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos2] = new System.Windows.Point(3, 4);

            Room oThalos3 = AddRoom("Thalos", "Thalos Road");
            AddBidirectionalExits(oThalos2, oThalos3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos3] = new System.Windows.Point(4, 4);

            Room oThalos4 = AddRoom("Thalos", "East Thalos Road");
            AddBidirectionalExits(oThalos3, oThalos4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos4] = new System.Windows.Point(5, 4);

            Room oThalosFolca = AddRoom("Thalos/Folca", "East Thalos Road/Folca Avenue");
            AddBidirectionalExits(oFolca2, oThalosFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos4, oThalosFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosFolca] = new System.Windows.Point(6, 4);

            Room oThalos5 = AddRoom("Thalos", "East Thalos Road");
            AddBidirectionalExits(oThalosFolca, oThalos5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos5] = new System.Windows.Point(7, 4);

            Room oThalosWindfola = AddRoom("Thalos/Windfola", "Windfola Avenue/ East Thalos Road");
            AddBidirectionalExits(oWindfola2, oThalosWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos5, oThalosWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosWindfola] = new System.Windows.Point(8, 4);

            Room oHirgon3 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oThalosHirgon, oHirgon3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon3] = new System.Windows.Point(0, 5);

            Room oRivel3 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oThalosRivel, oRivel3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel3] = new System.Windows.Point(2, 5);
            //CSRTODO: south (blocked)

            Room oFolca3 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oThalosFolca, oFolca3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca3] = new System.Windows.Point(6, 5);
            //CSRTODO: south (blocked)

            Room oWindfola3 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oThalosWindfola, oWindfola3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola3] = new System.Windows.Point(8, 5);

            Room oEllessarHirgon = AddRoom("Ellessar/Hirgon", "Hirgon Way/Ellessar Street");
            AddBidirectionalExits(oHirgon3, oEllessarHirgon, BidirectionalExitType.NorthSouth);
            //CSRTODO: east (blocked)
            armenelosGraph.Rooms[oEllessarHirgon] = new System.Windows.Point(0, 6);

            Room oEllessarWindfola = AddRoom("Ellessar/Windfola", "Windfola Avenue/Ellessar Street");
            AddBidirectionalExits(oWindfola3, oEllessarWindfola, BidirectionalExitType.NorthSouth);
            //CSRTODO: west (blocked)
            armenelosGraph.Rooms[oEllessarWindfola] = new System.Windows.Point(8, 6);

            Room oHirgon4 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oEllessarHirgon, oHirgon4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon4] = new System.Windows.Point(0, 7);

            Room oOutdoorMarket = AddRoom("OutdoorMarket", "Outdoor Market");
            AddBidirectionalExits(oHirgon4, oOutdoorMarket, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOutdoorMarket] = new System.Windows.Point(1, 7);

            Room oRivel4 = AddRoom("Rivel", "Rivel Way");
            armenelosGraph.Rooms[oRivel4] = new System.Windows.Point(2, 7);
            //CSRTODO: north (blocked)

            Room oFolca4 = AddRoom("Folca", "Folca Avenue");
            armenelosGraph.Rooms[oFolca4] = new System.Windows.Point(6, 7);
            //CSRTODO: north (blocked)

            Room oWindfola4 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oEllessarWindfola, oWindfola4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola4] = new System.Windows.Point(8, 7);

            Room oOrithilHirgon = AddRoom("Orithil/Hirgon", "Hirgon Way/Orithil Drive");
            AddBidirectionalExits(oHirgon4, oOrithilHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oOrithilHirgon] = new System.Windows.Point(0, 8);

            Room oOrithil1 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOutdoorMarket, oOrithil1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithilHirgon, oOrithil1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil1] = new System.Windows.Point(1, 8);

            Room oOrithilRivel = AddRoom("Orithil/Rivel", "Orithil Drive/Rivel Way");
            AddBidirectionalExits(oRivel4, oOrithilRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil1, oOrithilRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilRivel] = new System.Windows.Point(2, 8);

            Room oOrithil2 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithilRivel, oOrithil2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil2] = new System.Windows.Point(3, 8);

            Room oOrithil3 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithil2, oOrithil3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil3] = new System.Windows.Point(4, 8);

            Room oOrithil4 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithil3, oOrithil4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil4] = new System.Windows.Point(5, 8);

            Room oYurahtamJewelers = AddRoom("Yurahtam Jewlers", "Yurahtam Jewlers");
            AddBidirectionalExitsWithOut(oOrithil4, oYurahtamJewelers, "south");
            armenelosGraph.Rooms[oYurahtamJewelers] = new System.Windows.Point(5, 8.5);

            Room oOrithilFolca = AddRoom("Orithil/Folca", "Orithil Drive/Folca Street");
            AddBidirectionalExits(oFolca4, oOrithilFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil4, oOrithilFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilFolca] = new System.Windows.Point(6, 8);

            Room oOrithil5 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithilFolca, oOrithil5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil5] = new System.Windows.Point(7, 8);
            //CSRTODO: archway (blocked)

            Room oOrithilWindfola = AddRoom("Orithil/Windfola", "Windfola Avenue/Orithil Drive");
            AddBidirectionalExits(oWindfola4, oOrithilWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil5, oOrithilWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilWindfola] = new System.Windows.Point(8, 8);

            Room oHirgon5 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oOrithilHirgon, oHirgon5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon5] = new System.Windows.Point(0, 9);

            Room oStairwayLanding = AddRoom("Stairway Landing", "Stairway Landing");
            AddExit(oHirgon5, oStairwayLanding, "stairway");
            AddExit(oStairwayLanding, oHirgon5, "down");
            armenelosGraph.Rooms[oStairwayLanding] = new System.Windows.Point(1, 9);

            Room oAmme = AddRoom("Amme", "Commoner's Home");
            oAmme.AddPermanentMobs(MobTypeEnum.Amme);
            AddBidirectionalExitsWithOut(oStairwayLanding, oAmme, "doorway");
            armenelosGraph.Rooms[oAmme] = new System.Windows.Point(1, 8.5);

            Room oRivel5 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oOrithilRivel, oRivel5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel5] = new System.Windows.Point(2, 9);

            Room oFolca5 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oOrithilFolca, oFolca5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca5] = new System.Windows.Point(6, 9);

            Room oWindfola5 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oOrithilWindfola, oWindfola5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola5] = new System.Windows.Point(8, 9);

            Room oBalanHirgon = AddRoom("Balan/Hirgon", "Hirgon Way/Balan Avenue");
            AddBidirectionalExits(oHirgon5, oBalanHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oBalanHirgon] = new System.Windows.Point(0, 10);

            Room oBalan1 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalanHirgon, oBalan1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan1] = new System.Windows.Point(1, 10);

            Room oBalanRivel = AddRoom("Balan/Rivel", "Balan Avenue/Rivel Way");
            AddBidirectionalExits(oRivel5, oBalanRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan1, oBalanRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanRivel] = new System.Windows.Point(2, 10);

            Room oBalan2 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalanRivel, oBalan2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan2] = new System.Windows.Point(3, 10);

            Room oBalan3 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalan2, oBalan3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan3] = new System.Windows.Point(4, 10);

            Room oBalan4 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalan3, oBalan4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan4] = new System.Windows.Point(5, 10);

            Room oMerchantsMarket1 = AddRoom("Merchant Market", "Merchant's Market");
            AddBidirectionalExits(oMerchantsMarket1, oBalan2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket1] = new System.Windows.Point(3, 9.5);

            Room oCultOfAzogHQ = AddRoom("Cult of Azoq HQ", "Cult of Azog Headquarters");
            oCultOfAzogHQ.AddPermanentMobs(MobTypeEnum.Voteli);
            Exit e = AddBidirectionalExitsWithOut(oMerchantsMarket1, oCultOfAzogHQ, "tent");
            e.Hidden = true;
            armenelosGraph.Rooms[oCultOfAzogHQ] = new System.Windows.Point(3, 9);

            Room oMerchantsMarket2 = AddRoom("Merchant Market", "Merchant's Market");
            AddBidirectionalExits(oMerchantsMarket1, oMerchantsMarket2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMerchantsMarket2, oBalan3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket2] = new System.Windows.Point(4, 9.5);

            Room oMerchantsMarket3 = AddRoom("Merchant Market", "Merchant's Market");
            AddBidirectionalExits(oMerchantsMarket2, oMerchantsMarket3, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMerchantsMarket3, oBalan4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket3] = new System.Windows.Point(5, 9.5);

            Room oBalanFolca = AddRoom("Balan/Folca", "Folca Street/Balan Avenue");
            AddBidirectionalExits(oFolca5, oBalanFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan4, oBalanFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanFolca] = new System.Windows.Point(6, 10);

            Room oBalan5 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalanFolca, oBalan5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan5] = new System.Windows.Point(7, 10);

            Room oBalanWindfola = AddRoom("Balan/Windfola", "Windfola Avenue/Balan Avenue");
            AddBidirectionalExits(oWindfola5, oBalanWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan5, oBalanWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanWindfola] = new System.Windows.Point(8, 10);

            Room oHirgon6 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oBalanHirgon, oHirgon6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon6] = new System.Windows.Point(0, 11);

            Room oRivel6 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oBalanRivel, oRivel6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel6] = new System.Windows.Point(2, 11);

            Room oFolca6 = AddRoom("Folca", "Folca Street");
            AddBidirectionalExits(oBalanFolca, oFolca6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca6] = new System.Windows.Point(6, 11);

            Room oWindfola6 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oBalanWindfola, oWindfola6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola6] = new System.Windows.Point(8, 11);

            Room oGoldberryHirgon = AddRoom("Goldberry/Hirgon", "Goldberry Road/Hirgon Way");
            AddBidirectionalExits(oHirgon6, oGoldberryHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oGoldberryHirgon] = new System.Windows.Point(0, 12);

            Room oGoldberry1 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberryHirgon, oGoldberry1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry1] = new System.Windows.Point(1, 12);

            Room oImrahil = AddRoom("Imrahil", "Imrahil's Pub");
            oImrahil.AddPermanentMobs(MobTypeEnum.Imrahil);
            AddBidirectionalSameNameExit(oGoldberry1, oImrahil, "swinging");
            armenelosGraph.Rooms[oImrahil] = new System.Windows.Point(1, 11);

            Room oGoldberryRivel = AddRoom("Goldberry/Rivel", "Goldberry Road/Rivil Way");
            AddBidirectionalExits(oRivel6, oGoldberryRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry1, oGoldberryRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberryRivel] = new System.Windows.Point(2, 12);

            Room oGoldberry2 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberryRivel, oGoldberry2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry2] = new System.Windows.Point(3, 12);

            Room oGoldberry3 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberry2, oGoldberry3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry3] = new System.Windows.Point(4, 12);

            Room oHummley = AddRoom("Hummley", "Hummley's Shop o' Fun");
            oHummley.AddPermanentMobs(MobTypeEnum.Hummley);
            AddBidirectionalExitsWithOut(oGoldberry3, oHummley, "doorway");
            armenelosGraph.Rooms[oHummley] = new System.Windows.Point(4, 11);

            Room oGoldberry4 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberry3, oGoldberry4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry4] = new System.Windows.Point(5, 12);

            Room oGoldberryFolca = AddRoom("Goldberry/Folca", "Goldberry Road/Folca Street");
            AddBidirectionalExits(oFolca6, oGoldberryFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry4, oGoldberryFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberryFolca] = new System.Windows.Point(6, 12);

            Room oGoldberry5 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberryFolca, oGoldberry5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry5] = new System.Windows.Point(7, 12);

            Room oZain = AddRoom("Zain", "Tourist Information");
            oZain.AddPermanentMobs(MobTypeEnum.Zain);
            AddBidirectionalExitsWithOut(oGoldberry5, oZain, "north");
            armenelosGraph.Rooms[oZain] = new System.Windows.Point(7, 11);

            Room oGateInside = AddRoom("Gate Inside", "Entrance to Armenelos");
            AddBidirectionalExits(oWindfola6, oGateInside, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry5, oGateInside, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGateInside] = new System.Windows.Point(8, 12);
            nindamosGraph.Rooms[oGateInside] = new System.Windows.Point(2, -3);

            AddExit(oGateInside, oArmenelosGatesOutside, "gate");
            e = AddExit(oArmenelosGatesOutside, oGateInside, "gate");
            e.RequiresDay = true;
            armenelosGraph.Rooms[oArmenelosGatesOutside] = new System.Windows.Point(8, 13);
            AddMapBoundaryPoint(oArmenelosGatesOutside, oGateInside, MapType.Nindamos, MapType.Armenelos);
        }

        private void AddWestOfNindamosAndArmenelos(Room oSouthernJunction, Room oPathThroughTheValley, out Room oEldemondeEastGateOutside, RoomGraph nindamosGraph)
        {
            RoomGraph nindamosEldemondeGraph = _graphs[MapType.NindamosToEldemonde];
            RoomGraph deathValleyGraph = _graphs[MapType.DeathValley];

            Room r;
            Room previousRoom = oSouthernJunction;
            nindamosEldemondeGraph.Rooms[oSouthernJunction] = new System.Windows.Point(26, 18);
            for (int i = 0; i < 7; i++)
            {
                r = AddRoom("Laiquendi", "Laiquendi");
                AddBidirectionalExits(r, previousRoom, BidirectionalExitType.WestEast);
                nindamosEldemondeGraph.Rooms[r] = new System.Windows.Point(25 - i, 18);
                if (i == 0)
                {
                    System.Windows.Point pSJ = nindamosGraph.Rooms[oSouthernJunction];
                    nindamosGraph.Rooms[r] = new System.Windows.Point(pSJ.X - 1, pSJ.Y);
                    AddMapBoundaryPoint(oSouthernJunction, r, MapType.Nindamos, MapType.NindamosToEldemonde);
                }
                previousRoom = r;
            }
            Room hiddenPathRoom = null;
            for (int i = 0; i < 9; i++)
            {
                r = AddRoom("Liara", "Liara");
                AddExit(r, previousRoom, "south");
                if (i == 4)
                {
                    hiddenPathRoom = r;
                }
                AddExit(previousRoom, r, "north");
                nindamosEldemondeGraph.Rooms[r] = new System.Windows.Point(19, 17-i);
                previousRoom = r;
            }
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[r] = new System.Windows.Point(18, 8);
            previousRoom = r;
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[r] = new System.Windows.Point(17, 7);
            previousRoom = r;
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[r] = new System.Windows.Point(17, 6);
            previousRoom = r;
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[r] = new System.Windows.Point(17, 4);
            previousRoom = r;
            Room oLastLiara = r;

            Room oBaseOfMenelTarma = AddRoom("Base of Menel tarma", "Base of Menel tarma");
            oBaseOfMenelTarma.AddPermanentMobs(MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oBaseOfMenelTarma, previousRoom, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oBaseOfMenelTarma] = new System.Windows.Point(15, 4);
            AddMenelTarma(oBaseOfMenelTarma, nindamosEldemondeGraph);

            Room oHiddenPath1 = AddRoom("Streambed", "Streambed");
            AddBidirectionalExits(hiddenPathRoom, oHiddenPath1, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath1] = new System.Windows.Point(20, 13.5);
            Room oHiddenPath2 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath1, oHiddenPath2, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath2] = new System.Windows.Point(21, 14);
            Room oHiddenPath3 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath2, oHiddenPath3, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath3] = new System.Windows.Point(21, 14.5);
            Room oHiddenPath4 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath3, oHiddenPath4, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath4] = new System.Windows.Point(21, 15);
            Room oHiddenPath5 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath4, oHiddenPath5, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath5] = new System.Windows.Point(21, 15.5);
            Room oHiddenPath6 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath5, oHiddenPath6, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath6] = new System.Windows.Point(21, 16);
            Room oHiddenPath7 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath6, oHiddenPath7, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath7] = new System.Windows.Point(21, 16.5);
            Room oHiddenPath8 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath7, oHiddenPath8, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath8] = new System.Windows.Point(21, 17);
            Room oHiddenPath9 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath8, oHiddenPath9, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath9] = new System.Windows.Point(22, 17.5);
            Room oHiddenPath10 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath9, oHiddenPath10, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath10] = new System.Windows.Point(23, 17.5);
            Room oHiddenPath11 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath10, oHiddenPath11, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath11] = new System.Windows.Point(24, 17.5);
            Room oHiddenPath12 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath12, oHiddenPath11, BidirectionalExitType.SouthwestNortheast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath12] = new System.Windows.Point(25, 17);
            Room oHiddenPath13 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath12, oHiddenPath13, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath13] = new System.Windows.Point(26, 17);
            Room oHiddenPath14 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath13, oHiddenPath14, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath14] = new System.Windows.Point(27, 17);
            Room oHiddenPath15 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath14, oHiddenPath15, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath15] = new System.Windows.Point(28, 17);
            AddBidirectionalExits(oHiddenPath15, oPathThroughTheValley, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oPathThroughTheValley] = new System.Windows.Point(29, 17.5);
            System.Windows.Point p = nindamosGraph.Rooms[oPathThroughTheValley];
            nindamosGraph.Rooms[oHiddenPath15] = new System.Windows.Point(p.X - 1, p.Y - 1);
            AddMapBoundaryPoint(oPathThroughTheValley, oHiddenPath15, MapType.Nindamos, MapType.NindamosToEldemonde);

            Room oGrasslands1 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oSouthernJunction, oGrasslands1, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands1] = new System.Windows.Point(25, 19);

            Room oGrasslands2 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands2, oGrasslands1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands2] = new System.Windows.Point(24, 19);

            Room oHostaEncampment = AddRoom("Hosta Encampment", "Hosta Encampment");
            oHostaEncampment.AddPermanentMobs(MobTypeEnum.HostaWarrior);
            AddBidirectionalExits(oHostaEncampment, oGrasslands2, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oHostaEncampment] = new System.Windows.Point(23, 18.5);

            Room oGrasslands3 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands2, oGrasslands3, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands3] = new System.Windows.Point(23, 20);

            Room oGrasslands4 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands4, oGrasslands3, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands4] = new System.Windows.Point(22, 20);

            Room oGrasslands5 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands5, oGrasslands4, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands5] = new System.Windows.Point(22, 19);

            Room oGrasslands6 = AddRoom("Grasslands", "Grasslands of Mittalamar");
            AddBidirectionalExits(oGrasslands5, oGrasslands6, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands6] = new System.Windows.Point(23, 19);

            Room oGrasslands7 = AddRoom("Grasslands", "Grasslands of Mittalamar");
            AddBidirectionalExits(oGrasslands6, oGrasslands7, BidirectionalExitType.NorthSouth);
            AddExit(oGrasslands7, oGrasslands3, "south");
            nindamosEldemondeGraph.Rooms[oGrasslands7] = new System.Windows.Point(23, 19.5);

            Room oGrasslands8 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands4, oGrasslands8, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands8] = new System.Windows.Point(21, 21);

            Room oGrasslands9 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands9, oGrasslands4, BidirectionalExitType.WestEast);
            AddExit(oGrasslands8, oGrasslands9, "north");
            AddExit(oGrasslands9, oGrasslands8, "north");
            nindamosEldemondeGraph.Rooms[oGrasslands9] = new System.Windows.Point(21, 20);

            Room oGrasslands10 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands4, oGrasslands10, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands10] = new System.Windows.Point(22, 21);

            Room oGrasslands11 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands10, oGrasslands11, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands11] = new System.Windows.Point(21, 22);

            Room oGrasslands12 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands12, oGrasslands11, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands12] = new System.Windows.Point(20, 22);

            Room oGrasslands13 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands13, oGrasslands12, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands13] = new System.Windows.Point(20, 21);

            Room oGrasslands14 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands14, oGrasslands13, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands14] = new System.Windows.Point(20, 20);

            Room oGrasslands15 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands15, oGrasslands14, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oGrasslands15, oGrasslands5, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oGrasslands15] = new System.Windows.Point(21, 18.5);

            Room oGrasslands16 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands16, oGrasslands13, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oGrasslands16] = new System.Windows.Point(19, 20);
            deathValleyGraph.Rooms[oGrasslands16] = new System.Windows.Point(6, 9.5);

            Room oDeathValleyEntrance = AddRoom("Death Valley Entrance", "Entrance to the Valley of the Dead");
            AddBidirectionalExits(oGrasslands16, oDeathValleyEntrance, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oDeathValleyEntrance] = new System.Windows.Point(19, 21);
            AddMapBoundaryPoint(oGrasslands16, oDeathValleyEntrance, MapType.NindamosToEldemonde, MapType.DeathValley);

            Room oGrassCoveredField1 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oLastLiara, oGrassCoveredField1, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField1] = new System.Windows.Point(16, 6);

            Room oGrassCoveredField2 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField2, oGrassCoveredField1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField2] = new System.Windows.Point(15, 6);

            Room oGrassCoveredField3 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField3, oGrassCoveredField2, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField3] = new System.Windows.Point(14, 6);

            Room oGrassCoveredField4 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField4, oGrassCoveredField3, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField4] = new System.Windows.Point(13, 5);

            Room oRiverPath1 = AddRoom("River Path", "River Path");
            AddBidirectionalExits(oRiverPath1, oGrassCoveredField4, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oRiverPath1] = new System.Windows.Point(12, 5);

            Room oRiverPath2 = AddRoom("River Path", "River Path");
            AddBidirectionalExits(oRiverPath2, oRiverPath1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oRiverPath2] = new System.Windows.Point(11, 5);

            Room oRiverBank = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank, oRiverPath2, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oRiverBank] = new System.Windows.Point(10, 4);

            Room oGrassCoveredField5 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField3, oGrassCoveredField5, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField5] = new System.Windows.Point(13, 7);

            Room oGrassCoveredField6 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField6, oGrassCoveredField5, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField6] = new System.Windows.Point(12, 7);

            Room oEdgeOfNisimaldar = AddRoom("Nisimaldar Edge", "Edge of Nisimaldar");
            AddBidirectionalExits(oGrassCoveredField6, oEdgeOfNisimaldar, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oEdgeOfNisimaldar] = new System.Windows.Point(11, 8);

            Room oNisimaldar1 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar1, oEdgeOfNisimaldar, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar1] = new System.Windows.Point(10, 9);

            Room oNisimaldar2 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar2, oNisimaldar1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar2] = new System.Windows.Point(9, 9);

            Room oNisimaldar3 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar3, oNisimaldar2, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oNisimaldar3] = new System.Windows.Point(8, 8);

            Room oNisimaldar4 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar4, oNisimaldar3, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar4] = new System.Windows.Point(7, 8);

            Room oNisimaldar5 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar4, oNisimaldar5, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oNisimaldar5] = new System.Windows.Point(6, 9);

            Room oNisimaldar6 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar6, oNisimaldar5, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar6] = new System.Windows.Point(5, 9);

            Room oNisimaldar7 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar6, oNisimaldar7, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oNisimaldar7] = new System.Windows.Point(4, 10);

            Room oNisimaldar8 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar8, oNisimaldar7, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar8] = new System.Windows.Point(3, 10);

            Room oNisimaldar9 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar9, oNisimaldar8, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oNisimaldar9] = new System.Windows.Point(2, 9);

            Room oNisimaldar10 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar10, oNisimaldar9, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar10] = new System.Windows.Point(1, 9);

            oEldemondeEastGateOutside = AddRoom("East Gate Outside", "East Gate of Eldalonde");
            AddBidirectionalExits(oEldemondeEastGateOutside, oNisimaldar10, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oEldemondeEastGateOutside] = new System.Windows.Point(0, 8);

            AddDeathValley(oDeathValleyEntrance);
        }

        private void AddMenelTarma(Room baseOfMenelTarma, RoomGraph eldemondeToNindamosGraph)
        {
            Room oRoad1 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(baseOfMenelTarma, oRoad1, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad1] = new System.Windows.Point(15, 4.5);

            Room oRoad2 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad2, oRoad1, BidirectionalExitType.WestEast);
            eldemondeToNindamosGraph.Rooms[oRoad2] = new System.Windows.Point(14, 4.5);

            Room oRoad3 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad2, oRoad3, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad3] = new System.Windows.Point(14, 5);

            Room oRoad4 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad3, oRoad4, BidirectionalExitType.WestEast);
            eldemondeToNindamosGraph.Rooms[oRoad4] = new System.Windows.Point(15, 5);

            Room oRoad5 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad4, oRoad5, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad5] = new System.Windows.Point(15, 5.5);

            Room oRoad6 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad5, oRoad6, BidirectionalExitType.WestEast);
            eldemondeToNindamosGraph.Rooms[oRoad6] = new System.Windows.Point(16, 5.5);

            Room oRoad7 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad7, oRoad6, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad7] = new System.Windows.Point(16, 5.1);

            Room oPath1 = AddRoom("Path", "Path to the summit of Menel tarma");
            AddBidirectionalExits(oPath1, oRoad7, BidirectionalExitType.UpDown);
            eldemondeToNindamosGraph.Rooms[oPath1] = new System.Windows.Point(16, 4.7);

            Room oSummit = AddRoom("Summit", "Summit of Menel Tarma");
            oSummit.AddPermanentMobs(MobTypeEnum.GoldenEagle, MobTypeEnum.GoldenEagle, MobTypeEnum.GoldenEagle);
            AddExit(oPath1, oSummit, "up");
            AddExit(oSummit, oPath1, "slope");
            eldemondeToNindamosGraph.Rooms[oSummit] = new System.Windows.Point(16, 4.3);
        }

        private void AddDeathValley(Room oDeathValleyEntrance)
        {
            RoomGraph deathValleyGraph = _graphs[MapType.DeathValley];

            deathValleyGraph.Rooms[oDeathValleyEntrance] = new System.Windows.Point(6, 10);

            Room oDeathValleyWest1 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest1, oDeathValleyEntrance, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyWest1] = new System.Windows.Point(5, 10);

            Room oDeathValleyWest2 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest2, oDeathValleyWest1, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyWest2] = new System.Windows.Point(5, 9);

            Room oAmlug = AddRoom("Amlug", "Tomb of Amlug");
            oAmlug.AddPermanentMobs(MobTypeEnum.Amlug);
            AddBidirectionalExitsWithOut(oDeathValleyWest2, oAmlug, "tomb");
            deathValleyGraph.Rooms[oAmlug] = new System.Windows.Point(5, 8);

            Room oDeathValleyWest3 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest3, oDeathValleyWest2, BidirectionalExitType.SouthwestNortheast);
            deathValleyGraph.Rooms[oDeathValleyWest3] = new System.Windows.Point(6, 8);

            Room oDeathValleyWest4 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest4, oDeathValleyWest3, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyWest4] = new System.Windows.Point(6, 7);

            Room oKallo = AddRoom("Kallo", "Kallo's Final Resting Place");
            oKallo.AddPermanentMobs(MobTypeEnum.Kallo);
            AddBidirectionalExitsWithOut(oDeathValleyWest4, oKallo, "tomb");
            deathValleyGraph.Rooms[oKallo] = new System.Windows.Point(5, 7);

            Room oDeathValleyWest5 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest5, oDeathValleyWest4, BidirectionalExitType.SoutheastNorthwest);
            deathValleyGraph.Rooms[oDeathValleyWest5] = new System.Windows.Point(5, 6);

            Room oDeathValleyWest6 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest6, oDeathValleyWest5, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyWest6] = new System.Windows.Point(5, 5);

            Room oWizard = AddRoom("Wizard of the First Order", "Wizard's Resting Place");
            oWizard.AddPermanentMobs(MobTypeEnum.WizardOfTheFirstOrder);
            AddBidirectionalExitsWithOut(oDeathValleyWest6, oWizard, "vault");
            deathValleyGraph.Rooms[oWizard] = new System.Windows.Point(6, 5);

            Room oDeathValleyWest7 = AddRoom("Death Valley", "Valley of the Dead.");
            AddBidirectionalExits(oDeathValleyWest7, oDeathValleyWest6, BidirectionalExitType.SouthwestNortheast);
            //CSRTODO: doorway
            deathValleyGraph.Rooms[oDeathValleyWest7] = new System.Windows.Point(6, 4);

            Room oDeathValleyEast1 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEntrance, oDeathValleyEast1, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyEast1] = new System.Windows.Point(7, 10);

            Room oDeathValleyEast2 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast2, oDeathValleyEast1, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyEast2] = new System.Windows.Point(7, 9.5);

            Room oDeathValleyEast3 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast3, oDeathValleyEast2, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyEast3] = new System.Windows.Point(7, 9);

            Room oTranquilParkKaivo = AddHealingRoom("Kaivo", "Tranquil Park", HealingRoom.DeathValley);
            oTranquilParkKaivo.AddPermanentMobs(MobTypeEnum.Kaivo);
            AddBidirectionalExits(oTranquilParkKaivo, oDeathValleyEast3, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oTranquilParkKaivo] = new System.Windows.Point(6, 9);

            Room oDeathValleyEast4 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast4, oDeathValleyEast3, BidirectionalExitType.SouthwestNortheast);
            deathValleyGraph.Rooms[oDeathValleyEast4] = new System.Windows.Point(8, 7);

            Room oDeathValleyEast5 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast4, oDeathValleyEast5, BidirectionalExitType.SoutheastNorthwest);
            deathValleyGraph.Rooms[oDeathValleyEast5] = new System.Windows.Point(9, 8);

            Room oDeathValleyEast6 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast6, oDeathValleyEast5, BidirectionalExitType.SouthwestNortheast);
            deathValleyGraph.Rooms[oDeathValleyEast6] = new System.Windows.Point(11, 7);

            Room oDeathValleyEast7 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast7, oDeathValleyEast6, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyEast7] = new System.Windows.Point(10, 7);

            Room oDeathValleyEast8 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast8, oDeathValleyEast7, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyEast8] = new System.Windows.Point(9, 7);

            Room oStorageRoom = AddRoom("Storage Room", "Storage Room");
            Exit e = AddBidirectionalExitsWithOut(oDeathValleyEast8, oStorageRoom, "rocky");
            e.Hidden = true;
            deathValleyGraph.Rooms[oStorageRoom] = new System.Windows.Point(9, 6);
        }

        private void AddEldemondeCity(Room oEldemondeEastGateOutside)
        {
            RoomGraph eldemondeGraph = _graphs[MapType.Eldemonde];
            RoomGraph nindamosToEldemondeGraph = _graphs[MapType.NindamosToEldemonde];

            eldemondeGraph.Rooms[oEldemondeEastGateOutside] = new System.Windows.Point(10, 7);

            Room oEldemondeEastGateInside = AddRoom("East Gate Inside", "Eldalondë East Gate");
            oEldemondeEastGateInside.AddPermanentMobs(MobTypeEnum.GateGuard, MobTypeEnum.GateGuard);
            AddBidirectionalSameNameExit(oEldemondeEastGateOutside, oEldemondeEastGateInside, "gate");
            eldemondeGraph.Rooms[oEldemondeEastGateInside] = new System.Windows.Point(9, 7);
            nindamosToEldemondeGraph.Rooms[oEldemondeEastGateInside] = new System.Windows.Point(-1, 8);
            AddMapBoundaryPoint(oEldemondeEastGateOutside, oEldemondeEastGateInside, MapType.NindamosToEldemonde, MapType.Eldemonde);

            Room oCebe1 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe1, oEldemondeEastGateInside, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCebe1] = new System.Windows.Point(9, 6);

            Room oDorie1 = AddRoom("Dorie", "Dorië Avenue");
            AddBidirectionalExits(oEldemondeEastGateInside, oDorie1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oDorie1] = new System.Windows.Point(9, 8);

            Room oCebe2 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe2, oCebe1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe2] = new System.Windows.Point(8, 6);

            Room oElros2 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros2, oEldemondeEastGateInside, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros2] = new System.Windows.Point(8, 7);

            Room oDorie2 = AddRoom("Dorie", "Dorië Avenue - The Guardstation");
            AddBidirectionalExits(oDorie2, oDorie1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie2] = new System.Windows.Point(8, 8);

            Room oGuardHall = AddRoom("Guard Hall", "Guard Station Main Hall");
            AddBidirectionalExits(oGuardHall, oDorie2, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oGuardHall] = new System.Windows.Point(8, 7.7);

            Room oBarracks = AddRoom("Barracks", "Barracks");
            AddBidirectionalExits(oBarracks, oGuardHall, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oBarracks] = new System.Windows.Point(7.6, 7.4);

            Room oGuardHQ = AddRoom("Guard HQ", "Guard Station Headquarters");
            AddBidirectionalExits(oGuardHall, oGuardHQ, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oGuardHQ] = new System.Windows.Point(8.4, 7.4);

            Room oCebe3 = AddRoom("Cebe", "Tower of Morgatha");
            AddBidirectionalExits(oCebe3, oCebe2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe3] = new System.Windows.Point(7, 6);

            Room oTower = AddRoom("Tower", "Base of Morgathas Tower");
            AddBidirectionalSameNameExit(oCebe3, oTower, "door");
            eldemondeGraph.Rooms[oTower] = new System.Windows.Point(7, 5.5);

            Room oElementsChamber = AddRoom("Elements Chamber", "Chamber of Elements");
            AddBidirectionalExits(oElementsChamber, oTower, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oElementsChamber] = new System.Windows.Point(8, 5.5);

            Room oGolemsChamber = AddRoom("Golems Chamber", "Chamber of Golems");
            AddBidirectionalExits(oGolemsChamber, oElementsChamber, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oGolemsChamber] = new System.Windows.Point(9, 5.5);

            Room oMorgatha = AddRoom("Morgatha", "Morgatha's Chamber");
            oMorgatha.AddPermanentMobs(MobTypeEnum.MorgathaTheEnchantress);
            AddBidirectionalExits(oMorgatha, oGolemsChamber, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oMorgatha] = new System.Windows.Point(10, 5.5);

            Room oElros3 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros3, oElros2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros3] = new System.Windows.Point(7, 7);

            Room oDorie3 = AddRoom("Dorie", "Dorië Avenue");
            AddBidirectionalExits(oDorie3, oDorie2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie3] = new System.Windows.Point(7, 8);

            Room oPostOffice = AddRoom("Post Office", "The Eldalonde Post Office");
            AddBidirectionalExits(oPostOffice, oDorie3, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oPostOffice] = new System.Windows.Point(7, 7.5);

            Room oIothCandol = AddRoom("Ioth/Candol", "Ioth Road / Candol Street");
            eldemondeGraph.Rooms[oIothCandol] = new System.Windows.Point(6, 4);

            Room oCandol1 = AddRoom("Candol", "Candol Street");
            AddBidirectionalExits(oIothCandol, oCandol1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCandol1] = new System.Windows.Point(6, 5);

            Room oUniversityHall = AddRoom("University Hall", "Main Hall");
            AddBidirectionalExits(oCandol1, oUniversityHall, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oUniversityHall] = new System.Windows.Point(7, 4);

            Room oUniversityHallSouth = AddRoom("University Hall", "South Hall");
            AddBidirectionalExits(oUniversityHall, oUniversityHallSouth, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oUniversityHallSouth] = new System.Windows.Point(7, 4.5);

            Room oUniversityHallSE = AddRoom("University Hall", "South Hall");
            AddBidirectionalExits(oUniversityHallSouth, oUniversityHallSE, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oUniversityHallSE] = new System.Windows.Point(8, 4.5);

            Room oAlchemy = AddRoom("Alchemy", "Alchemy");
            AddBidirectionalSameNameExit(oUniversityHallSE, oAlchemy, "door");
            eldemondeGraph.Rooms[oAlchemy] = new System.Windows.Point(8, 5);

            Room oAurelius = AddRoom("Aurelius", "Mysticism");
            oAurelius.AddPermanentMobs(MobTypeEnum.AureliusTheScholar);
            AddBidirectionalSameNameExit(oUniversityHallSouth, oAurelius, "door");
            eldemondeGraph.Rooms[oAurelius] = new System.Windows.Point(7, 5);

            Room oUniversityHallNorth = AddRoom("University Hall", "North Hall");
            AddBidirectionalExits(oUniversityHallNorth, oUniversityHall, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oUniversityHallNorth] = new System.Windows.Point(7, 4);

            Room oMathemathics = AddRoom("Mathemathics", "Mathemathics");
            AddBidirectionalExitsWithOut(oUniversityHallNorth, oMathemathics, "door");
            eldemondeGraph.Rooms[oMathemathics] = new System.Windows.Point(8, 4);

            Room oCebeCandol = AddRoom("Cebe/Candol", "Cebe Avenue / Candol Street");
            AddBidirectionalExits(oCebeCandol, oCebe3, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCandol1, oCebeCandol, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCebeCandol] = new System.Windows.Point(6, 6);

            Room oElrosCandol = AddRoom("Elros/Candol", "Elros Boulevard / Candol Street");
            AddBidirectionalExits(oCebeCandol, oElrosCandol, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElrosCandol, oElros3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElrosCandol] = new System.Windows.Point(6, 7);

            Room oDorieCandol = AddRoom("Dorie/Candol", "Dorië Avenue / Candol Street");
            AddBidirectionalExits(oElrosCandol, oDorieCandol, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorieCandol, oDorie3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorieCandol] = new System.Windows.Point(6, 8);

            Room oIoth1 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth1, oIothCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth1] = new System.Windows.Point(5, 4);

            Room oCebe4 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe4, oCebeCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe4] = new System.Windows.Point(5, 6);

            Room oIsildur = AddRoom("Isildur", "Isildur's Bows");
            oIsildur.AddPermanentMobs(MobTypeEnum.Isildur);
            AddBidirectionalExitsWithOut(oCebe4, oIsildur, "shop");
            eldemondeGraph.Rooms[oIsildur] = new System.Windows.Point(5, 5.5);

            Room oElros4 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros4, oElrosCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros4] = new System.Windows.Point(5, 7);

            Room oGate = AddRoom("Palace Gate", "The Palace of Eldalondë");
            AddBidirectionalSameNameExit(oElros4, oGate, "gate");
            eldemondeGraph.Rooms[oGate] = new System.Windows.Point(4.6, 6.5);

            Room oPalaceSouth = AddRoom("Palace", "The Palace of Eldalondë");
            AddBidirectionalSameNameExit(oGate, oPalaceSouth, "stairway");
            eldemondeGraph.Rooms[oPalaceSouth] = new System.Windows.Point(4.6, 3);

            Room oPalaceSouthwest = AddRoom("Palace", "West Wing");
            AddBidirectionalExits(oPalaceSouthwest, oPalaceSouth, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oPalaceSouthwest] = new System.Windows.Point(3.6, 3);

            Room oFaeldor = AddRoom("Faeldor", "Elven Embassy");
            oFaeldor.AddPermanentMobs(MobTypeEnum.Faeldor);
            AddBidirectionalSameNameExit(oPalaceSouthwest, oFaeldor, "door");
            eldemondeGraph.Rooms[oFaeldor] = new System.Windows.Point(2.6, 3);

            Room oPalaceSoutheast = AddRoom("Palace", "East Wing");
            AddBidirectionalExits(oPalaceSouth, oPalaceSoutheast, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oPalaceSoutheast] = new System.Windows.Point(5.6, 3);

            Room oGrimaxeGrimson = AddRoom("Grimaxe Grimson", "Dwarven Embassy");
            oGrimaxeGrimson.AddPermanentMobs(MobTypeEnum.GrimaxeGrimson);
            AddBidirectionalSameNameExit(oPalaceSoutheast, oGrimaxeGrimson, "door");
            eldemondeGraph.Rooms[oGrimaxeGrimson] = new System.Windows.Point(6.6, 3);

            Room oMirrorHallEast = AddRoom("Mirror Hall", "Mirror Hall");
            AddBidirectionalExits(oMirrorHallEast, oPalaceSoutheast, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallEast] = new System.Windows.Point(5.6, 2);

            Room oMirrorHallCenter = AddRoom("Mirror Hall", "Mirror Hall");
            AddBidirectionalExits(oMirrorHallCenter, oMirrorHallEast, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMirrorHallCenter, oPalaceSouth, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallCenter] = new System.Windows.Point(4.6, 2);

            Room oMirrorHallWest = AddRoom("Mirror Hall", "Mirror Hall");
            AddBidirectionalExits(oMirrorHallWest, oMirrorHallCenter, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMirrorHallWest, oPalaceSouthwest, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallWest] = new System.Windows.Point(3.6, 2);

            Room oThroneHallWest = AddRoom("Throne Hall", "Throne hall");
            AddExit(oMirrorHallWest, oThroneHallWest, "hall");
            AddExit(oThroneHallWest, oMirrorHallWest, "south");
            eldemondeGraph.Rooms[oThroneHallWest] = new System.Windows.Point(3.6, 1);

            Room oThroneHallCenter = AddRoom("Throne Hall", "Throne Hall");
            AddBidirectionalExits(oThroneHallWest, oThroneHallCenter, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oThroneHallCenter] = new System.Windows.Point(4.6, 1);

            Room oThroneHallEast = AddRoom("Throne Hall", "Throne Hall");
            AddBidirectionalExits(oThroneHallCenter, oThroneHallEast, BidirectionalExitType.WestEast);
            AddExit(oMirrorHallEast, oThroneHallEast, "hall");
            AddExit(oThroneHallEast, oMirrorHallEast, "south");
            eldemondeGraph.Rooms[oThroneHallEast] = new System.Windows.Point(5.6, 1);

            Room oThroneHall = AddRoom("Throne Hall", "Throne Hall");
            AddBidirectionalSameNameExit(oThroneHallCenter, oThroneHall, "stairs");
            eldemondeGraph.Rooms[oThroneHall] = new System.Windows.Point(4.6, 0);

            Room oDorie4 = AddRoom("Dori", "Tower of Yotha");
            AddBidirectionalExits(oDorie4, oDorieCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie4] = new System.Windows.Point(5, 8);

            Room oYothasTower = AddRoom("Yotha Tower", "Yotha's Tower");
            AddBidirectionalSameNameExit(oDorie4, oYothasTower, "door");
            eldemondeGraph.Rooms[oYothasTower] = new System.Windows.Point(5, 7.8);

            Room oChamberOfEnergy = AddRoom("Chamber of Energy", "Chamber of Energy");
            AddBidirectionalExits(oChamberOfEnergy, oYothasTower, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oChamberOfEnergy] = new System.Windows.Point(5, 7.6);

            Room oChamberOfIllusions = AddRoom("Chamber of Illusions", "Chamber of Illusions");
            AddBidirectionalExits(oChamberOfIllusions, oChamberOfEnergy, BidirectionalExitType.UpDown);
            AddExit(oChamberOfIllusions, oChamberOfEnergy, "up");
            eldemondeGraph.Rooms[oChamberOfIllusions] = new System.Windows.Point(5, 7.4);

            Room oYothasChamber = AddRoom("Yotha's Chamber", "Yotha's Chamber");
            Exit e = AddExit(oChamberOfIllusions, oYothasChamber, "wall");
            e.Hidden = true;
            AddExit(oYothasChamber, oChamberOfIllusions, "wall");
            eldemondeGraph.Rooms[oYothasChamber] = new System.Windows.Point(5, 7.2);

            Room oIoth2 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth2, oIoth1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth2] = new System.Windows.Point(4, 4);

            Room oCebe5 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe5, oCebe4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe5] = new System.Windows.Point(4, 6);

            Room oSssreth = AddRoom("Sssreth", "Sssreth's Potion Shop");
            oSssreth.AddPermanentMobs(MobTypeEnum.SssrethTheLizardman);
            AddBidirectionalExitsWithOut(oCebe5, oSssreth, "south");
            eldemondeGraph.Rooms[oSssreth] = new System.Windows.Point(4, 6.5);

            Room oElros5 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros5, oElros4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros5] = new System.Windows.Point(4, 7);

            Room oDorie5 = AddRoom("Elros Statue", "Statue of Elros");
            AddBidirectionalExits(oDorie5, oDorie4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie5] = new System.Windows.Point(4, 8);

            Room oIothNundine = AddRoom("North Gate Inside", "Eldalondë, North Gate");
            AddBidirectionalExits(oIothNundine, oIoth2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIothNundine] = new System.Windows.Point(3, 4);

            Room oNundine1 = AddRoom("Nundine", "Nundine Street");
            AddBidirectionalExits(oIothNundine, oNundine1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oNundine1] = new System.Windows.Point(3, 5);

            Room oKegTavern = AddRoom("Keg Tavern", "The Keg Tavern");
            AddBidirectionalExitsWithOut(oNundine1, oKegTavern, "west");
            eldemondeGraph.Rooms[oKegTavern] = new System.Windows.Point(2, 5.3);

            Room oTavernKitchen = AddRoom("Tavern Kitchen", "Tavern Kitchen");
            e = AddExit(oKegTavern, oTavernKitchen, "door");
            e.Hidden = true;
            AddExit(oTavernKitchen, oKegTavern, "door");
            eldemondeGraph.Rooms[oTavernKitchen] = new System.Windows.Point(2, 4.7);

            Room oCebeNundine = AddRoom("Cebe/Nundine", "Cebe Avenue / Nundine Street");
            AddBidirectionalExits(oNundine1, oCebeNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCebeNundine, oCebe5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebeNundine] = new System.Windows.Point(3, 6);

            Room oElrosNundine = AddRoom("Elros/Nundine", "Elros Boulevard / Nundine Street");
            AddBidirectionalExits(oCebeNundine, oElrosNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElrosNundine, oElros5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElrosNundine] = new System.Windows.Point(3, 7);

            Room oDoriNundine = AddRoom("Dori/Nundine", "Dorië Avenue / Nundine Street");
            AddBidirectionalExits(oElrosNundine, oDoriNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDoriNundine, oDorie5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDoriNundine] = new System.Windows.Point(3, 8);

            Room oIoth3 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth3, oIothNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth3] = new System.Windows.Point(2, 4);

            Room oCebe6 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe6, oCebeNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe6] = new System.Windows.Point(2, 6);

            Room oElros6 = AddRoom("Wish Fountain", "The Fountain of Wishes");
            AddBidirectionalExits(oElros6, oElrosNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros6] = new System.Windows.Point(2, 7);

            Room oDori6 = AddRoom("Dark Lord Taunting", "The Taunting of The Dark Lord");
            AddBidirectionalExits(oDori6, oDoriNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori6] = new System.Windows.Point(2, 8);

            Room oIoth4 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth4, oIoth3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth4] = new System.Windows.Point(1, 4);

            Room oSmallPath = AddRoom("Small Path", "Small path");
            e = AddExit(oIoth4, oSmallPath, "south");
            e.Hidden = true;
            e = AddExit(oSmallPath, oTavernKitchen, "backdoor");
            e.Hidden = true;
            AddExit(oSmallPath, oIoth4, "north");
            eldemondeGraph.Rooms[oSmallPath] = new System.Windows.Point(1, 5);

            Room oCebe7 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oSmallPath, oCebe7, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCebe7, oCebe6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe7] = new System.Windows.Point(1, 6);

            Room oBezanthi = AddRoom("Bezanthi", "Bezanthi's Trading Post");
            oBezanthi.AddPermanentMobs(MobTypeEnum.Bezanthi);
            AddBidirectionalExitsWithOut(oCebe7, oBezanthi, "shop");
            eldemondeGraph.Rooms[oBezanthi] = new System.Windows.Point(0, 6);

            Room oElros7 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros7, oElros6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros7] = new System.Windows.Point(1, 7);

            Room oDori7 = AddRoom("Dori", "Dorië Avenue");
            AddBidirectionalExits(oDori7, oDori6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori7] = new System.Windows.Point(1, 8);

            Room oElros8 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oCebe7, oElros8, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oElros8, oElros7, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oElros8, oDori7, BidirectionalExitType.SoutheastNorthwest);
            eldemondeGraph.Rooms[oElros8] = new System.Windows.Point(0, 7);

            Room oCityWalkway1 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oDorie1, oCityWalkway1, BidirectionalExitType.SouthwestNortheast);
            eldemondeGraph.Rooms[oCityWalkway1] = new System.Windows.Point(8, 9);

            Room oCityWalkway2 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oCityWalkway2, oCityWalkway1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway2] = new System.Windows.Point(6, 9);

            Room oCityWalkway3 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oCityWalkway3, oCityWalkway2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway3] = new System.Windows.Point(4, 9);

            Room oCityWalkway4 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oDori7, oCityWalkway4, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oCityWalkway4, oCityWalkway3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway4] = new System.Windows.Point(2, 9);
        }

        private void AddMapBoundaryPoint(Room r1, Room r2, MapType mt1, MapType mt2)
        {
            AddRoomMapDisambiguation(r1, mt1);
            AddRoomMapDisambiguation(r2, mt2);
            AddBoundaryPointToMap(r1, mt2);
            AddBoundaryPointToMap(r2, mt1);
        }
        private void AddRoomMapDisambiguation(Room r, MapType mt)
        {
            RoomsToMaps[r] = mt;
            AddBoundaryPointToMap(r, mt);
        }
        private void AddBoundaryPointToMap(Room r, MapType mt)
        {
            if (!BoundaryPointsToMaps.TryGetValue(r, out List<MapType> mts))
            {
                mts = new List<MapType>();
                BoundaryPointsToMaps[r] = mts;
            }
            if (!mts.Contains(mt))
            {
                mts.Add(mt);
            }
        }

        private Room AddRoom(string roomName, string backendName)
        {
            Room r = new Room(roomName, backendName);
            _map.AddVertex(r);
            if (AmbiguousRoomsByBackendName.TryGetValue(backendName, out List<Room> rooms))
            {
                rooms.Add(r);
            }
            else if (UnambiguousRoomsByBackendName.TryGetValue(backendName, out Room existingRoom))
            {
                UnambiguousRoomsByBackendName.Remove(backendName);
                AmbiguousRoomsByBackendName[backendName] = new List<Room>() { existingRoom, r };
            }
            else
            {
                UnambiguousRoomsByBackendName[backendName] = r;
            }
            if (UnambiguousRoomsByDisplayName.TryGetValue(roomName, out _))
            {
                UnambiguousRoomsByDisplayName[roomName] = null;
            }
            else
            {
                UnambiguousRoomsByDisplayName[roomName] = r;
            }
            return r;
        }

        private Room AddPawnShoppeRoom(string roomName, string backendName, PawnShoppe pawnShoppe)
        {
            Room r = AddRoom(roomName, backendName);
            r.PawnShoppe = pawnShoppe;
            PawnShoppes[pawnShoppe] = r;
            return r;
        }

        private Room AddHealingRoom(string roomName, string backendName, HealingRoom healingRoom)
        {
            Room r = AddRoom(roomName, backendName);
            r.HealingRoom = healingRoom;
            HealingRooms[healingRoom] = r;
            return r;
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

        /// <summary>
        /// adds bidirectional exits with the out exit as out
        /// </summary>
        /// <param name="aRoom">entrance room</param>
        /// <param name="bRoom">in room</param>
        /// <param name="inText">exit text from entrance room to in room</param>
        /// <returns>in exit</returns>
        private Exit AddBidirectionalExitsWithOut(Room aRoom, Room bRoom, string inText)
        {
            return AddBidirectionalExitsWithOut(aRoom, bRoom, inText, false);
        }

        /// <summary>
        /// adds birectional exits with the out exit as out
        /// </summary>
        /// <param name="aRoom">entrance room</param>
        /// <param name="bRoom">in room</param>
        /// <param name="inText">exit text from entrance room to in room</param>
        /// <param name="inExitIsMustOpen">whether the in exit must be opened</param>
        /// <returns>in exit</returns>
        private Exit AddBidirectionalExitsWithOut(Room aRoom, Room bRoom, string inText, bool inExitIsMustOpen)
        {
            Exit e = AddExit(aRoom, bRoom, inText);
            e.MustOpen = inExitIsMustOpen;
            AddExit(bRoom, aRoom, "out");
            return e;
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
    }

    internal static class MapComputation
    {
        public static List<Exit> ComputeLowestCostPath(Room currentRoom, Room targetRoom, GraphInputs graphInputs)
        {
            if (currentRoom == null)
            {
                return null;
            }

            List<Exit> ret = null;
            Dictionary<Room, Exit> pathMapping = new Dictionary<Room, Exit>();
            GenericPriorityQueue<ExitPriorityNode, int> pq = new GenericPriorityQueue<ExitPriorityNode, int>(2000);

            pathMapping[currentRoom] = null;
            foreach (Exit e in currentRoom.Exits)
            {
                if (!pathMapping.ContainsKey(e.Target))
                {
                    int cost = e.GetCost(graphInputs);
                    if (cost != int.MaxValue)
                    {
                        pq.Enqueue(new ExitPriorityNode(e), cost);
                    }
                }
                
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
                        foreach (Exit e in nextNodeTarget.Exits)
                        {
                            if (!pathMapping.ContainsKey(e.Target))
                            {
                                int cost = e.GetCost(graphInputs);
                                if (cost != int.MaxValue)
                                {
                                    pq.Enqueue(new ExitPriorityNode(e), iPriority + cost);
                                }
                            }
                        }
                    }
                }
            }
            return ret;
        }
    }

    internal class GraphInputs
    {
        public GraphInputs(ClassType Class, int Level, bool IsDay, bool Flying, bool Levitating)
        {
            this.Class = Class;
            this.Level = Level;
            this.IsDay = IsDay;
            this.Flying = Flying;
            this.Levitating = Levitating;
        }
        public bool Flying { get; set; }
        public bool Levitating { get; set; }
        public bool IsDay { get; set; }
        public int Level { get; set; }
        public ClassType Class { get; set; }
    }
}
