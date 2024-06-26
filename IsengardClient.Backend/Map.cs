﻿using Priority_Queue;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace IsengardClient.Backend
{
    public class IsengardMap
    {
        internal AdjacencyGraph<Room, Exit> _map;
        private Dictionary<MapType, RoomGraph> _graphs;
        public Dictionary<HealingRoom, Room> HealingRooms = new Dictionary<HealingRoom, Room>();
        public Dictionary<PawnShoppe, Room> PawnShoppes = new Dictionary<PawnShoppe, Room>();
        public Dictionary<Room, MapType> RoomsToMaps = new Dictionary<Room, MapType>();
        public Dictionary<Room, List<MapType>> BoundaryPointsToMaps = new Dictionary<Room, List<MapType>>();
        public Dictionary<ItemTypeEnum, MobTypeEnum> Trades = new Dictionary<ItemTypeEnum, MobTypeEnum>();
        public Dictionary<MobTypeEnum, Room> MobRooms = new Dictionary<MobTypeEnum, Room>();

        /// <summary>
        /// maps room backend names to rooms when unambiguous, ambiguous rooms have a separate mapping.
        /// </summary>
        public Dictionary<string, Room> UnambiguousRoomsByBackendName = new Dictionary<string, Room>();

        /// <summary>
        /// maps room backend names to rooms when ambiguous.
        /// </summary>
        public Dictionary<string, List<Room>> AmbiguousRoomsByBackendName = new Dictionary<string, List<Room>>();

        /// <summary>
        /// maps room display names to rooms when unambiguous. for ambiguous room names the value is null.
        /// </summary>
        public Dictionary<string, Room> UnambiguousRoomsByDisplayName = new Dictionary<string, Room>();

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
            AddBreeToHobbiton(oBreeWestGateInside, oSmoulderingVillage, out Room westronRoadToMithlond, out Room valleyRoad, out Room westernRoadWestOfHobbiton);
            AddBreeToImladris(out Room oOuthouse, breeEastGateInside, breeEastGateOutside, out Room imladrisWestGateOutside, oCemetery, out Room southernBrethilForestNWEdge, out Room southernBrethilForestSW, out Room southernBrethilForestSE, out Room southernBrethilForestNE, out Room smugglersVillage2);
            AddUnderBree(oNorthBridge, oOuthouse, oSewerPipeExit);
            AddImladrisCity(out Room oImladrisSouthGateInside, out Room oEastGateOfImladrisOutside, imladrisWestGateOutside, out Room healingHand, out Room oEastGateOfImladrisInside);
            AddEastOfImladris(oEastGateOfImladrisOutside, oEastGateOfImladrisInside, out Room westGateOfEsgaroth);
            AddImladrisToTharbad(oImladrisSouthGateInside, out Room oTharbadGateOutside, southernBrethilForestNWEdge, southernBrethilForestSW, southernBrethilForestSE, southernBrethilForestNE);
            AddTharbadCity(oTharbadGateOutside, out Room tharbadWestGateOutside, out Room tharbadDocks, out Room tharbadEastGate);
            AddWestOfTharbad(tharbadWestGateOutside);
            AddEastOfTharbad(tharbadEastGate);
            AddEsgaroth(westGateOfEsgaroth, out Room esgarothNorthGateOutside);
            AddNorthOfEsgaroth(esgarothNorthGateOutside);
            AddNindamos(out Room oArmenelosGatesOutside, out Room oSouthernJunction, out Room oPathThroughTheValleyHiddenPath, out Room nindamosDocks, out RoomGraph nindamosGraph, out Room nindamosVillageCenter);
            AddArmenelos(oArmenelosGatesOutside);
            AddWestOfNindamosAndArmenelos(oSouthernJunction, oPathThroughTheValleyHiddenPath, out Room oEldemondeEastGateOutside, nindamosGraph);
            AddEldemondeCity(oEldemondeEastGateOutside);
            AddMithlond(breeDocks, boatswain, tharbadDocks, nindamosDocks, westronRoadToMithlond, smugglersVillage2, valleyRoad, westernRoadWestOfHobbiton);
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
                g.Rooms = new Dictionary<Room, PointF>();
                foreach (KeyValuePair<Room, PointF> next in oldRooms)
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
                    g.Rooms[r] = new PointF(next.Value.X * g.ScalingFactor, next.Value.Y * g.ScalingFactor);
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
                if (nextExit.BoatExitType != BoatExitType.None)
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

            tharbadEastGraph.Rooms[tharbadEastGate] = new PointF(0, 4);

            Room oAlliskPlainsEntrance = AddRoom("Entrance", "Path around Allisk Plains");
            AddBidirectionalExits(tharbadEastGate, oAlliskPlainsEntrance, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEntrance] = new PointF(1, 4);

            Room oAlliskPlainsEastPath1 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsEntrance, oAlliskPlainsEastPath1, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath1] = new PointF(1.5F, 4);
            //CSRTODO: narrow path

            Room oAlliskPlainsEastPath2 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastPath1, oAlliskPlainsEastPath2, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath2] = new PointF(2, 4);

            Room oAlliskPlainsEastPath3 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastPath2, oAlliskPlainsEastPath3, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath3] = new PointF(2.5F, 4);

            Room oAlliskPlainsEastPath4Fork = AddRoom("Fork", "Fork in the Path");
            AddBidirectionalExits(oAlliskPlainsEastPath3, oAlliskPlainsEastPath4Fork, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastPath4Fork] = new PointF(3, 4);

            Room oAlliskPlainsEastTrail1 = AddRoom("Trail", "Trail through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastPath4Fork, oAlliskPlainsEastTrail1, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastTrail1] = new PointF(3.5F, 4);

            Room oAlliskPlainsEastTrail2 = AddRoom("Trail", "Trail through the Plains");
            AddBidirectionalExits(oAlliskPlainsEastTrail1, oAlliskPlainsEastTrail2, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oAlliskPlainsEastTrail2] = new PointF(4, 4.5F);

            Room oAlliskPlainsEastTrailDeadEnd = AddRoom("Dead End", "Dead End");
            AddBidirectionalExits(oAlliskPlainsEastTrail2, oAlliskPlainsEastTrailDeadEnd, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastTrailDeadEnd] = new PointF(4.5F, 4.5F);

            Room oAlliskPlainsEastBend = AddRoom("Bend", "Bend in the Path");
            AddBidirectionalExits(oAlliskPlainsEastBend, oAlliskPlainsEastPath4Fork, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oAlliskPlainsEastBend] = new PointF(6, 3);

            Room oAlliskPlainsNortheastPath1 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsNortheastPath1, oAlliskPlainsEastBend, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oAlliskPlainsNortheastPath1] = new PointF(10, 2);

            Room oAlliskPlainsNortheastPath2 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsNortheastPath2, oAlliskPlainsNortheastPath1, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsNortheastPath2] = new PointF(9, 2);

            Room oAlliskPlainsNortheastPath3 = AddRoom("Path", "Path through the Plains");
            AddBidirectionalExits(oAlliskPlainsNortheastPath3, oAlliskPlainsNortheastPath2, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oAlliskPlainsNortheastPath3] = new PointF(8, 2);

            Room oNorthPathAroundAlliskPlains1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains1, oAlliskPlainsEntrance, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains1] = new PointF(2, 1);

            Room oNorthPathAroundAlliskPlains2 = AddRoom("Path", "Path through Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains1, oNorthPathAroundAlliskPlains2, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains2] = new PointF(4, 1);

            Room oNorthPathAroundAlliskPlains3 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains2, oNorthPathAroundAlliskPlains3, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains3] = new PointF(6, 1);

            Room oNorthPathAroundAlliskPlains4 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oNorthPathAroundAlliskPlains3, oNorthPathAroundAlliskPlains4, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oNorthPathAroundAlliskPlains4, oAlliskPlainsNortheastPath3, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oNorthPathAroundAlliskPlains4] = new PointF(8, 1);

            Room oSoutheastPathAroundAlliskPlains1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oAlliskPlainsEntrance, oSoutheastPathAroundAlliskPlains1, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains1] = new PointF(2, 5);

            Room oSoutheastPathAroundAlliskPlains2 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains1, oSoutheastPathAroundAlliskPlains2, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains2] = new PointF(3, 6);

            Room oSoutheastPathAroundAlliskPlains3 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains2, oSoutheastPathAroundAlliskPlains3, BidirectionalExitType.WestEast);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains3] = new PointF(4, 6);

            Room oSoutheastPathAroundAlliskPlains4 = AddRoom("Allisk Plains", "Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains4, oSoutheastPathAroundAlliskPlains3, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oNorthPathAroundAlliskPlains4, oSoutheastPathAroundAlliskPlains4, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oSoutheastPathAroundAlliskPlains4] = new PointF(6, 3.5F);
            //CSRTODO: sloping path

            Room oSouthPathAlliskPlains1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSoutheastPathAroundAlliskPlains2, oSouthPathAlliskPlains1, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains1] = new PointF(4, 7);

            Room oSouthPathAlliskPlains2 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains1, oSouthPathAlliskPlains2, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains2] = new PointF(4, 8);

            Room oSouthPathAlliskPlains3 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains2, oSouthPathAlliskPlains3, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains3] = new PointF(4, 9);

            Room oSouthPathAlliskPlains4 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains3, oSouthPathAlliskPlains4, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains4] = new PointF(3, 10);

            Room oSouthPathAlliskPlains5 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains4, oSouthPathAlliskPlains5, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains5] = new PointF(4, 11);

            Room oSouthPathAlliskPlains6 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains5, oSouthPathAlliskPlains6, BidirectionalExitType.SoutheastNorthwest);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains6] = new PointF(5, 12);

            Room oSouthPathAlliskPlains7 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains7, oSouthPathAlliskPlains6, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains7] = new PointF(5, 11);

            Room oSouthPathAlliskPlains8 = AddRoom("Allisk plains", "Allisk plains");
            AddExit(oSouthPathAlliskPlains7, oSouthPathAlliskPlains8, "northeast");
            Exit e = AddExit(oSoutheastPathAroundAlliskPlains4, oSouthPathAlliskPlains8, "southeast");
            e.Hidden = true;
            AddExit(oSouthPathAlliskPlains8, oSoutheastPathAroundAlliskPlains4, "northwest");
            tharbadEastGraph.Rooms[oSouthPathAlliskPlains8] = new PointF(7, 4.5F);
            //CSRTODO: pit (hidden)

            Room oPathToOgres1 = AddRoom("Path", "Path around Allisk Plains");
            AddBidirectionalExits(oSouthPathAlliskPlains6, oPathToOgres1, BidirectionalExitType.NorthSouth);
            tharbadEastGraph.Rooms[oPathToOgres1] = new PointF(5, 13);

            Room oTrakardOgreRanger = AddRoom("Ogre Rangers", "Path around Allisk Plains");
            AddPermanentMobs(oTrakardOgreRanger, MobTypeEnum.TrakardOgreRanger, MobTypeEnum.TrakardOgreRanger);
            AddBidirectionalExits(oPathToOgres1, oTrakardOgreRanger, BidirectionalExitType.SouthwestNortheast);
            tharbadEastGraph.Rooms[oTrakardOgreRanger] = new PointF(4, 14);
        }

        private void AddWestOfTharbad(Room tharbadWestGateOutside)
        {
            RoomGraph tharbadWestGraph = _graphs[MapType.WestOfTharbad];
            RoomGraph tharbadGraph = _graphs[MapType.Tharbad];

            tharbadWestGraph.Rooms[tharbadWestGateOutside] = new PointF(6, 5);

            Room lelionBeachAndPark = AddRoom("Lelion Beach and Park", "Lelion Beach and Park");
            AddBidirectionalSameNameExit(tharbadWestGateOutside, lelionBeachAndPark, "ramp");
            tharbadWestGraph.Rooms[lelionBeachAndPark] = new PointF(5, 5);
            tharbadGraph.Rooms[lelionBeachAndPark] = new PointF(-1, 7);
            AddMapBoundaryPoint(tharbadWestGateOutside, lelionBeachAndPark, MapType.Tharbad, MapType.WestOfTharbad);

            Room beachPath = AddRoom("Beach Path", "Beach Path");
            AddBidirectionalExits(lelionBeachAndPark, beachPath, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[beachPath] = new PointF(5, 6);

            Room marinersAnchor = AddRoom("Mariner's Anchor", "The Mariner's Anchor");
            AddBidirectionalExitsWithOut(beachPath, marinersAnchor, "west");
            tharbadWestGraph.Rooms[marinersAnchor] = new PointF(4, 6);

            Room dalePurves = AddRoom("Dale Purves", "Dale's Beach");
            AddPermanentMobs(dalePurves, MobTypeEnum.DalePurves);
            AddExit(marinersAnchor, dalePurves, "back door");
            AddExit(dalePurves, marinersAnchor, "east");
            tharbadWestGraph.Rooms[dalePurves] = new PointF(3, 6);

            Room greyfloodRiver1 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver1.DamageType = RoomDamageType.Water;
            AddExit(dalePurves, greyfloodRiver1, "river");
            AddExit(greyfloodRiver1, dalePurves, "beach");
            tharbadWestGraph.Rooms[greyfloodRiver1] = new PointF(2, 6);

            Room greyfloodRiver2 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver2.DamageType = RoomDamageType.Water;
            AddBidirectionalExits(greyfloodRiver1, greyfloodRiver2, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[greyfloodRiver2] = new PointF(2, 7);

            Room greyfloodRiver3 = AddRoom("Greyflood River", "The Greyflood River");
            greyfloodRiver3.DamageType = RoomDamageType.Water;
            AddBidirectionalExits(greyfloodRiver2, greyfloodRiver3, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[greyfloodRiver3] = new PointF(2, 8);

            Room riverMouth = AddRoom("River Mouth", "The Mouth of the Greyflood River");
            riverMouth.DamageType = RoomDamageType.Water;
            AddExit(greyfloodRiver3, riverMouth, "southwest");
            AddExit(riverMouth, greyfloodRiver3, "river");
            tharbadWestGraph.Rooms[riverMouth] = new PointF(1, 9);

            Room oWesternBeachPath1 = AddRoom("Western Beach Path", "Western Beach Path");
            AddBidirectionalExits(oWesternBeachPath1, riverMouth, BidirectionalExitType.WestEast);
            tharbadWestGraph.Rooms[oWesternBeachPath1] = new PointF(0, 9);

            Room oWesternBeachPath2 = AddRoom("Western Beach Path", "Western Beach Path");
            AddBidirectionalExits(oWesternBeachPath2, oWesternBeachPath1, BidirectionalExitType.SouthwestNortheast);
            tharbadWestGraph.Rooms[oWesternBeachPath2] = new PointF(1, 8);

            Room oBottomOfTheRocks = AddRoom("Bottom of the Rocks", "Bottom Of The Rocks");
            AddBidirectionalExits(oBottomOfTheRocks, oWesternBeachPath2, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oBottomOfTheRocks] = new PointF(1, 7);

            Room oRockSlide = AddRoom("Rock Slide", "Rock Slide");
            AddBidirectionalExits(oRockSlide, oBottomOfTheRocks, BidirectionalExitType.UpDown);
            tharbadWestGraph.Rooms[oRockSlide] = new PointF(1, 6);

            Room oDropOff = AddRoom("Drop Off", "Drop Off");
            AddBidirectionalExits(oDropOff, oRockSlide, BidirectionalExitType.UpDown);
            tharbadWestGraph.Rooms[oDropOff] = new PointF(1, 5);

            Room oErynVornSouth = AddRoom("Eryn Vorn South", "South Edge of Eryn Vorn");
            AddBidirectionalExits(oErynVornSouth, oDropOff, BidirectionalExitType.SoutheastNorthwest);
            tharbadWestGraph.Rooms[oErynVornSouth] = new PointF(0, 4);

            Room oLelionParkHillside = AddRoom("Lelion Park Hillside", "Lelion Park Hillside");
            AddBidirectionalExits(oLelionParkHillside, lelionBeachAndPark, BidirectionalExitType.SoutheastNorthwest);
            tharbadWestGraph.Rooms[oLelionParkHillside] = new PointF(5, 4);

            Room oChildrensTidalPool = AddRoom("Children's Tidal Pool", "Children's Tidal Pool");
            AddPermanentMobs(oChildrensTidalPool, MobTypeEnum.SmallCrab, MobTypeEnum.UglyKid);
            AddBidirectionalExits(oChildrensTidalPool, oLelionParkHillside, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oChildrensTidalPool] = new PointF(5, 3);

            Room oNorthShore = AddRoom("North Shore", "North Shore");
            AddBidirectionalExits(oNorthShore, oChildrensTidalPool, BidirectionalExitType.WestEast);
            tharbadWestGraph.Rooms[oNorthShore] = new PointF(4, 3);

            Room oLelionPark = AddRoom("Lelion Park", "Lelion Park");
            AddBidirectionalExits(oLelionPark, lelionBeachAndPark, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oNorthShore, oLelionPark, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oLelionPark] = new PointF(4, 5);

            Room oSouthCoveSandBar = AddRoom("South Cove Sand Bar", "South Cove Sand Bar");
            AddBidirectionalSameNameExit(oLelionPark, oSouthCoveSandBar, "drainage ditch");
            tharbadWestGraph.Rooms[oSouthCoveSandBar] = new PointF(3, 5);

            Room oMultiTurnPath = AddRoom("Multi-turn Path", "Multi-turn Path");
            AddBidirectionalExits(oMultiTurnPath, oSouthCoveSandBar, BidirectionalExitType.SoutheastNorthwest);
            tharbadWestGraph.Rooms[oMultiTurnPath] = new PointF(2, 4);

            Room oCrookedPath = AddRoom("Crooked Path", "Crooked Path");
            AddExit(oMultiTurnPath, oCrookedPath, "west");
            AddExit(oMultiTurnPath, oCrookedPath, "north");
            AddExit(oCrookedPath, oMultiTurnPath, "west");
            AddExit(oCrookedPath, oCrookedPath, "northeast");
            AddExit(oCrookedPath, oCrookedPath, "east");
            AddExit(oCrookedPath, oCrookedPath, "north");
            AddExit(oCrookedPath, oCrookedPath, "south");
            tharbadWestGraph.Rooms[oCrookedPath] = new PointF(1, 4);

            Room oNorthShoreGrotto = AddRoom("North Shore Grotto", "North Shore Grotto");
            Exit e = AddExit(oNorthShore, oNorthShoreGrotto, "west");
            e.IsTrapExit = true;
            AddExit(oNorthShoreGrotto, oCrookedPath, "southwest");
            tharbadWestGraph.Rooms[oNorthShoreGrotto] = new PointF(3, 3);

            Room oNorthLookoutPoint = AddRoom("North Lookout Point", "North Lookout Point");
            AddExit(oNorthShoreGrotto, oNorthLookoutPoint, "west");
            AddExit(oNorthLookoutPoint, oCrookedPath, "south");
            tharbadWestGraph.Rooms[oNorthLookoutPoint] = new PointF(0, 3);

            Room oNorthShoreShallowWaters = AddRoom("Shallow Waters", "North Shore Shallow Waters");
            AddBidirectionalExits(oNorthShoreShallowWaters, oNorthShore, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oNorthShoreShallowWaters] = new PointF(4, 2);

            Room oNorthShoreWaters = AddRoom("Waters", "North Shore Waters");
            AddExit(oNorthShoreShallowWaters, oNorthShoreWaters, "tide");
            AddBidirectionalExits(oNorthShoreWaters, oNorthShoreGrotto, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oNorthShoreWaters] = new PointF(4, 1);

            Room oOpenBay = AddRoom("Open Bay", "Open Bay");
            AddExit(oNorthShoreWaters, oOpenBay, "tide");
            tharbadWestGraph.Rooms[oOpenBay] = new PointF(4, 0);

            Room oNorthLookoutTower = AddRoom("North Lookout Tower", "North Lookout Tower");
            AddPermanentItems(oNorthLookoutTower, ItemTypeEnum.OldWoodenSign);
            AddExit(oOpenBay, oNorthLookoutTower, "south");
            AddBidirectionalExits(oNorthLookoutTower, oNorthLookoutPoint, BidirectionalExitType.NorthSouth);
            AddExit(oNorthShoreWaters, oNorthLookoutTower, "southwest");
            tharbadWestGraph.Rooms[oNorthLookoutTower] = new PointF(0, 2);

            Room oNorthLookoutTowerCellar = AddRoom("North Lookout Tower Cellar", "North Lookout Tower Cellar");
            e = AddExit(oNorthLookoutTower, oNorthLookoutTowerCellar, "cellar");
            e.Hidden = true;
            AddExit(oNorthLookoutTowerCellar, oNorthLookoutTower, "door");
            tharbadWestGraph.Rooms[oNorthLookoutTowerCellar] = new PointF(0, 1.5F);

            Room oShroudedTunnel = AddRoom("Shrouded Tunnel", "Shrouded Tunnel");
            e = AddBidirectionalExitsWithOut(oNorthLookoutTowerCellar, oShroudedTunnel, "shroud");
            e.Hidden = true;
            tharbadWestGraph.Rooms[oShroudedTunnel] = new PointF(0, 1);

            Room oShoreOfSeaOfTranquility1 = AddRoom("Sea Shore", "The Shore of the Sea of Tranquility");
            AddExit(riverMouth, oShoreOfSeaOfTranquility1, "shore");
            AddExit(oShoreOfSeaOfTranquility1, riverMouth, "north");
            tharbadWestGraph.Rooms[oShoreOfSeaOfTranquility1] = new PointF(1, 10);

            Room oShoreOfSeaOfTranquility2 = AddRoom("Sea Shore", "The Shore of the Sea of Tranquility");
            AddBidirectionalExits(oShoreOfSeaOfTranquility1, oShoreOfSeaOfTranquility2, BidirectionalExitType.SouthwestNortheast);
            tharbadWestGraph.Rooms[oShoreOfSeaOfTranquility2] = new PointF(0, 11);

            Room oShoreOfSeaOfTranquility3 = AddRoom("Sea Shore", "The Shore of the Sea of Tranquility");
            AddBidirectionalExits(oShoreOfSeaOfTranquility2, oShoreOfSeaOfTranquility3, BidirectionalExitType.SouthwestNortheast);
            tharbadWestGraph.Rooms[oShoreOfSeaOfTranquility3] = new PointF(-1, 12);

            Room oEntranceToThunderCove = AddRoom("Thunder Cove Entrance", "Entrance to Thunder Cove");
            AddBidirectionalExits(oShoreOfSeaOfTranquility3, oEntranceToThunderCove, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oEntranceToThunderCove] = new PointF(-1, 13);

            Room oDarkJungleEdge = AddRoom("Dark Jungle Edge", "Edge of a Dark Jungle");
            AddBidirectionalExits(oEntranceToThunderCove, oDarkJungleEdge, BidirectionalExitType.NorthSouth);
            tharbadWestGraph.Rooms[oDarkJungleEdge] = new PointF(-1, 14);

            Room oPrehistoricJungle = AddRoom("Prehistoric Jungle", "The Prehistoric Jungle");
            e = AddExit(oDarkJungleEdge, oPrehistoricJungle, "southwest");
            e.Hidden = true;
            AddExit(oPrehistoricJungle, oDarkJungleEdge, "northeast");
            tharbadWestGraph.Rooms[oPrehistoricJungle] = new PointF(-2, 15);

            Room oWildmanVillage = AddRoom("Wildman Village", "Wildman Village");
            AddExit(oDarkJungleEdge, oWildmanVillage, "path");
            AddExit(oWildmanVillage, oDarkJungleEdge, "north");
            tharbadWestGraph.Rooms[oWildmanVillage] = new PointF(-1, 15);
        }

        private void AddMithlond(Room breeDocks, Room boatswain, Room tharbadDocks, Room nindamosDocks, Room westronRoadToHobbiton, Room smugglersVillage2, Room oValleyRoad, Room westernRoadWestOfHobbiton)
        {
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];
            RoomGraph shipsGraph = _graphs[MapType.Ships];
            RoomGraph breeToImladrisGraph = _graphs[MapType.BreeToImladris];
            RoomGraph westOfBreeMap = _graphs[MapType.WestOfBree];

            Room oCelduinExpressSlip = AddRoom("Celduin Express Slip", "Pier - Slip for the Celduin Express");
            AddPermanentMobs(oCelduinExpressSlip, MobTypeEnum.HarborMaster);
            oCelduinExpressSlip.BoatLocationType = BoatEmbarkOrDisembark.CelduinExpressMithlond;
            Exit e = AddExit(boatswain, oCelduinExpressSlip, "pier");
            e.BoatExitType = BoatExitType.MithlondExitCelduinExpress;
            e = AddExit(oCelduinExpressSlip, boatswain, "gangway");
            e.BoatExitType = BoatExitType.MithlondEnterCelduinExpress;
            mithlondGraph.Rooms[oCelduinExpressSlip] = new PointF(2, 5);
            shipsGraph.Rooms[oCelduinExpressSlip] = new PointF(2, 5);
            AddMapBoundaryPoint(boatswain, oCelduinExpressSlip, MapType.Ships, MapType.Mithlond);

            Room oBullroarerSlip = AddRoom("Bullroarer Slip", "Pier - Slip for the Bullroarer");
            oBullroarerSlip.BoatLocationType = BoatEmbarkOrDisembark.BullroarerMithlond;
            AddBidirectionalExits(oCelduinExpressSlip, oBullroarerSlip, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oBullroarerSlip] = new PointF(2, 6);
            shipsGraph.Rooms[oBullroarerSlip] = new PointF(2, 6);

            Room oOmaniPrincessSlip = AddRoom("Omani Princess Slip", "Pier - Slip for the Omani Princess");
            oOmaniPrincessSlip.BoatLocationType = BoatEmbarkOrDisembark.OmaniPrincessMithlondDock;
            AddBidirectionalExits(oBullroarerSlip, oOmaniPrincessSlip, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oOmaniPrincessSlip] = new PointF(2, 7);
            shipsGraph.Rooms[oOmaniPrincessSlip] = new PointF(2, 7);

            Room oHarbringerSlip = AddRoom("Harbringer Slip", "Pier - Slip for the Harbringer");
            AddBidirectionalExits(oOmaniPrincessSlip, oHarbringerSlip, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oHarbringerSlip] = new PointF(2, 8);
            shipsGraph.Rooms[oHarbringerSlip] = new PointF(2, 8);
            AddRoomMapDisambiguation(oHarbringerSlip, MapType.Mithlond);

            Room oBayOfSomund = AddRoom("Bay of Somund", "Bay of Somund");
            AddExit(oHarbringerSlip, oBayOfSomund, "down");
            AddExit(oBayOfSomund, oHarbringerSlip, "east");
            mithlondGraph.Rooms[oBayOfSomund] = new PointF(2, 9);

            Room oHarbringerGangplank = AddRoom("Gangplank", "Gangplank");
            AddPermanentItems(oHarbringerGangplank, ItemTypeEnum.Sign);
            oHarbringerGangplank.BoatLocationType = BoatEmbarkOrDisembark.HarbringerMithlond;
            AddExit(oHarbringerSlip, oHarbringerGangplank, "gangplank");
            AddExit(oHarbringerGangplank, oHarbringerSlip, "pier");
            mithlondGraph.Rooms[oHarbringerGangplank] = new PointF(1, 8);
            shipsGraph.Rooms[oHarbringerGangplank] = new PointF(3, 8);

            Room oMithlondPort = AddRoom("Mithlond Port", "Mithlond Port");
            AddPermanentItems(oMithlondPort, ItemTypeEnum.PortManifest);
            AddExit(oCelduinExpressSlip, oMithlondPort, "north");
            AddExit(oMithlondPort, oCelduinExpressSlip, "pier");
            mithlondGraph.Rooms[oMithlondPort] = new PointF(2, 4);

            Room oEvendimTrailEnd = AddRoom("Evendim Trail", "End of the Evendim Trail");
            AddBidirectionalExits(oMithlondPort, oEvendimTrailEnd, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oEvendimTrailEnd] = new PointF(3, 5);

            Room oEvendimTrail2 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail2, oEvendimTrailEnd, BidirectionalExitType.UpDown);
            mithlondGraph.Rooms[oEvendimTrail2] = new PointF(3, 4.5F);

            Room oEvendimTrail3 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail3, oEvendimTrail2, BidirectionalExitType.UpDown);
            mithlondGraph.Rooms[oEvendimTrail3] = new PointF(3, 4);

            Room oEvendimTrail4 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail3, oEvendimTrail4, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oEvendimTrail4] = new PointF(4, 5);

            Room oEvendimTrail5 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail5, oEvendimTrail4, BidirectionalExitType.UpDown);
            mithlondGraph.Rooms[oEvendimTrail5] = new PointF(4, 4);

            Room oEvendimTrail6 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail5, oEvendimTrail6, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oEvendimTrail6] = new PointF(5, 5);

            Room oEvendimTrail7 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail6, oEvendimTrail7, BidirectionalExitType.UpDown);
            mithlondGraph.Rooms[oEvendimTrail7] = new PointF(5, 5.5F);

            Room oEvendimTrail8 = AddRoom("Trail", "Evendim Trail");
            AddExit(oEvendimTrail7, oEvendimTrail8, "down");
            e = AddExit(oEvendimTrail8, oEvendimTrail7, "up");
            e.FloatRequirement = FloatRequirement.Fly;
            mithlondGraph.Rooms[oEvendimTrail8] = new PointF(5, 6);

            Room oEvendimTrail9 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail8, oEvendimTrail9, BidirectionalExitType.UpDown);
            mithlondGraph.Rooms[oEvendimTrail9] = new PointF(5, 6.5F);

            Room oEvendimTrail10 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail9, oEvendimTrail10, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail10] = new PointF(6, 6.5F);

            Room oCollapsedTrail1 = AddRoom("Collapsed Trail", "Collapsed Trail");
            AddBidirectionalExits(oEvendimTrail10, oCollapsedTrail1, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oCollapsedTrail1] = new PointF(5, 7.5F);

            Room oCollapsedTrail2 = AddRoom("Collapsed Trail", "Collapsed Trail");
            AddBidirectionalExits(oCollapsedTrail1, oCollapsedTrail2, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oCollapsedTrail2] = new PointF(4, 8.5F);

            Room oCollapsedTrail3 = AddRoom("Collapsed Trail", "Collapsed Trail");
            AddBidirectionalExits(oCollapsedTrail2, oCollapsedTrail3, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oCollapsedTrail3] = new PointF(3, 9.5F);

            Room oCollapsedTrail4 = AddRoom("Collapsed Trail", "Collapsed Trail");
            AddBidirectionalExits(oCollapsedTrail3, oCollapsedTrail4, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oCollapsedTrail4] = new PointF(3, 10.5F);

            Room oCollapsedTrail5 = AddRoom("Collapsed Trail", "Collapsed Trail");
            AddBidirectionalExits(oCollapsedTrail4, oCollapsedTrail5, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oCollapsedTrail5] = new PointF(3, 11.5F);

            Room oCollapsedTrail6 = AddRoom("Collapsed Trail", "Collapsed Trail");
            AddBidirectionalExits(oCollapsedTrail5, oCollapsedTrail6, BidirectionalExitType.UpDown);
            mithlondGraph.Rooms[oCollapsedTrail6] = new PointF(3, 12);

            Room oCollapsedTrail7 = AddRoom("Collapsed Trail", "Collapsed Trail");
            AddBidirectionalExits(oCollapsedTrail6, oCollapsedTrail7, BidirectionalExitType.UpDown);
            mithlondGraph.Rooms[oCollapsedTrail7] = new PointF(3, 12.5F);

            Room oWestronRoad1 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oCollapsedTrail7, oWestronRoad1, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWestronRoad1] = new PointF(4, 13.5F);

            Room oWesternShore1 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWestronRoad1, oWesternShore1, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWesternShore1] = new PointF(4, 14.5F);

            Room oWesternShore2 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore1, oWesternShore2, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWesternShore2] = new PointF(4, 15.5F);

            Room oWesternShore3 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore2, oWesternShore3, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWesternShore3] = new PointF(4, 16.5F);

            Room oWesternShore4 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore3, oWesternShore4, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oWesternShore4] = new PointF(3, 17.5F);

            Room blackmireCove = AddRoom("Blackmire Cove", "Blackmire Cove");
            AddBidirectionalExits(blackmireCove, smugglersVillage2, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[blackmireCove] = new PointF(-12.25F, 6.5F);

            Room oBlackmireCove2 = AddRoom("Blackmire Cove", "Blackmire Cove");
            AddBidirectionalExits(blackmireCove, oBlackmireCove2, BidirectionalExitType.WestEast);

            Room oBeach = AddRoom("Beach", "Beach");
            AddExit(oWesternShore4, oBeach, "west");
            AddExit(oBeach, oWesternShore4, "shore");
            mithlondGraph.Rooms[oBeach] = new PointF(1, 17.5F);

            Room oEncirclingSea = AddRoom("Encircling Sea", "Encircling Sea");
            AddBidirectionalExits(oEncirclingSea, oBeach, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oEncirclingSea] = new PointF(0, 16.5F);

            Room oGreatWesternOcean = AddRoom("Ocean", "Great Western Ocean");
            AddBidirectionalExits(oGreatWesternOcean, oBeach, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oGreatWesternOcean] = new PointF(0, 17.5F);

            Room oWesternShore5 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore4, oWesternShore5, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWesternShore5] = new PointF(4, 18.5F);

            Room oWesternShore6 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore5, oWesternShore6, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWesternShore6] = new PointF(5, 19.5F);

            Room oWesternShore7 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore6, oWesternShore7, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWesternShore7] = new PointF(5, 20.5F);
            //CSRTODO: east

            Room oWesternShore8 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore7, oWesternShore8, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWesternShore8] = new PointF(6, 21.5F);

            Room oWesternShore9 = AddRoom("Western Shore", "Western Shore");
            AddPermanentMobs(oWesternShore9, MobTypeEnum.WhiteKnightPerm);
            AddBidirectionalExits(oWesternShore8, oWesternShore9, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWesternShore9] = new PointF(7, 22.5F);

            Room oWesternShore10 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore9, oWesternShore10, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWesternShore10] = new PointF(7, 23.5F);

            Room oBeach2 = AddRoom("Beach", "Beach");
            AddBidirectionalExits(oWesternShore10, oBeach2, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oBeach2, blackmireCove, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oBeach2] = new PointF(8, 24.5F);
            mithlondGraph.Rooms[blackmireCove] = new PointF(9, 25.5F);
            mithlondGraph.Rooms[oBlackmireCove2] = new PointF(10, 25.5F);
            mithlondGraph.Rooms[smugglersVillage2] = new PointF(10, 26.5F);
            AddMapBoundaryPoint(blackmireCove, smugglersVillage2, MapType.Mithlond, MapType.BreeToImladris);

            Room oBeach3 = AddRoom("Beach", "Beach");
            AddBidirectionalExits(oBeach3, oBeach2, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oBeach3] = new PointF(8, 24);

            Room oGreatWesternOcean2 = AddRoom("Ocean", "Great Western Ocean");
            AddBidirectionalExits(oGreatWesternOcean2, oBeach2, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oGreatWesternOcean2] = new PointF(6, 24.5F);

            Room oTidalPool = AddRoom("Tidal Pool", "Tidal Pool");
            AddBidirectionalExits(oTidalPool, oGreatWesternOcean2, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oTidalPool] = new PointF(5, 24.5F);

            Room oGreatWesternOcean3 = AddRoom("Ocean", "Great Western Ocean");
            AddBidirectionalExits(oGreatWesternOcean3, oBeach3, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oGreatWesternOcean3, oGreatWesternOcean2, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oGreatWesternOcean3] = new PointF(6, 24);

            Room oSandBar = AddRoom("Sand Bar", "Sand Bar");
            AddExit(oTidalPool, oSandBar, "north");
            AddBidirectionalExits(oSandBar, oGreatWesternOcean3, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oSandBar] = new PointF(5, 24);

            Room oEdgeOfWoods = AddRoom("Edge of Woods", "Edge of Woods");
            AddExit(oBeach3, oEdgeOfWoods, "east");
            AddExit(oEdgeOfWoods, oBeach3, "beach");
            mithlondGraph.Rooms[oEdgeOfWoods] = new PointF(9, 24);

            Room oWornTrail = AddRoom("Worn Trail", "Worn Trail");
            AddBidirectionalExits(oEdgeOfWoods, oWornTrail, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oWornTrail] = new PointF(10, 24);

            Room oForest = AddRoom("Forest", "Forest");
            AddBidirectionalExits(oForest, oWornTrail, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oForest] = new PointF(10, 23.75F);

            Room oBriarPatch = AddRoom("Briar Patch", "Briar Patch");
            AddBidirectionalExits(oBriarPatch, oForest, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oBriarPatch] = new PointF(11, 23.5F);

            Room oDirtPath = AddRoom("Dirt Path", "Dirt Path");
            AddExit(oBeach3, oDirtPath, "north");
            AddExit(oDirtPath, oBeach2, "beach");
            mithlondGraph.Rooms[oDirtPath] = new PointF(8, 23);

            Room oDirtPath2 = AddRoom("Dirt Path", "Dirt Path");
            AddBidirectionalExits(oDirtPath2, oDirtPath, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oDirtPath2] = new PointF(9, 22);

            Room oDirtPath3 = AddRoom("Dirt Path", "Dirt Path");
            AddBidirectionalExits(oDirtPath3, oDirtPath2, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oDirtPath3] = new PointF(8, 22);

            Room oDirtPath4 = AddRoom("Headless Horseman", "Dirt Path");
            AddPermanentMobs(oDirtPath4, MobTypeEnum.HeadlessHorseman);
            AddExit(oDirtPath2, oDirtPath4, "north");
            mithlondGraph.Rooms[oDirtPath4] = new PointF(9, 21.5F);

            Room oThundarrWay1 = AddRoom("Thundarr Way", "Thundarr Way");
            AddExit(oDirtPath4, oThundarrWay1, "east");
            mithlondGraph.Rooms[oThundarrWay1] = new PointF(10, 21);

            Room oThundarrWay2 = AddRoom("Thundarr Way", "Thundarr Way");
            AddBidirectionalExits(oThundarrWay1, oThundarrWay2, BidirectionalExitType.WestEast);
            AddExit(oForest, oThundarrWay2, "northwest");
            mithlondGraph.Rooms[oThundarrWay2] = new PointF(11, 21);

            Room oThundarrWay3 = AddRoom("Thundarr Way", "Thundarr Way");
            AddBidirectionalExits(oThundarrWay2, oThundarrWay3, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oThundarrWay3] = new PointF(12, 21);
            mithlondGraph.Rooms[oValleyRoad] = new PointF(13, 21);
            AddBidirectionalExits(oThundarrWay3, oValleyRoad, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oThundarrWay3] = new PointF(1, 2);
            AddMapBoundaryPoint(oThundarrWay3, oValleyRoad, MapType.Mithlond, MapType.WestOfBree);

            Room oDirtPath5 = AddRoom("Dirt Path", "Dirt Path");
            AddBidirectionalExits(westernRoadWestOfHobbiton, oDirtPath5, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDirtPath5, oThundarrWay1, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oDirtPath5] = new PointF(10, 20);
            mithlondGraph.Rooms[westernRoadWestOfHobbiton] = new PointF(10, 19);
            westOfBreeMap.Rooms[oDirtPath5] = new PointF(0, 1);
            AddExit(oDirtPath5, oThundarrWay2, "west");
            AddMapBoundaryPoint(westernRoadWestOfHobbiton, oDirtPath5, MapType.WestOfBree, MapType.Mithlond);

            Room oDirtPath6 = AddRoom("Dirt Path", "Dirt Path");
            AddBidirectionalExits(oThundarrWay1, oDirtPath6, BidirectionalExitType.NorthSouth);
            AddExit(oDirtPath6, oThundarrWay1, "south");
            mithlondGraph.Rooms[oDirtPath6] = new PointF(10, 22.5F);

            Room oDirtPath7 = AddRoom("Dirt Path", "Dirt Path");
            AddExit(oDirtPath4, oDirtPath7, "north");
            AddExit(oDirtPath7, oThundarrWay1, "north");
            mithlondGraph.Rooms[oDirtPath7] = new PointF(9, 21);

            Room oDirtPath9 = AddRoom("Dirt Path", "Dirt Path");
            AddExit(oDirtPath4, oDirtPath9, "west");
            AddExit(oDirtPath7, oDirtPath9, "west");
            AddExit(oDirtPath9, oThundarrWay1, "south");
            AddExit(oDirtPath9, oDirtPath, "west");
            mithlondGraph.Rooms[oDirtPath9] = new PointF(8, 21.25F);

            Room oDirtPath10 = AddRoom("Dirt Path", "Dirt Path");
            AddExit(oDirtPath9, oDirtPath10, "east");
            AddExit(oDirtPath10, oDirtPath7, "east");
            AddExit(oDirtPath6, oDirtPath10, "east");
            AddExit(oDirtPath7, oDirtPath10, "east");
            AddExit(oDirtPath4, oDirtPath10, "south");
            AddExit(oDirtPath10, oThundarrWay1, "north");
            mithlondGraph.Rooms[oDirtPath10] = new PointF(11, 22);

            Room oDirtPath11 = AddRoom("Dirt Path", "Dirt Path");
            AddBidirectionalExits(oDirtPath11, oDirtPath10, BidirectionalExitType.WestEast);
            AddExit(oDirtPath11, oDirtPath10, "west");
            AddExit(oDirtPath3, oDirtPath11, "west");
            AddExit(oDirtPath11, oThundarrWay1, "north");
            mithlondGraph.Rooms[oDirtPath11] = new PointF(10, 22);

            Room oDirtPath12 = AddRoom("Dirt Path", "Dirt Path");
            AddExit(oDirtPath9, oDirtPath12, "north");
            AddExit(oDirtPath12, oThundarrWay1, "north");
            AddExit(oDirtPath12, oDirtPath10, "west");
            AddExit(oDirtPath12, oDirtPath3, "south");
            mithlondGraph.Rooms[oDirtPath12] = new PointF(8, 20.25F);

            Room oDirtPath13 = AddRoom("Dirt Path", "Dirt Path");
            AddExit(oThundarrWay1, oDirtPath13, "west");
            AddExit(oDirtPath3, oDirtPath13, "north");
            AddExit(oDirtPath11, oDirtPath13, "south");
            AddExit(oDirtPath13, oDirtPath7, "west");
            AddBidirectionalExits(oDirtPath10, oDirtPath13, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oDirtPath13] = new PointF(11, 23);

            Room oDirtPath14 = AddRoom("Dirt Path", "Dirt Path");
            AddBidirectionalExits(oDirtPath13, oDirtPath14, BidirectionalExitType.WestEast);
            AddExit(oDirtPath14, oThundarrWay1, "north");
            AddExit(oDirtPath3, oDirtPath14, "south");
            AddExit(oDirtPath14, oDirtPath4, "south");
            AddExit(oDirtPath6, oDirtPath14, "west");
            AddExit(oDirtPath12, oDirtPath14, "east");
            mithlondGraph.Rooms[oDirtPath14] = new PointF(12, 23);

            Room oDirtPath15 = AddRoom("Dirt Path", "Dirt Path");
            AddExit(oDirtPath14, oDirtPath15, "east");
            AddExit(oDirtPath15, oDirtPath14, "east");
            AddExit(oDirtPath15, oThundarrWay1, "west");
            AddExit(oDirtPath7, oDirtPath15, "south");
            AddExit(oDirtPath15, oDirtPath10, "south");
            AddExit(oDirtPath13, oDirtPath15, "south");
            AddExit(oDirtPath15, oDirtPath12, "north");
            mithlondGraph.Rooms[oDirtPath15] = new PointF(13, 23);

            Room oWestronRoad2 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad1, oWestronRoad2, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWestronRoad2] = new PointF(5, 14);

            Room oWestronRoad3 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad2, oWestronRoad3, BidirectionalExitType.SoutheastNorthwest);
            AddPermanentMobs(oWestronRoad3, MobTypeEnum.BarbarianRaider, MobTypeEnum.BarbarianRaider, MobTypeEnum.BarbarianRaider);
            mithlondGraph.Rooms[oWestronRoad3] = new PointF(6, 14.5F);

            Room oWestronRoad4 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad3, oWestronRoad4, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWestronRoad4] = new PointF(7, 15);

            Room oWestronRoad5 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad4, oWestronRoad5, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWestronRoad5] = new PointF(8, 15.5F);

            Room oWestronRoad6 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad5, oWestronRoad6, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oWestronRoad6] = new PointF(9, 16);

            Room oWestronRoad7 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad6, oWestronRoad7, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWestronRoad7] = new PointF(9, 16.5F);

            Room oWestronRoad8 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad7, oWestronRoad8, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWestronRoad8] = new PointF(9, 17);

            Room oWestronRoad9 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad8, oWestronRoad9, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWestronRoad9] = new PointF(9, 17.5F);

            Room oWestronRoad10 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad9, oWestronRoad10, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWestronRoad10] = new PointF(9, 18);

            Room oWestronRoad11 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad10, oWestronRoad11, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWestronRoad11] = new PointF(9, 18.5F);
            //CSRTODO: west

            Room oWestronRoad12 = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(oWestronRoad11, oWestronRoad12, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oWestronRoad12] = new PointF(9, 19);

            AddBidirectionalExits(oWestronRoad12, westronRoadToHobbiton, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[westronRoadToHobbiton] = new PointF(9, 19.5F);

            Room oEvendimTrail11 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail10, oEvendimTrail11, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail11] = new PointF(7, 6.5F);

            Room oEvendimTrail12 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail12, oEvendimTrail11, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oEvendimTrail12] = new PointF(8, 5.5F);

            Room oEvendimTrail13 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail13, oEvendimTrail12, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oEvendimTrail13] = new PointF(9, 4.5F);

            Room oEvendimTrail14 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail13, oEvendimTrail14, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail14] = new PointF(10, 4.5F);

            Room oEvendimTrail15 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail14, oEvendimTrail15, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail15] = new PointF(11, 4.5F);

            Room oEvendimTrail16 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail15, oEvendimTrail16, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail16] = new PointF(12, 4.5F);

            Room oEvendimTrail17 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail16, oEvendimTrail17, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail17] = new PointF(13, 4.5F);

            Room oEvendimTrail18 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail18, oEvendimTrail17, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oEvendimTrail18] = new PointF(14, 3.5F);

            Room oEvendimTrail19 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail19, oEvendimTrail18, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oEvendimTrail19] = new PointF(15, 2.5F);

            Room oEvendimTrail20 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail19, oEvendimTrail20, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail20] = new PointF(16, 2.5F);
            //CSRTODO: village

            Room oEvendimTrail21 = AddRoom("Trail", "Evendim Trail");
            AddBidirectionalExits(oEvendimTrail20, oEvendimTrail21, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEvendimTrail21] = new PointF(17, 2.5F);

            Room oEndOfTheEvendimTrail = AddRoom("Trail End", "End of the Evendim Trail");
            AddBidirectionalExits(oEndOfTheEvendimTrail, oEvendimTrail21, BidirectionalExitType.SouthwestNortheast);
            mithlondGraph.Rooms[oEndOfTheEvendimTrail] = new PointF(18, 1.5F);
            //CSRTODO: boulder

            Room oMithlondPort2 = AddRoom("Mithlond Port", "Mithlond Port");
            AddBidirectionalExits(oMithlondPort2, oMithlondPort, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oMithlondPort2] = new PointF(2, 3);

            Room oMusicianSchool = AddHealingRoom("Musician School", "Mithlond Musician School", HealingRoom.Mithlond);
            AddBidirectionalExits(oMithlondPort2, oMusicianSchool, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oMusicianSchool] = new PointF(3, 3);

            Room oMithlondPort3 = AddRoom("Mithlond Port", "Mithlond Port");
            AddBidirectionalExits(oMithlondPort3, oMithlondPort2, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oMithlondPort3] = new PointF(2, 2);

            Room oDarkAlley = AddRoom("Dark Alley", "Dark Alley");
            AddBidirectionalExits(oDarkAlley, oMithlondPort3, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oDarkAlley] = new PointF(1, 2);

            Room oDeadEnd = AddRoom("Dead End", "Dead End");
            AddBidirectionalExits(oDeadEnd, oDarkAlley, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oDeadEnd] = new PointF(1, 1.5F);

            Room oSharkey = AddPawnShoppeRoom("Sharkey", "Skarkey's Shippers", PawnShoppe.Mithlond);
            AddPermanentMobs(oSharkey, MobTypeEnum.Sharkey);
            e = AddExit(oDeadEnd, oSharkey, "west");
            e.Hidden = true;
            AddExit(oSharkey, oDeadEnd, "east");
            mithlondGraph.Rooms[oSharkey] = new PointF(0, 1.5F);

            Room oPicadilyAvenue = AddRoom("Picadily", "Picadily Avenue");
            AddBidirectionalExits(oMithlondPort3, oPicadilyAvenue, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oPicadilyAvenue] = new PointF(3, 2);

            Room oHosuan = AddRoom("Ho-suan", "The Opium Den of Chon-Loc");
            AddPermanentMobs(oHosuan, MobTypeEnum.HoSuanThePenniless);
            AddBidirectionalExitsWithOut(oPicadilyAvenue, oHosuan, "north");
            mithlondGraph.Rooms[oHosuan] = new PointF(3, 1);

            Room oMithlondGateInside = AddRoom("Gate Inside", "Mithlond Port");
            AddBidirectionalExits(oMithlondGateInside, oMithlondPort3, BidirectionalExitType.NorthSouth);
            mithlondGraph.Rooms[oMithlondGateInside] = new PointF(2, 1);

            Room oGrunkillCharters = AddRoom("Grunkill Charters", "Grunkill Charters");
            AddPermanentMobs(oGrunkillCharters, MobTypeEnum.Grunkill);
            AddBidirectionalExits(oGrunkillCharters, oMithlondGateInside, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oGrunkillCharters] = new PointF(1, 1);

            Room oGrunkillQuarters = AddRoom("Grunkill Quarters", "Grunkill's Quarters");
            AddBidirectionalSameNameExit(oGrunkillCharters, oGrunkillQuarters, "curtain");
            mithlondGraph.Rooms[oGrunkillQuarters] = new PointF(0, 1);

            Room oMithlondGateOutside = AddRoom("Gate Outside", "Ered Lune");
            AddBidirectionalSameNameExit(oMithlondGateInside, oMithlondGateOutside, "gate");
            mithlondGraph.Rooms[oMithlondGateOutside] = new PointF(2, 0);

            Room oEredLune1 = AddRoom("Ered Lune", "Ered Lune");
            AddBidirectionalExits(oMithlondGateOutside, oEredLune1, BidirectionalExitType.WestEast);
            mithlondGraph.Rooms[oEredLune1] = new PointF(3, 0);

            Room oEredLune2 = AddRoom("Ered Lune", "Ered Lune");
            AddBidirectionalExits(oEredLune1, oEredLune2, BidirectionalExitType.SoutheastNorthwest);
            mithlondGraph.Rooms[oEredLune2] = new PointF(4, 1);

            Room oEredLune3 = AddRoom("Ered Lune", "Ered Lune");
            e = AddExit(oEredLune2, oEredLune3, "up");
            e.IsTrapExit = true;
            e = AddExit(oEredLune3, oEredLune2, "down");
            e.IsTrapExit = true;
            mithlondGraph.Rooms[oEredLune3] = new PointF(4, 0);

            Room oCliff = AddRoom("Cliff", "Cliff");
            e = AddExit(oEredLune3, oCliff, "up");
            e.IsTrapExit = true;
            AddExit(oCliff, oEredLune3, "down");
            mithlondGraph.Rooms[oCliff] = new PointF(4, -1);

            Room oCliff2 = AddRoom("Cliff", "Cliff");
            AddExit(oCliff, oCliff2, "up");
            e = AddExit(oCliff2, oCliff, "down");
            e.IsTrapExit = true;
            mithlondGraph.Rooms[oCliff2] = new PointF(4, -2);

            AddHarbringer(oHarbringerGangplank, tharbadDocks);
            AddBullroarer(oBullroarerSlip, nindamosDocks);
            AddCelduinExpress(boatswain, breeDocks);
            AddOmaniPrincess(oOmaniPrincessSlip);
        }

        private void AddOmaniPrincess(Room oOmaniPrincessSlip)
        {
            RoomGraph shipsGraph = _graphs[MapType.Ships];
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];

            Room oStem = AddRoom("Stem", "Stem of the Omani Princess");
            shipsGraph.Rooms[oStem] = new PointF(-0.5F, 5.5F);

            Room oMainDeck1 = AddRoom("Main Deck", "Main Deck of the Omani Princess");
            oMainDeck1.BoatLocationType = BoatEmbarkOrDisembark.OmaniPrincessMithlondBoat;
            AddExit(oMainDeck1, oStem, "bow");
            AddExit(oStem, oMainDeck1, "stern");
            Exit e = AddExit(oOmaniPrincessSlip, oMainDeck1, "dhow");
            e.BoatExitType = BoatExitType.MithlondEnterOmaniPrincess;
            e = AddExit(oMainDeck1, oOmaniPrincessSlip, "gangway");
            e.BoatExitType = BoatExitType.MithlondExitOmaniPrincess;
            mithlondGraph.Rooms[oMainDeck1] = new PointF(1, 7);
            shipsGraph.Rooms[oMainDeck1] = new PointF(0.5F, 6.5F);
            AddMapBoundaryPoint(oOmaniPrincessSlip, oMainDeck1, MapType.Mithlond, MapType.Ships);

            Room oMainDeck2 = AddRoom("Main Deck", "Maindeck of the Omani Princess");
            oMainDeck2.BoatLocationType = BoatEmbarkOrDisembark.OmaniPrincessUmbarBoat;
            AddExit(oMainDeck2, oStem, "bow");
            AddExit(oMainDeck1, oMainDeck2, "starboard");
            AddExit(oMainDeck2, oMainDeck1, "portside");
            shipsGraph.Rooms[oMainDeck2] = new PointF(-1.5F, 6.5F);

            Room oForwardCabin = AddRoom("Forward Cabin", "Forward Cabin of the Omani Princess");
            AddBidirectionalSameNameExit(oMainDeck1, oForwardCabin, "door");
            shipsGraph.Rooms[oForwardCabin] = new PointF(-0.5F, 6);

            Room oMidship = AddRoom("Midship", "Midship of the Omani Princess");
            AddExit(oMainDeck1, oMidship, "stern");
            AddExit(oMainDeck2, oMidship, "stern");
            AddExit(oMidship, oMainDeck1, "bow");
            shipsGraph.Rooms[oMidship] = new PointF(-0.5F, 7);

            Room oMainCabin = AddRoom("Main Cabin", "Main Cabin of the Omani Princess");
            AddBidirectionalExitsWithOut(oMidship, oMainCabin, "door");
            shipsGraph.Rooms[oMainCabin] = new PointF(-0.5F, 6.75F);

            Room oCargoHold = AddRoom("Cargo Hold", "Cargo Hold of the Omani Princess");
            AddBidirectionalSameNameExit(oMidship, oCargoHold, "hatch");
            shipsGraph.Rooms[oCargoHold] = new PointF(-0.5F, 8);

            Room oCrowsNest = AddRoom("Crow's Nest", "Crow's Nest of the Omani Princess");
            AddBidirectionalExits(oCrowsNest, oMidship, BidirectionalExitType.UpDown);
            shipsGraph.Rooms[oCrowsNest] = new PointF(-1.5F, 8);

            Room oGangway = AddRoom("Gangway", "Gangway of the Omani Princess");
            AddExit(oMidship, oGangway, "gangway");
            AddExit(oGangway, oMidship, "down");
            shipsGraph.Rooms[oGangway] = new PointF(0.5F, 8);

            Room oCaptainBelfalas = AddRoom("Captain Belfalas", "Transom Deck of the Omani Princess");
            AddExit(oGangway, oCaptainBelfalas, "up");
            AddExit(oCaptainBelfalas, oGangway, "gangway");
            shipsGraph.Rooms[oCaptainBelfalas] = new PointF(0.5F, 7.5F);

            Room oUmbarPort = AddRoom("Umbar Port", "Umbar Port");
            oUmbarPort.BoatLocationType = BoatEmbarkOrDisembark.OmaniPrincessUmbarDock;
            e = AddExit(oMainDeck2, oUmbarPort, "gangway");
            e.BoatExitType = BoatExitType.UmbarExitOmaniPrincess;
            e = AddExit(oUmbarPort, oMainDeck2, "dhow");
            e.BoatExitType = BoatExitType.UmbarEnterOmaniPrincess;
            shipsGraph.Rooms[oUmbarPort] = new PointF(-2.5F, 6.5F);
        }

        private void AddCelduinExpress(Room boatswain, Room breeDocks)
        {
            RoomGraph shipsGraph = _graphs[MapType.Ships];
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];

            mithlondGraph.Rooms[boatswain] = new PointF(1, 5);
            shipsGraph.Rooms[boatswain] = new PointF(1, 3);
            shipsGraph.Rooms[breeDocks] = new PointF(1, 2.5F);

            Room oBeneathBridge = AddRoom("Under Bridge", "Beneath the Bridge of the Celduin Express");
            shipsGraph.Rooms[oBeneathBridge] = new PointF(-0.5F, 3.5F);
            AddBidirectionalExits(boatswain, oBeneathBridge, BidirectionalExitType.SouthwestNortheast);

            Room oBridge = AddRoom("Capt. Felagund", "Bridge of the Celduin Express");
            AddPermanentMobs(oBridge, MobTypeEnum.CaptainFelagund);
            Exit e = AddExit(oBeneathBridge, oBridge, "hatchway");
            e.KeyType = SupportedKeysFlags.BridgeKey;
            e.MustOpen = true;
            e = AddExit(oBridge, oBeneathBridge, "down");
            e.MustOpen = true;
            shipsGraph.Rooms[oBridge] = new PointF(-0.5F, 3.25F);

            Room oUnderDeck = AddRoom("Under Deck", "Beneath the Deck of the Celduin Express");
            AddBidirectionalSameNameExit(oBeneathBridge, oUnderDeck, "stairway");
            shipsGraph.Rooms[oUnderDeck] = new PointF(-1, 4.5F);

            Room oPassengerLounge = AddRoom("Passenger Lounge", "Passenger Lounge");
            AddBidirectionalExits(oUnderDeck, oPassengerLounge, BidirectionalExitType.NorthSouth);
            shipsGraph.Rooms[oPassengerLounge] = new PointF(-1, 4.75F);

            Room oBoilerRoom = AddRoom("Boiler Room", "Boiler Room");
            e = AddExit(oUnderDeck, oBoilerRoom, "door");
            e.KeyType = SupportedKeysFlags.BoilerKey;
            e.MustOpen = true;
            e = AddExit(oBoilerRoom, oUnderDeck, "door");
            e.MustOpen = true;
            shipsGraph.Rooms[oBoilerRoom] = new PointF(0, 4.5F);

            Room oCelduinExpressNW = AddRoom("Stern", "Stern of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressNW, boatswain, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCelduinExpressNW, oBeneathBridge, BidirectionalExitType.SoutheastNorthwest);
            shipsGraph.Rooms[oCelduinExpressNW] = new PointF(-2, 3);

            Room oCelduinExpressMainDeckW = AddRoom("Main Deck", "Main Deck of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressNW, oCelduinExpressMainDeckW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBeneathBridge, oCelduinExpressMainDeckW, BidirectionalExitType.SouthwestNortheast);
            shipsGraph.Rooms[oCelduinExpressMainDeckW] = new PointF(-2, 5);

            Room oCelduinExpressMainDeckE = AddRoom("Main Deck", "Main Deck of the Celduin Express");
            AddBidirectionalExits(boatswain, oCelduinExpressMainDeckE, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBeneathBridge, oCelduinExpressMainDeckE, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oCelduinExpressMainDeckW, oCelduinExpressMainDeckE, BidirectionalExitType.WestEast);
            shipsGraph.Rooms[oCelduinExpressMainDeckE] = new PointF(1, 5);

            Room oCelduinExpressBow = AddRoom("Bow", "Bow of the Celduin Express");
            AddBidirectionalExits(oCelduinExpressMainDeckE, oCelduinExpressBow, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oCelduinExpressMainDeckW, oCelduinExpressBow, BidirectionalExitType.SoutheastNorthwest);
            shipsGraph.Rooms[oCelduinExpressBow] = new PointF(-0.5F, 5.25F);
        }

        /// <summary>
        /// harbringer allows travel from Tharbad to Mithlond (but not the reverse?)
        /// </summary>
        private void AddHarbringer(Room mithlondEntrance, Room tharbadDocks)
        {
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];
            RoomGraph tharbadGraph = _graphs[MapType.Tharbad];
            RoomGraph shipsGraph = _graphs[MapType.Ships];

            Room oHarbringerTop = AddRoom("Bluejacket", "Bow of the Harbringer.");
            AddPermanentMobs(oHarbringerTop, MobTypeEnum.Bluejacket, MobTypeEnum.Scallywag);
            shipsGraph.Rooms[oHarbringerTop] = new PointF(4.5F, 5.5F);

            Room oHarbringerWest1 = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            AddBidirectionalExits(oHarbringerTop, oHarbringerWest1, BidirectionalExitType.SouthwestNortheast);
            shipsGraph.Rooms[oHarbringerWest1] = new PointF(4, 6);

            Room oHarbringerEast1 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerTop, oHarbringerEast1, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oHarbringerWest1, oHarbringerEast1, BidirectionalExitType.WestEast);
            shipsGraph.Rooms[oHarbringerEast1] = new PointF(5, 6);

            Room oHarbringerMithlondEntrance = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            oHarbringerMithlondEntrance.BoatLocationType = BoatEmbarkOrDisembark.Harbringer;
            AddBidirectionalExits(oHarbringerWest1, oHarbringerMithlondEntrance, BidirectionalExitType.NorthSouth);
            Exit e = AddExit(mithlondEntrance, oHarbringerMithlondEntrance, "ship");
            e.BoatExitType = BoatExitType.MithlondEnterHarbringer;
            e = AddExit(oHarbringerMithlondEntrance, mithlondEntrance, "gangplank");
            e.BoatExitType = BoatExitType.MithlondExitHarbringer;
            e = AddExit(tharbadDocks, oHarbringerMithlondEntrance, "gangway");
            e.BoatExitType = BoatExitType.TharbadEnterHarbringer;
            mithlondGraph.Rooms[oHarbringerMithlondEntrance] = new PointF(0, 8);
            shipsGraph.Rooms[oHarbringerMithlondEntrance] = new PointF(4, 6.5F);
            shipsGraph.Rooms[tharbadDocks] = new PointF(3, 6.5F);
            tharbadGraph.Rooms[oHarbringerMithlondEntrance] = new PointF(0, 9);
            AddMapBoundaryPoint(tharbadDocks, oHarbringerMithlondEntrance, MapType.Tharbad, MapType.Ships);
            AddMapBoundaryPoint(mithlondEntrance, oHarbringerMithlondEntrance, MapType.Mithlond, MapType.Ships);

            Room oHarbringerEast2 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerMithlondEntrance, oHarbringerEast2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oHarbringerEast1, oHarbringerEast2, BidirectionalExitType.NorthSouth);
            shipsGraph.Rooms[oHarbringerEast2] = new PointF(5, 6.5F);

            Room oHarbringerWest3 = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            AddBidirectionalExits(oHarbringerMithlondEntrance, oHarbringerWest3, BidirectionalExitType.NorthSouth);
            shipsGraph.Rooms[oHarbringerWest3] = new PointF(4, 7);

            Room oHarbringerEast3 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerEast2, oHarbringerEast3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oHarbringerWest3, oHarbringerEast3, BidirectionalExitType.WestEast);
            shipsGraph.Rooms[oHarbringerEast3] = new PointF(5, 7);

            Room oHarbringerWest4 = AddRoom("Harbringer", "Starboad-side of the Harbringer");
            AddBidirectionalExits(oHarbringerWest3, oHarbringerWest4, BidirectionalExitType.NorthSouth);
            shipsGraph.Rooms[oHarbringerWest4] = new PointF(4, 7.5F);

            Room oHarbringerEast4 = AddRoom("Harbringer", "Port-side of the Harbringer");
            AddBidirectionalExits(oHarbringerEast3, oHarbringerEast4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oHarbringerWest4, oHarbringerEast4, BidirectionalExitType.WestEast);
            shipsGraph.Rooms[oHarbringerEast4] = new PointF(5, 7.5F);

            Room oKralle = AddRoom("Kralle", "Stern of the Harbringer");
            AddPermanentMobs(oKralle, MobTypeEnum.Kralle);
            AddBidirectionalExits(oHarbringerWest4, oKralle, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oHarbringerEast4, oKralle, BidirectionalExitType.SouthwestNortheast);
            shipsGraph.Rooms[oKralle] = new PointF(4.5F, 8);

            Room oShipsHoldSE = AddRoom("Ship's Hold", "Ship's Hold");
            AddExit(oHarbringerEast3, oShipsHoldSE, "hatch");
            AddExit(oShipsHoldSE, oHarbringerEast3, "up");
            shipsGraph.Rooms[oShipsHoldSE] = new PointF(7, 7.5F);

            Room oShipsHoldS = AddRoom("Ship's Hold", "Ship's Hold");
            AddBidirectionalExits(oShipsHoldSE, oShipsHoldS, BidirectionalExitType.SouthwestNortheast);
            shipsGraph.Rooms[oShipsHoldS] = new PointF(6.5F, 8);

            Room oShipsHoldSW = AddRoom("Ship's Hold", "Ship's Hold");
            AddBidirectionalExits(oShipsHoldSW, oShipsHoldS, BidirectionalExitType.SoutheastNorthwest);
            shipsGraph.Rooms[oShipsHoldSW] = new PointF(6, 7.5F);

            Room oBrig = AddRoom("Brig", "Brig");
            AddPermanentMobs(oBrig, MobTypeEnum.Smee);
            AddExit(oShipsHoldSW, oBrig, "door");
            shipsGraph.Rooms[oBrig] = new PointF(6, 6.5F);

            Room oShipsHoldNE = AddRoom("Ship's Hold", "Ship's Hold");
            AddBidirectionalExits(oShipsHoldNE, oShipsHoldSE, BidirectionalExitType.NorthSouth);
            shipsGraph.Rooms[oShipsHoldNE] = new PointF(7, 7);

            Room oRandsQuarters = AddRoom("Rand's Quarters", "Rand's Quarters");
            AddBidirectionalSameNameMustOpenExit(oShipsHoldNE, oRandsQuarters, "door");
            shipsGraph.Rooms[oRandsQuarters] = new PointF(7, 6.5F);
        }

        private void AddBullroarer(Room mithlondEntrance, Room nindamosDocks)
        {
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];
            RoomGraph nindamosGraph = _graphs[MapType.Nindamos];
            RoomGraph shipsGraph = _graphs[MapType.Ships];

            Room bullroarerSE = AddRoom("Bullroarer", "Deck of the Bullroarer");
            bullroarerSE.BoatLocationType = BoatEmbarkOrDisembark.Bullroarer;
            Exit e = AddExit(mithlondEntrance, bullroarerSE, "gangway");
            e.BoatExitType = BoatExitType.MithlondEnterBullroarer;
            e = AddExit(nindamosDocks, bullroarerSE, "gangway");
            e.BoatExitType = BoatExitType.NindamosEnterBullroarer;
            e = AddExit(bullroarerSE, mithlondEntrance, "plank");
            e.BoatExitType = BoatExitType.MithlondExitBullroarer;
            e = AddExit(bullroarerSE, nindamosDocks, "plank");
            e.BoatExitType = BoatExitType.NindamosExitBullroarer;
            nindamosGraph.Rooms[bullroarerSE] = new PointF(15, 6);
            mithlondGraph.Rooms[bullroarerSE] = new PointF(1, 6);
            shipsGraph.Rooms[bullroarerSE] = new PointF(5, 5);
            shipsGraph.Rooms[nindamosDocks] = new PointF(6, 5);
            AddMapBoundaryPoint(nindamosDocks, bullroarerSE, MapType.Nindamos, MapType.Ships);
            AddMapBoundaryPoint(mithlondEntrance, bullroarerSE, MapType.Mithlond, MapType.Ships);

            Room bullroarerSW = AddRoom("Bullroarer", "Covered Deck");
            AddBidirectionalExits(bullroarerSW, bullroarerSE, BidirectionalExitType.WestEast);
            shipsGraph.Rooms[bullroarerSW] = new PointF(4, 5);

            Room bullroarerNW = AddRoom("Bullroarer", "Fish Landing Deck");
            AddBidirectionalExits(bullroarerNW, bullroarerSW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(bullroarerNW, bullroarerSE, BidirectionalExitType.SoutheastNorthwest);
            shipsGraph.Rooms[bullroarerNW] = new PointF(4, 4.5F);

            Room bullroarerNE = AddRoom("Bullroarer", "Fish Cleaning Deck");
            AddBidirectionalExits(bullroarerNW, bullroarerNE, BidirectionalExitType.WestEast);
            AddBidirectionalExits(bullroarerNE, bullroarerSE, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(bullroarerNE, bullroarerSW, BidirectionalExitType.SouthwestNortheast);
            shipsGraph.Rooms[bullroarerNE] = new PointF(5, 4.5F);

            Room wheelhouse = AddRoom("Wheelhouse", "The Bullroarers Wheelhouse");
            AddExit(bullroarerNE, wheelhouse, "wheelhouse");
            AddExit(bullroarerNW, wheelhouse, "wheelhouse");
            AddExit(wheelhouse, bullroarerNE, "out");
            shipsGraph.Rooms[wheelhouse] = new PointF(4.5F, 4);

            Room cargoHold = AddRoom("Cargo Hold", "Cargo Hold");
            AddBidirectionalSameNameExit(wheelhouse, cargoHold, "stairs");
            shipsGraph.Rooms[cargoHold] = new PointF(4.5F, 3.5F);

            Room fishHold = AddRoom("Fish Hold", "Fish Hold");
            fishHold.DamageType = RoomDamageType.Wind;
            Room brentDiehard = AddRoom("Brent Diehard", "Engine Room");
            AddPermanentMobs(brentDiehard, MobTypeEnum.BrentDiehard);
            shipsGraph.Rooms[fishHold] = new PointF(4.5F, 3);
            shipsGraph.Rooms[brentDiehard] = new PointF(4.5F, 2.5F);
            AddBidirectionalSameNameMustOpenExit(fishHold, brentDiehard, "hatchway");
            AddBidirectionalSameNameMustOpenExit(cargoHold, fishHold, "hatch");
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

            tharbadGraph.Rooms[oTharbadGateOutside] = new PointF(3, 0.5F);

            Room balleNightingale = AddRoom("Balle/Nightingale", "Nightingale Ave./Balle St.");
            Room balle1 = AddRoom("Balle", "Balle Street");
            AddBidirectionalExits(balleNightingale, balle1, BidirectionalExitType.WestEast);
            AddBidirectionalSameNameExit(oTharbadGateOutside, balleNightingale, "gate");
            tharbadGraph.Rooms[balleNightingale] = new PointF(3, 1);
            tharbadGraph.Rooms[balle1] = new PointF(4, 1);

            Room balleIllusion = AddRoom("Balle/Illusion", "Balle Street/Illusion Road");
            AddBidirectionalExits(balle1, balleIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balleIllusion] = new PointF(5, 1);

            Room balle2 = AddRoom("Balle", "Balle Street");
            AddBidirectionalExits(balleIllusion, balle2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balle2] = new PointF(8, 1);

            Room balleEvard = AddRoom("Balle/Evard", "Balle St./Evard Ave.");
            AddBidirectionalExits(balle2, balleEvard, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balleEvard] = new PointF(10, 1);

            Room evardMemorialTree = AddRoom("Evard Memorial Tree", "In the arms of the Evard Memorial Tree");
            Exit e = AddExit(balleEvard, evardMemorialTree, "tree");
            e.Hidden = true;
            AddExit(evardMemorialTree, balleEvard, "down");
            tharbadGraph.Rooms[evardMemorialTree] = new PointF(10, 0);

            Room evard1 = AddRoom("Evard", "Evard Avenue");
            AddBidirectionalExits(balleEvard, evard1, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[evard1] = new PointF(10, 3);

            Room evard2 = AddRoom("Evard", "Evard Avenue");
            AddBidirectionalExits(evard1, evard2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[evard2] = new PointF(10, 7);

            Room alley = AddRoom("Alley", "Alley");
            AddBidirectionalExits(alley, evard2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[alley] = new PointF(8, 7);

            Room sabreEvard = AddRoom("Sabre/Evard", "Sabre Street/Evard Avenue");
            AddBidirectionalExits(evard2, sabreEvard, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[sabreEvard] = new PointF(10, 8);
            eastOfTharbadGraph.Rooms[sabreEvard] = new PointF(-1, 4);

            Room sabre1 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre1, sabreEvard, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre1] = new PointF(8, 8);

            Room sabreIllusion = AddRoom("Sabre/Illusion", "Sabre Street/Illusion Road");
            AddBidirectionalExits(sabreIllusion, sabre1, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabreIllusion] = new PointF(5, 8);

            Room sabre2 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre2, sabreIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre2] = new PointF(4, 8);

            Room sabreNightingale = AddRoom("Sabre/Nightingale", "Nightingale Ave./Sabre Street");
            AddBidirectionalExits(sabreNightingale, sabre2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabreNightingale] = new PointF(3, 8);

            Room nightingale1 = AddRoom("Nightingale", "Nightingale Avenue");
            AddBidirectionalExits(nightingale1, sabreNightingale, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale1] = new PointF(3, 5);

            Room efrimsJuiceCafe = AddRoom("Efrim's Juice Cafe", "Efrim's Juice Cafe");
            AddBidirectionalSameNameExit(nightingale1, efrimsJuiceCafe, "door");
            tharbadGraph.Rooms[efrimsJuiceCafe] = new PointF(4, 5);

            Room nightingale2 = AddRoom("Nightingale", "Nightingale Avenue");
            AddBidirectionalExits(nightingale2, nightingale1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(balleNightingale, nightingale2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale2] = new PointF(3, 3);

            Room jewelryShop = AddRoom("Jewelry Shop", "Jewelry Shop");
            AddBidirectionalExits(nightingale2, jewelryShop, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[jewelryShop] = new PointF(4, 3);

            Room nightingale3 = AddRoom("Nightingale", "Nightingale Avenue");
            AddBidirectionalExits(sabreNightingale, nightingale3, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale3] = new PointF(3, 9);

            Room illusion2 = AddRoom("Illusion", "Illusion Road");
            AddBidirectionalExits(balleIllusion, illusion2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[illusion2] = new PointF(5, 3);

            Room marketBeast = AddRoom("Market Beast", "Market District - Beast Sellers");
            AddBidirectionalExits(illusion2, marketBeast, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketBeast] = new PointF(5, 5);

            Room bardicGuildhall = AddHealingRoom("Bardic Guildhall", "Bardic Guildhall", HealingRoom.Tharbad);
            AddBidirectionalExits(bardicGuildhall, nightingale3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[bardicGuildhall] = new PointF(2, 9);

            Room oGuildmasterAnsette = AddRoom("Ansette", "Guildmaster's Office");
            AddPermanentMobs(oGuildmasterAnsette, MobTypeEnum.GuildmasterAnsette, MobTypeEnum.PrucillaTheGroupie);
            e = AddBidirectionalExitsWithOut(bardicGuildhall, oGuildmasterAnsette, "door");
            e.Hidden = true;
            e.RequiresDay = true;
            tharbadGraph.Rooms[oGuildmasterAnsette] = new PointF(2, 8.5F);

            Room sabre3 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre3, sabreNightingale, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre3] = new PointF(2, 8);

            Room sabre4 = AddRoom("Sabre", "Sabre Street");
            AddBidirectionalExits(sabre4, sabre3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre4] = new PointF(1, 8);

            Room tourismAndCustoms = AddRoom("Tourism and Customs", "Tourism and Customs");
            AddBidirectionalExits(tourismAndCustoms, sabre4, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[tourismAndCustoms] = new PointF(1, 7);

            Room tharbadWestGateInside = AddRoom("West Gate Inside", "Sabre Street");
            AddBidirectionalExits(tharbadWestGateInside, sabre4, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[tharbadWestGateInside] = new PointF(0, 8);

            tharbadWestGateOutside = AddRoom("West Gate Outside", "West Gate of Tharbad");
            AddBidirectionalSameNameExit(tharbadWestGateInside, tharbadWestGateOutside, "gate");
            tharbadGraph.Rooms[tharbadWestGateOutside] = new PointF(-1, 8);

            tharbadDocks = AddRoom("Docks", "Tharbad Docks");
            tharbadDocks.BoatLocationType = BoatEmbarkOrDisembark.HarbringerTharbad;
            AddBidirectionalExits(tharbadWestGateOutside, tharbadDocks, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[tharbadDocks] = new PointF(-1, 9);

            Room illusion1 = AddRoom("Illusion", "Illusion Road");
            AddBidirectionalExits(illusion1, sabreIllusion, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[illusion1] = new PointF(5, 7);

            Room marketDistrictClothiers = AddRoom("Market Clothiers", "Market District - Clothiers");
            AddBidirectionalExits(marketDistrictClothiers, illusion1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(marketBeast, marketDistrictClothiers, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketDistrictClothiers] = new PointF(5, 6);

            Room oMasterJeweler = AddRoom("Jeweler", "Market District - Precious Gems");
            AddPermanentMobs(oMasterJeweler, MobTypeEnum.MasterJeweler, MobTypeEnum.ElvenGuard);
            AddBidirectionalExits(marketDistrictClothiers, oMasterJeweler, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oMasterJeweler] = new PointF(6, 6);

            Room marketTrinkets = AddPawnShoppeRoom("Market Trinkets", "Market District - Trinkets and Baubles", PawnShoppe.Tharbad);
            AddBidirectionalExits(marketBeast, marketTrinkets, BidirectionalExitType.WestEast);
            AddBidirectionalExits(marketTrinkets, oMasterJeweler, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketTrinkets] = new PointF(6, 5);

            Room oEntranceToGypsyEncampment = AddRoom("Gypsy Encampment", "Entrance to Gypsy Encampment");
            e = AddExit(oMasterJeweler, oEntranceToGypsyEncampment, "row");
            e.MaximumLevel = 13;
            AddExit(oEntranceToGypsyEncampment, oMasterJeweler, "market");
            tharbadGraph.Rooms[oEntranceToGypsyEncampment] = new PointF(7, 6);

            Room oGypsyRow1 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oEntranceToGypsyEncampment, oGypsyRow1, BidirectionalExitType.WestEast);
            e = AddExit(alley, oGypsyRow1, "north");
            e.Hidden = true;
            e.MaximumLevel = 13;
            e = AddExit(oGypsyRow1, alley, "south");
            e.Hidden = true;
            tharbadGraph.Rooms[oGypsyRow1] = new PointF(8, 6);

            Room oGypsyRow2 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oGypsyRow1, oGypsyRow2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oGypsyRow2] = new PointF(9, 6);

            Room oGypsyRow3 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oGypsyRow3, oGypsyRow2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oGypsyRow3] = new PointF(9, 5);

            Room oGypsyRow4 = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(oGypsyRow4, oGypsyRow3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oGypsyRow4] = new PointF(8, 5);

            Room oKingBrundensWagon = AddRoom("King Wagon", "King's Wagon");
            AddBidirectionalExitsWithOut(oGypsyRow4, oKingBrundensWagon, "wagon");
            tharbadGraph.Rooms[oKingBrundensWagon] = new PointF(8, 4);

            Room oKingBrunden = AddRoom("King Brunden", "Gypsy King's Lounge");
            AddPermanentMobs(oKingBrunden, MobTypeEnum.KingBrunden);
            AddBidirectionalExitsWithOut(oKingBrundensWagon, oKingBrunden, "back");
            tharbadGraph.Rooms[oKingBrunden] = new PointF(8, 3);

            Room oGypsyBlademaster = AddRoom("Blademaster", "Fighters' Tent");
            AddPermanentMobs(oGypsyBlademaster, MobTypeEnum.GypsyBlademaster);
            AddBidirectionalExitsWithOut(oGypsyRow3, oGypsyBlademaster, "tent");
            tharbadGraph.Rooms[oGypsyBlademaster] = new PointF(9, 4);

            Room oKingsMoneychanger = AddRoom("Moneychanger", "Gypsy Moneychanger");
            AddPermanentMobs(oKingsMoneychanger, MobTypeEnum.KingsMoneychanger);
            AddBidirectionalExitsWithOut(oGypsyRow2, oKingsMoneychanger, "tent");
            tharbadGraph.Rooms[oKingsMoneychanger] = new PointF(9, 6.5F);

            Room oMadameNicolov = AddRoom("Nicolov", "Madame Nicolov's Wagon");
            AddPermanentMobs(oMadameNicolov, MobTypeEnum.MadameNicolov);
            AddBidirectionalExitsWithOut(oGypsyRow1, oMadameNicolov, "wagon");
            tharbadGraph.Rooms[oMadameNicolov] = new PointF(8, 5.5F);

            Room gildedApple = AddRoom("Gilded Apple", "The Gilded Apple");
            AddBidirectionalSameNameExit(sabre3, gildedApple, "door");
            tharbadGraph.Rooms[gildedApple] = new PointF(2, 7.5F);

            Room zathriel = AddRoom("Zathriel the Minstrel", "Gilded Apple - Stage");
            AddPermanentMobs(zathriel, MobTypeEnum.ZathrielTheMinstrel);
            e = AddExit(gildedApple, zathriel, "stage");
            e.Hidden = true;
            AddExit(zathriel, gildedApple, "down");
            tharbadGraph.Rooms[zathriel] = new PointF(2, 7);

            Room oOliphauntsTattoos = AddRoom("Oliphaunt's Tattoos", "Oliphaunt's Tattoos");
            AddBidirectionalExits(balle2, oOliphauntsTattoos, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oOliphauntsTattoos] = new PointF(8, 1.5F);

            Room oOliphaunt = AddRoom("Oliphaunt", "Oliphaunt's Workroom");
            AddPermanentMobs(oOliphaunt, MobTypeEnum.OliphauntTheTattooArtist);
            AddBidirectionalSameNameExit(oOliphauntsTattoos, oOliphaunt, "curtain");
            tharbadGraph.Rooms[oOliphaunt] = new PointF(8, 2);

            Room oCrimsonBridge = AddRoom("Crimson Bridge", "Crimson Bridge");
            AddBidirectionalExits(sabre2, oCrimsonBridge, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oCrimsonBridge] = new PointF(4, 8.25F);

            Room oCrimsonTowerBase = AddRoom("Tower Base", "Tower Base");
            AddBidirectionalExits(oCrimsonBridge, oCrimsonTowerBase, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oCrimsonTowerBase] = new PointF(4, 8.5F);

            Room oClingingToIvy1 = AddRoom("Clinging to Ivy", "Clinging to Ivy");
            e = AddExit(oCrimsonTowerBase, oClingingToIvy1, "ivy");
            e.Hidden = true;
            AddExit(oClingingToIvy1, oCrimsonTowerBase, "down");
            tharbadGraph.Rooms[oClingingToIvy1] = new PointF(4, 8.75F);

            Room oClingingToIvy2 = AddRoom("Clinging to Ivy", "Clinging to Ivy");
            AddBidirectionalExits(oClingingToIvy2, oClingingToIvy1, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oClingingToIvy2] = new PointF(4, 9);

            Room oTopOfTheTower = AddRoom("Tower Top", "Top of the Tower");
            AddBidirectionalExits(oTopOfTheTower, oClingingToIvy2, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oTopOfTheTower] = new PointF(4, 9.25F);

            Room oPalaceGates = AddRoom("Palace Gates", "Gates of the Palace of Illusion");
            AddBidirectionalExits(sabreIllusion, oPalaceGates, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oPalaceGates] = new PointF(5, 9);

            Room oPalaceOfIllusion = AddRoom("Illusion Palace", "Palace of Illusion");
            AddBidirectionalSameNameExit(oPalaceGates, oPalaceOfIllusion, "gate");
            tharbadGraph.Rooms[oPalaceOfIllusion] = new PointF(4.5F, 12);

            Room oImperialKitchens = AddRoom("Imperial Kitchens", "Imperial Kitchens");
            AddBidirectionalExits(oImperialKitchens, oPalaceOfIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oImperialKitchens] = new PointF(3.5F, 12);

            Room oHallOfRainbows1 = AddRoom("Rainbow Hall", "Hall of Rainbows");
            AddBidirectionalExits(oHallOfRainbows1, oPalaceOfIllusion, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oHallOfRainbows1] = new PointF(4.5F, 11);

            Room oHallOfRainbows2 = AddRoom("Rainbow Hall", "Hall of Rainbows");
            AddBidirectionalExits(oHallOfRainbows1, oHallOfRainbows2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oHallOfRainbows2] = new PointF(5.5F, 11);

            Room oHallOfRainbows3 = AddRoom("Rainbow Hall", "Hall of Rainbows");
            AddBidirectionalExits(oHallOfRainbows2, oHallOfRainbows3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oHallOfRainbows3] = new PointF(6.5F, 11);

            Room oEmptyGuestRoom = AddRoom("Guest Room", "Empty Guest Room");
            AddBidirectionalSameNameExit(oHallOfRainbows3, oEmptyGuestRoom, "door");
            tharbadGraph.Rooms[oEmptyGuestRoom] = new PointF(6.5F, 12);

            Room oChancellorsDesk = AddRoom("Chancellor's Desk", "Chancellor's Desk");
            AddExit(oHallOfRainbows1, oChancellorsDesk, "arch");
            AddExit(oChancellorsDesk, oHallOfRainbows1, "east");
            tharbadGraph.Rooms[oChancellorsDesk] = new PointF(3.5F, 11);

            Room oMainAudienceChamber = AddRoom("Audience Chamber", "Main Audience Chamber");
            AddBidirectionalExits(oMainAudienceChamber, oChancellorsDesk, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oMainAudienceChamber] = new PointF(3.5F, 10);

            Room oCaptainRenton = AddRoom("Throne Room", "Throne Room");
            AddPermanentMobs(oCaptainRenton, MobTypeEnum.CaptainRenton);
            AddBidirectionalExits(oCaptainRenton, oMainAudienceChamber, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oCaptainRenton] = new PointF(2.5F, 10);

            Room oAdvisorsSecretChamber = AddRoom("Advisor's Chamber", "Advisor's Secret Chamber");
            e = AddExit(oCaptainRenton, oAdvisorsSecretChamber, "tapestry");
            e.Hidden = true;
            AddExit(oAdvisorsSecretChamber, oCaptainRenton, "tapestry");
            tharbadGraph.Rooms[oAdvisorsSecretChamber] = new PointF(2.5F, 11);

            Room oStepsToAzureTower = AddRoom("Azure Steps", "Steps to Azure Tower");
            e = AddExit(oAdvisorsSecretChamber, oStepsToAzureTower, "passage");
            e.Hidden = true;
            AddExit(oStepsToAzureTower, oHallOfRainbows2, "corridor");
            tharbadGraph.Rooms[oStepsToAzureTower] = new PointF(1.5F, 12);

            Room oAzureTowerStaircase1 = AddRoom("Azure Staircase", "Azure Tower Staircase");
            AddBidirectionalExits(oAzureTowerStaircase1, oStepsToAzureTower, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oAzureTowerStaircase1] = new PointF(1.5F, 11);

            Room oAzureTowerStaircase2 = AddRoom("Azure Tower Staircase", "Azure Tower Staircase");
            AddBidirectionalExits(oAzureTowerStaircase2, oAzureTowerStaircase1, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oAzureTowerStaircase2] = new PointF(1.5F, 10);

            Room oArenaPath = AddRoom("Arena Path", "Arena Path");
            AddBidirectionalExits(sabreEvard, oArenaPath, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oArenaPath] = new PointF(9, 8.5F);

            Room oArenaTunnel1 = AddRoom("Arena Tunnel", "Arena Tunnel");
            e = AddExit(oArenaPath, oArenaTunnel1, "arch");
            e.Hidden = true;
            AddExit(oArenaTunnel1, oArenaPath, "arch");
            tharbadGraph.Rooms[oArenaTunnel1] = new PointF(9, 9);

            Room oArenaTunnel2 = AddRoom("Arena Tunnel", "Arena Tunnel");
            AddBidirectionalSameNameExit(oArenaTunnel1, oArenaTunnel2, "slope");
            tharbadGraph.Rooms[oArenaTunnel2] = new PointF(9, 9.5F);

            Room oTunnel1 = AddRoom("Tunnel 1", "Tunnel One");
            AddBidirectionalExits(oTunnel1, oArenaTunnel2, BidirectionalExitType.SoutheastNorthwest);
            tharbadGraph.Rooms[oTunnel1] = new PointF(8, 9);

            Room oCenterRing = AddRoom("Center Ring", "Center Ring");
            AddBidirectionalExits(oCenterRing, oTunnel1, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oCenterRing] = new PointF(7, 9);

            Room oTunnel2 = AddRoom("Tunnel 2", "Tunnel Two");
            AddBidirectionalExits(oArenaTunnel2, oTunnel2, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oTunnel2] = new PointF(8, 10);

            Room oMiddleRing1 = AddRoom("Middle Ring", "Middle Ring");
            AddBidirectionalExits(oMiddleRing1, oTunnel2, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oMiddleRing1] = new PointF(7, 10);

            Room oMiddleRing2 = AddRoom("Middle Ring", "Middle Ring");
            AddBidirectionalSameNameExit(oMiddleRing1, oMiddleRing2, "ring");
            tharbadGraph.Rooms[oMiddleRing2] = new PointF(6, 10);

            Room oTunnel3 = AddRoom("Tunnel 3", "Tunnel Three");
            AddBidirectionalExits(oArenaTunnel2, oTunnel3, BidirectionalExitType.SoutheastNorthwest);
            tharbadGraph.Rooms[oTunnel3] = new PointF(10, 10);

            Room oOuterRingEast = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingEast, oTunnel3, BidirectionalExitType.UpDown);
            tharbadGraph.Rooms[oOuterRingEast] = new PointF(10, 11);

            Room oOuterRingNorth = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingNorth, oOuterRingEast, BidirectionalExitType.SoutheastNorthwest);
            tharbadGraph.Rooms[oOuterRingNorth] = new PointF(9, 10);

            Room oOuterRingWest = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingNorth, oOuterRingWest, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oOuterRingWest] = new PointF(8, 11);

            Room oOuterRingSouth = AddRoom("Outer Ring", "Outer Ring");
            AddBidirectionalExits(oOuterRingWest, oOuterRingSouth, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oOuterRingEast, oOuterRingSouth, BidirectionalExitType.SouthwestNortheast);
            tharbadGraph.Rooms[oOuterRingSouth] = new PointF(9, 12);

            tharbadEastGate = AddRoom("East Gate Outside", "Eastern Gate of Tharbad");
            AddBidirectionalSameNameExit(sabreEvard, tharbadEastGate, "gate");
            tharbadGraph.Rooms[tharbadEastGate] = new PointF(11, 8);
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
            AddPermanentMobs(breeStreets[1, 0], MobTypeEnum.SeasonedVeteran);
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
            AddPermanentMobs(breeStreets[1, 3], MobTypeEnum.SeasonedVeteran);
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
            AddPermanentMobs(breeStreets[8, 3], MobTypeEnum.SeasonedVeteran);
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
            AddPermanentMobs(oBigPapa, MobTypeEnum.BigPapa);
            Room oToMagicShop = breeStreets[10, 5] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x6
            breeStreets[14, 5] = AddRoom("Brownhaven", "Brownhaven Road"); //15x6
            breeStreets[0, 6] = AddRoom("Wain", "Wain Road South"); //1x7
            breeSewers[0, 6] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x7
            breeStreets[3, 6] = AddRoom("High", "South High Street"); //4x7
            breeStreets[7, 6] = AddRoom("Main", "Main Street"); //8x7
            breeStreets[10, 6] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x7
            breeStreets[14, 6] = AddRoom("Brownhaven", "Brownhaven Road"); //15x7
            oWestGateInside = breeStreets[0, 7] = AddRoom("West Gate Inside", "West Gate of Bree"); //1x8
            AddPermanentItems(oWestGateInside, ItemTypeEnum.GateWarning);
            breeSewers[0, 7] = AddRoom("Sewers West Gate", "Wain Road/Leviathan Way Sewer Main"); //1x8
            AddExit(breeSewers[0, 7], oWestGateInside, "up");
            breeStreets[1, 7] = AddRoom("Leviathan", "Leviathan Way"); //2x8
            Room oHauntedMansionEntrance = breeStreets[2, 7] = AddRoom("Leviathan", "Leviathan Way"); //3x8
            breeStreets[3, 7] = AddRoom("Leviathan/High", "Leviathan Way/High Street"); //4x8
            AddPermanentMobs(breeStreets[3, 7], MobTypeEnum.SeasonedVeteran);
            breeStreets[4, 7] = AddRoom("Leviathan", "Leviathan Way"); //5x8
            oBreeTownSquare = breeStreets[5, 7] = AddRoom("Town Square", "Bree Town Square"); //6x8
            AddPermanentMobs(oBreeTownSquare, MobTypeEnum.TheTownCrier, MobTypeEnum.Scribe, MobTypeEnum.SmallSpider, MobTypeEnum.Vagrant);
            AddPermanentItems(oBreeTownSquare, ItemTypeEnum.WelcomeSign);
            breeStreets[6, 7] = AddRoom("Leviathan", "Leviathan Way"); //7x8
            breeStreets[7, 7] = AddRoom("Leviathan/Main", "Leviathan Way/Main Street"); //8x8
            breeStreets[8, 7] = AddRoom("Leviathan", "Leviathan Way"); //9x8
            oNorthBridge = breeStreets[9, 7] = AddRoom("North Bridge", "North Bridge"); //10x8
            breeStreets[10, 7] = AddRoom("Leviathan/Crissaegrim", "Leviathan Way/Crissaegrim Road"); //11x8
            breeStreets[11, 7] = AddRoom("Leviathan", "Leviathan Way"); //12x8
            Room oLeviathanPoorAlley = breeStreets[12, 7] = AddRoom("Leviathan", "Leviathan Way"); //13x8
            Room oToGrantsStables = breeStreets[13, 7] = AddRoom("Leviathan", "Leviathan Way"); //14x8
            breeEastGateInside = breeStreets[14, 7] = AddRoom("East Gate Inside", "Bree's East Gate"); //15x8
            AddPermanentItems(breeEastGateInside, ItemTypeEnum.GateWarning);
            breeStreets[0, 8] = AddRoom("Wain", "Wain Road North"); //1x9
            breeSewers[0, 8] = AddRoom("Sewers Wain", "Wain Road Sewer Main"); //1x9
            breeStreets[3, 8] = AddRoom("High", "North High Street"); //4x9
            breeStreets[7, 8] = AddRoom("Main", "Main Street"); //8x9
            breeStreets[10, 8] = AddRoom("Crissaegrim", "Crissaegrim Road"); //11x9
            breeStreets[14, 8] = AddRoom("Brownhaven", "Brownhaven Road"); //15x9
            Room orderOfLove = breeStreets[15, 8] = AddHealingRoom("Order of Love", "Order of Love", HealingRoom.BreeNortheast); //16x9
            AddNonPermanentMobs(orderOfLove, MobTypeEnum.Drunk, MobTypeEnum.HobbitishDoctor, MobTypeEnum.Hobbit, MobTypeEnum.LittleMouse);
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
            AddPermanentMobs(oToCasino, MobTypeEnum.SeasonedVeteran);
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
            breeStreetsGraph.Rooms[oPoorAlley1] = new PointF(12, 4);

            Room oPoorAlley2 = AddRoom("Poor Alley", "Poor Alley");
            AddBidirectionalExits(oPoorAlley1, oPoorAlley2, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oPoorAlley2] = new PointF(12, 5);

            Room oPoorAlley3 = AddRoom("Poor Alley", "Poor Alley");
            AddBidirectionalExits(oPoorAlley2, oPoorAlley3, BidirectionalExitType.NorthSouth);
            AddExit(oPeriwinklePoorAlley, oPoorAlley3, "alley");
            AddExit(oPoorAlley3, oPeriwinklePoorAlley, "south");
            breeStreetsGraph.Rooms[oPoorAlley3] = new PointF(12, 6);

            Room oMensClub = AddRoom("Men's Club", "Men's Club");
            AddBidirectionalExits(oMensClub, oPoorAlley3, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oMensClub] = new PointF(11, 6);

            Room oCampusFreeClinic = AddHealingRoom("Bree Campus Free Clinic", "Campus Free Clinic", HealingRoom.BreeSouthwest);
            AddNonPermanentMobs(oCampusFreeClinic, MobTypeEnum.Student);
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");
            breeStreetsGraph.Rooms[oCampusFreeClinic] = new PointF(4, 9);

            Room oBreeRealEstateOffice = AddRoom("Real Estate Office", "Bree Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oBreeRealEstateOffice] = new PointF(11, 0.3F);

            Room oIxell = AddRoom("Ixell", "Kista Hills Show Home");
            AddPermanentMobs(oIxell, MobTypeEnum.IxellDeSantis);
            AddBidirectionalExitsWithOut(oBreeRealEstateOffice, oIxell, "door");
            breeStreetsGraph.Rooms[oIxell] = new PointF(11, 0.6F);

            Room oApartmentComplex = AddRoom("Apartment Complex", "Kista Hills Apartment Complex Entrance");
            AddBidirectionalExitsWithOut(oIxell, oApartmentComplex, "apartments");
            breeStreetsGraph.Rooms[oApartmentComplex] = new PointF(12, 0.6F);

            Room oApartmentComplexHallway = AddRoom("Hallway", "Kista Hills Apartment Complex Hallway");
            AddPermanentItems(oApartmentComplexHallway, ItemTypeEnum.StorageSign);
            AddBidirectionalExitsWithOut(oApartmentComplex, oApartmentComplexHallway, "hallway");
            breeStreetsGraph.Rooms[oApartmentComplexHallway] = new PointF(13, 0.6F);

            Room oApartmentComplexPlaza = AddRoom("Plaza", "Kista Hills Apartment Complex Plaza");
            AddPermanentItems(oApartmentComplexPlaza, ItemTypeEnum.StorageSign);
            AddBidirectionalExitsWithOut(oApartmentComplex, oApartmentComplexPlaza, "plaza");
            breeStreetsGraph.Rooms[oApartmentComplexPlaza] = new PointF(12, 1.6F);

            oConstructionSite = AddRoom("Construction Site", "Construction Site");
            AddExit(oIxell, oConstructionSite, "back door");
            AddExit(oConstructionSite, oIxell, "hoist");
            breeStreetsGraph.Rooms[oConstructionSite] = new PointF(11, 0.9F);

            Room oKistaHillsHousing = AddRoom("Kista Hills Housing", "Kista Hills Housing");
            AddBidirectionalExits(oStreetToFallon, oKistaHillsHousing, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oKistaHillsHousing] = new PointF(13, -0.5F);

            Room oChurchsEnglishGarden = AddRoom("Chuch's English Garden", "Church's English Garden");
            AddBidirectionalSameNameExit(oKistaHillsHousing, oChurchsEnglishGarden, "gate");
            Room oFallon = AddRoom("Fallon", "The Home of Church, the Cleric");
            AddPermanentMobs(oFallon, MobTypeEnum.Fallon);
            AddBidirectionalExitsWithOut(oChurchsEnglishGarden, oFallon, "door");
            breeStreetsGraph.Rooms[oChurchsEnglishGarden] = new PointF(13, -1);
            breeStreetsGraph.Rooms[oFallon] = new PointF(13, -1.5F);

            Room oGrantsStables = AddRoom("Grant's stables", "Grant's stables");
            e = AddExit(oToGrantsStables, oGrantsStables, "stable");
            e.MaximumLevel = 10;
            AddExit(oGrantsStables, oToGrantsStables, "south");
            breeStreetsGraph.Rooms[oGrantsStables] = new PointF(13, 2.5F);

            Room oGrant = AddRoom("Grant", "Grant's Office");
            AddPermanentMobs(oGrant, MobTypeEnum.Grant);
            AddBidirectionalExitsWithOut(oGrantsStables, oGrant, "gate", true);
            breeStreetsGraph.Rooms[oGrant] = new PointF(13, 2);

            Room oDTansLeatherArmory = AddRoom("Leather Armory", "D'Tan's Leather Armory");
            AddExit(oToGrantsStables, oDTansLeatherArmory, "armory");
            AddExit(oDTansLeatherArmory, oToGrantsStables, "north");
            breeStreetsGraph.Rooms[oDTansLeatherArmory] = new PointF(13, 3.5F);

            Room oPansy = AddRoom("Pansy Smallburrows", "Gambling Pit");
            AddPermanentMobs(oPansy, MobTypeEnum.PansySmallburrows);
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oPansy] = new PointF(13, 1);

            Room oIgor = AddRoom("Igor", "Blind Pig Pub");
            AddPermanentMobs(oIgor, MobTypeEnum.IgorTheBouncer);
            AddExit(oIgor, oToBlindPigPubAndUniversity, "east");
            AddExit(oToBlindPigPubAndUniversity, oIgor, "pub");
            breeStreetsGraph.Rooms[oIgor] = new PointF(2, 5.5F);

            Room oUniversityEntrance = AddRoom("University", "University of Bree");
            AddExit(oToBlindPigPubAndUniversity, oUniversityEntrance, "university");
            AddExit(oUniversityEntrance, oToBlindPigPubAndUniversity, "west");
            breeStreetsGraph.Rooms[breeStreets[3, 4]] = new PointF(3, 5.5F);
            breeStreetsGraph.Rooms[oUniversityEntrance] = new PointF(3.5F, 5.5F);

            Room oTulkasBookstore = AddRoom("Tulkas Bookstore", "Tulkas Memorial Bookstore");
            AddBidirectionalExits(oUniversityEntrance, oTulkasBookstore, BidirectionalExitType.SoutheastNorthwest);
            breeStreetsGraph.Rooms[oTulkasBookstore] = new PointF(3.75F, 6.5F);

            Room oTulkasLibrary = AddRoom("Tulkas Library", "Tulkas Memorial Library");
            AddPermanentMobs(oTulkasLibrary, MobTypeEnum.VeristriaTheLibrarian);
            AddBidirectionalExits(oTulkasBookstore, oTulkasLibrary, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oTulkasLibrary] = new PointF(4.75F, 6.5F);

            Room oFoyer = AddRoom("Foyer", "Foyer of the Iluvatar Research Building");
            e = AddExit(oUniversityEntrance, oFoyer, "east");
            e.MaximumLevel = 12;
            AddExit(oFoyer, oUniversityEntrance, "west");
            breeStreetsGraph.Rooms[oFoyer] = new PointF(4.25F, 5.5F);

            Room oInstructionalHall = AddRoom("Instruction", "Iluvatar Memorial Instructional Hall");
            AddBidirectionalExits(oFoyer, oInstructionalHall, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oInstructionalHall] = new PointF(4.25F, 5.75F);

            Room oPhrenologyOffice = AddRoom("Phrenology Office", "Department of Molecular Phrenology Office");
            e = AddExit(oInstructionalHall, oPhrenologyOffice, "office");
            e.RequiresDay = true;
            AddExit(oPhrenologyOffice, oInstructionalHall, "north");
            breeStreetsGraph.Rooms[oPhrenologyOffice] = new PointF(4.25F, 6);
            //CSRTODO: locked door

            Room oInstructionHall2 = AddRoom("Instruction", "Iluvatar Memorial Instructional Hall");
            AddBidirectionalExits(oInstructionalHall, oInstructionHall2, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oInstructionHall2] = new PointF(5, 5.75F);

            Room oInstructionHall3 = AddRoom("Instruction", "Iluvatar Memorial Instructional Hall");
            AddExit(oInstructionHall2, oInstructionHall3, "east");
            e = AddExit(oInstructionHall3, oInstructionHall2, "west");
            e.MaximumLevel = 9;
            breeStreetsGraph.Rooms[oInstructionHall3] = new PointF(5.75F, 5.75F);

            Room oPhrenology = AddRoom("Phrenology", "Molecular Phrenology Classroom");
            AddExit(oInstructionHall2, oPhrenology, "north");
            AddExit(oPhrenology, oInstructionHall2, "door");
            breeStreetsGraph.Rooms[oPhrenology] = new PointF(5, 5.5F);

            Room oResearchHall = AddRoom("Research", "Iluvatar Memorial Research Hall");
            AddBidirectionalExits(oResearchHall, oFoyer, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oResearchHall] = new PointF(4.25F, 5);

            Room oErech = AddRoom("Erech", "Erech's Laboratory");
            AddPermanentMobs(oErech, MobTypeEnum.Erech);
            AddExit(oResearchHall, oErech, "north");
            AddExit(oErech, oResearchHall, "door");
            breeStreetsGraph.Rooms[oErech] = new PointF(4.25F, 4.75F);

            Room oResearchHall2 = AddRoom("Research", "Iluvatar Memorial Research Hall");
            AddBidirectionalExits(oResearchHall, oResearchHall2, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oResearchHall2] = new PointF(5, 5);

            Room oMysticalSciences = AddRoom("Mystic Science", "Department of Mystical Sciences Office");
            e = AddExit(oResearchHall2, oMysticalSciences, "office");
            e.RequiresDay = true;
            AddExit(oMysticalSciences, oResearchHall2, "north");
            breeStreetsGraph.Rooms[oMysticalSciences] = new PointF(5, 5.25F);
            //CSRTODO: locked door

            Room oResearchHall3 = AddRoom("Research", "Iluvatar Memorial Research Hall");
            AddBidirectionalExits(oResearchHall2, oResearchHall3, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oResearchHall3] = new PointF(5.75F, 5);

            Room oPhrenologyLaboratory = AddRoom("Lab", "Molecular Phrenology Laboratory");
            AddExit(oResearchHall3, oPhrenologyLaboratory, "north");
            AddExit(oPhrenologyLaboratory, oResearchHall3, "door");
            breeStreetsGraph.Rooms[oPhrenologyLaboratory] = new PointF(5.75F, 4.75F);

            Room oMysticalScienceLab = AddRoom("Lab", "Mystical Sciences Laboratory");
            AddExit(oResearchHall3, oMysticalScienceLab, "south");
            AddExit(oMysticalScienceLab, oResearchHall3, "door");
            breeStreetsGraph.Rooms[oMysticalScienceLab] = new PointF(5.75F, 5.25F);

            Room oCampusWalkSouth = AddRoom("Walk", "Campus Walk South");
            AddBidirectionalExits(oInstructionHall3, oCampusWalkSouth, BidirectionalExitType.WestEast);
            AddExit(breeStreets[5, 3], oCampusWalkSouth, "university");
            AddExit(oCampusWalkSouth, breeStreets[5, 3], "south");
            breeStreetsGraph.Rooms[oCampusWalkSouth] = new PointF(6.25F, 5.75F);

            Room oCampusWalkSouth2 = AddRoom("Walk", "Campus Walk South");
            AddBidirectionalExits(oCampusWalkSouth2, oCampusWalkSouth, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oCampusWalkSouth2] = new PointF(6.25F, 5.5F);

            Room oFinancialAidOffice = AddRoom("Aid", "Financial Aid Office");
            AddBidirectionalExits(oCampusWalkSouth2, oFinancialAidOffice, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oFinancialAidOffice] = new PointF(6.6F, 5.5F);

            Room oCampusWalkSouth3 = AddRoom("Walk", "Campus Walk South");
            AddBidirectionalExits(oCampusWalkSouth3, oCampusWalkSouth2, BidirectionalExitType.NorthSouth);
            AddExit(oResearchHall3, oCampusWalkSouth3, "east");
            e = AddExit(oCampusWalkSouth3, oResearchHall3, "west");
            e.MaximumLevel = 12;
            breeStreetsGraph.Rooms[oCampusWalkSouth3] = new PointF(6.25F, 5);

            Room oUniversityQuad = AddRoom("Quad", "University Quad");
            AddBidirectionalExits(oUniversityQuad, oCampusWalkSouth3, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oUniversityQuad] = new PointF(6.25F, 4.75F);

            Room oCampusWalkEast = AddRoom("Walk", "Campus Walk East");
            AddBidirectionalExits(oUniversityQuad, oCampusWalkEast, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCampusWalkEast, breeStreets[7, 5], BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oCampusWalkEast] = new PointF(6.6F, 4.75F);
            breeStreetsGraph.Rooms[breeStreets[7, 5]] = new PointF(7, 4.75F);
            breeStreetsGraph.Rooms[oBigPapa] = new PointF(8, 4.75F);

            Room oCampusWalkNorth1 = AddRoom("Walk", "Campus Walk North");
            AddExit(oCampusWalkNorth1, oUniversityQuad, "south");
            e = AddExit(oUniversityQuad, oCampusWalkNorth1, "north");
            e.MaximumLevel = 7;
            breeStreetsGraph.Rooms[oCampusWalkNorth1] = new PointF(6.25F, 4.5F);

            Room oCampusWalkNorth2 = AddRoom("Walk", "Campus Walk North");
            AddBidirectionalExits(oCampusWalkNorth2, oCampusWalkNorth1, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oCampusWalkNorth2] = new PointF(6.25F, 4.25F);

            Room oLuistrin = AddRoom("Luistrin", "Halfast Hall");
            AddPermanentMobs(oLuistrin, MobTypeEnum.LuistrinTheArchitect);
            AddExit(oLuistrin, oCampusWalkNorth2, "out");
            AddExit(oCampusWalkNorth2, oLuistrin, "west");
            breeStreetsGraph.Rooms[oLuistrin] = new PointF(5.5F, 4.25F);

            Room oHalfastHall = AddRoom("Halfast Hall", "Halfast Hall");
            AddBidirectionalExits(oHalfastHall, oLuistrin, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oHalfastHall] = new PointF(4.75F, 4.25F);

            Room oToHalfastHall = AddRoom("Halfast Steps", "Steps to Halfast Hall");
            AddBidirectionalExits(oToHalfastHall, oHalfastHall, BidirectionalExitType.WestEast);
            e = AddExit(oUniversityEntrance, oToHalfastHall, "northeast");
            e.MaximumLevel = 7;
            AddExit(oToHalfastHall, oUniversityEntrance, "southwest");
            breeStreetsGraph.Rooms[oToHalfastHall] = new PointF(4, 4.25F);

            Room oSnarlingMutt = AddRoom("Snarling Mutt", "Snar Slystone's Apothecary and Curio Shoppe");
            AddPermanentMobs(oSnarlingMutt, MobTypeEnum.SnarlingMutt);
            AddBidirectionalExitsWithOut(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            breeStreetsGraph.Rooms[oSnarlingMutt] = new PointF(9, 6);

            Room oSnarSlystone = AddRoom("Snar Slystone", "Behind The Counter");
            AddPermanentMobs(oSnarSlystone, MobTypeEnum.SnarSlystone);
            AddBidirectionalExitsWithOut(oSnarlingMutt, oSnarSlystone, "counter");
            breeStreetsGraph.Rooms[oSnarSlystone] = new PointF(9, 5.75F);

            Room oBackHall = AddRoom("Back Hall", "Back Hall");
            AddBidirectionalSameNameExit(oSnarSlystone, oBackHall, "curtain");
            breeStreetsGraph.Rooms[oBackHall] = new PointF(9, 5.5F);

            Room oAtticCrawlway = AddRoom("Attic Crawlway", "Attic Crawlway");
            AddPermanentMobs(oAtticCrawlway, MobTypeEnum.Bugbear);
            e = AddExit(oBackHall, oAtticCrawlway, "hatch");
            e.Hidden = true;
            AddExit(oAtticCrawlway, oBackHall, "hatch");
            breeStreetsGraph.Rooms[oAtticCrawlway] = new PointF(9, 5.25F);

            Room oGuido = AddRoom("Guido", "Godfather's House of Games");
            AddPermanentMobs(oGuido, MobTypeEnum.Guido);
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");
            breeStreetsGraph.Rooms[oGuido] = new PointF(4, -0.5F);

            Room oGodfather = AddRoom("Godfather", "Godfather's Office");
            AddPermanentMobs(oGodfather, MobTypeEnum.Godfather);
            e = AddExit(oGuido, oGodfather, "door");
            e.Hidden = true;
            e.MustOpen = true;
            e = AddExit(oGodfather, oGuido, "door");
            e.MustOpen = true;
            breeStreetsGraph.Rooms[oGodfather] = new PointF(4, -1);

            Room oSergeantGrimgall = AddRoom("Sergeant Grimgall", "Guard Headquarters");
            AddPermanentMobs(oSergeantGrimgall, MobTypeEnum.SergeantGrimgall);
            AddExit(oToBarracks, oSergeantGrimgall, "barracks");
            AddExit(oSergeantGrimgall, oToBarracks, "east");
            breeStreetsGraph.Rooms[oSergeantGrimgall] = new PointF(6, 8);

            Room oGuardsRecRoom = AddRoom("Guard's Rec Room", "Guard's Rec Room");
            AddBidirectionalExits(oSergeantGrimgall, oGuardsRecRoom, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oGuardsRecRoom] = new PointF(6, 8.5F);

            Room oBreePawnShopWest = AddPawnShoppeRoom("Pawn SW", "Ixell's Antique Shop", PawnShoppe.BreeSouthwest);
            AddBidirectionalExits(oBreePawnShopWest, oToPawnShopWest, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oBreePawnShopWest] = new PointF(2, 8);

            Room oIxellsGeneralStore = AddRoom("General Store", "Ixell's General Store");
            AddExit(breeStreets[1, 3], oIxellsGeneralStore, "store");
            AddExit(oIxellsGeneralStore, breeStreets[1, 3], "north");
            AddBidirectionalSameNameExit(oIxellsGeneralStore, oBreePawnShopWest, "door");
            breeStreetsGraph.Rooms[oIxellsGeneralStore] = new PointF(1, 8);

            Room oBreePawnShopEast = AddPawnShoppeRoom("Pawn Shop", "Pawn Shop", PawnShoppe.BreeNortheast);
            e = AddExit(oPoorAlley1, oBreePawnShopEast, "east");
            e.Hidden = true;
            AddExit(oBreePawnShopEast, oPoorAlley1, "west");
            breeStreetsGraph.Rooms[oBreePawnShopEast] = new PointF(13, 4);

            Room oLeonardosFoundry = AddRoom("Leo Foundry", "Leonardo's Foundry");
            AddExit(oToLeonardosFoundry, oLeonardosFoundry, "foundry");
            AddExit(oLeonardosFoundry, oToLeonardosFoundry, "east");
            breeStreetsGraph.Rooms[oLeonardosFoundry] = new PointF(9, 1);

            Room oLeonardosSwords = AddRoom("Leo Swords", "Custom Swords");
            AddBidirectionalExits(oLeonardosSwords, oLeonardosFoundry, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oLeonardosSwords] = new PointF(9, 0.5F);

            Room oLeonardosArmor = AddRoom("Leo Armor", "Unblemished Armor");
            AddBidirectionalExits(oLeonardosArmor, oLeonardosFoundry, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oLeonardosArmor] = new PointF(8, 1);

            Room oLeonardosShields = AddRoom("Leo Shields", "Cast Iron Shields");
            AddBidirectionalExits(oLeonardosFoundry, oLeonardosShields, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oLeonardosShields] = new PointF(9, 1.5F);

            Room oZooEntrance = AddRoom("Zoo Entrance", "Scranlin's Zoological Wonders");
            AddExit(oToZoo, oZooEntrance, "zoo");
            AddExit(oZooEntrance, oToZoo, "exit");
            breeStreetsGraph.Rooms[oZooEntrance] = new PointF(2, -0.5F);

            Room oPathThroughScranlinsZoo = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo, oZooEntrance, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo] = new PointF(2, -1);

            Room oScranlinsPettingZoo = AddRoom("Petting Zoo", "Scranlin's Petting Zoo");
            AddExit(oPathThroughScranlinsZoo, oScranlinsPettingZoo, "north");
            AddExit(oScranlinsPettingZoo, oPathThroughScranlinsZoo, "south");
            breeStreetsGraph.Rooms[oScranlinsPettingZoo] = new PointF(2, -1.25F);

            Room oScranlinsTrainingArea = AddRoom("Training Area", "Scranlin's Training Area");
            oScranlinsTrainingArea.NoFlee = true;
            e = AddExit(oScranlinsPettingZoo, oScranlinsTrainingArea, "clearing");
            e.Hidden = true;
            AddExit(oScranlinsTrainingArea, oScranlinsPettingZoo, "gate");
            breeStreetsGraph.Rooms[oScranlinsTrainingArea] = new PointF(2, -1.5F);

            Room oScranlin = AddRoom("Scranlin", "Scranlin's Outhouse");
            AddPermanentMobs(oScranlin, MobTypeEnum.Scranlin);
            e = AddBidirectionalExitsWithOut(oScranlinsTrainingArea, oScranlin, "outhouse");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oScranlin] = new PointF(2, -1.75F);

            Room oPathThroughScranlinsZoo2 = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo2, oPathThroughScranlinsZoo, BidirectionalExitType.SoutheastNorthwest);
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo2] = new PointF(1, -2);

            Room oPathThroughScranlinsZoo3 = AddRoom("Path", "Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo3, oPathThroughScranlinsZoo2, BidirectionalExitType.SouthwestNortheast);
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo3] = new PointF(2, -3);

            Room oPathThroughScranlinsZoo4 = AddRoom("Path", "Path through Scranlin's Zoo");
            e = AddExit(oPathThroughScranlinsZoo3, oPathThroughScranlinsZoo4, "southeast");
            e.MaximumLevel = 10;
            AddExit(oPathThroughScranlinsZoo4, oPathThroughScranlinsZoo3, "northwest");
            e = AddExit(oPathThroughScranlinsZoo, oPathThroughScranlinsZoo4, "northeast");
            e.MaximumLevel = 10;
            AddExit(oPathThroughScranlinsZoo4, oPathThroughScranlinsZoo, "southwest");
            breeStreetsGraph.Rooms[oPathThroughScranlinsZoo4] = new PointF(3, -2);

            Room oDogHouse = AddRoom("Dog House", "The Dog House");
            AddPermanentMobs(oDogHouse, MobTypeEnum.Lathlorien);
            AddBidirectionalExitsWithOut(oPathThroughScranlinsZoo2, oDogHouse, "doghouse");
            breeStreetsGraph.Rooms[oDogHouse] = new PointF(1, -1);

            Room oMonkeyHouse = AddRoom("Monkey House", "Monkey House");
            AddBidirectionalExits(oMonkeyHouse, oPathThroughScranlinsZoo4, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oMonkeyHouse] = new PointF(2.67F, -2);

            Room oReptileHouse = AddRoom("Reptile House", "Reptile House");
            AddBidirectionalExits(oPathThroughScranlinsZoo4, oReptileHouse, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oReptileHouse] = new PointF(4, -2);

            Room oCreaturesOfMyth = AddRoom("Creatures of Myth", "Creatures of Myth");
            e = AddExit(oPathThroughScranlinsZoo2, oCreaturesOfMyth, "west");
            e.MinimumLevel = 10;
            AddExit(oCreaturesOfMyth, oPathThroughScranlinsZoo2, "east");
            breeStreetsGraph.Rooms[oCreaturesOfMyth] = new PointF(0, -2);

            Room oGeneticBlunders = AddRoom("Genetic Blunders", "Genetic Blunders");
            e = AddExit(oPathThroughScranlinsZoo2, oGeneticBlunders, "east");
            e.MinimumLevel = 4;
            AddExit(oGeneticBlunders, oPathThroughScranlinsZoo2, "west");
            breeStreetsGraph.Rooms[oGeneticBlunders] = new PointF(1.67F, -2);

            Room oBeastsOfFire = AddRoom("Beasts of Fire", "Beasts of Fire");
            e = AddExit(oPathThroughScranlinsZoo3, oBeastsOfFire, "north");
            e.MustOpen = true;
            e.MinimumLevel = 5;
            e = AddExit(oBeastsOfFire, oPathThroughScranlinsZoo3, "door");
            e.MustOpen = true;
            breeStreetsGraph.Rooms[oBeastsOfFire] = new PointF(2, -4);

            Room oOceania = AddRoom("Oceania", "Oceania");
            e = AddExit(oPathThroughScranlinsZoo3, oOceania, "south");
            e.MinimumLevel = 4;
            AddExit(oOceania, oPathThroughScranlinsZoo3, "north");
            breeStreetsGraph.Rooms[oOceania] = new PointF(2, -2.5F);
            //CSRTODO: tank (fly)

            boatswain = AddRoom("Boatswain", "Stern of the Celduin Express");
            AddPermanentMobs(boatswain, MobTypeEnum.Boatswain);
            boatswain.BoatLocationType = BoatEmbarkOrDisembark.CelduinExpress;
            breeStreetsGraph.Rooms[boatswain] = new PointF(9, 9.5F);
            e = AddExit(breeDocks, boatswain, "steamboat");
            e.BoatExitType = BoatExitType.BreeEnterCelduinExpress;
            e = AddExit(boatswain, breeDocks, "dock");
            e.BoatExitType = BoatExitType.BreeExitCelduinExpress;
            AddMapBoundaryPoint(breeDocks, boatswain, MapType.BreeStreets, MapType.Ships);

            Room oPearlAlley = AddRoom("Pearl Alley", "Pearl Alley");
            AddExit(oBreeTownSquare, oPearlAlley, "alley");
            AddExit(oPearlAlley, oBreeTownSquare, "north");
            breeStreetsGraph.Rooms[oPearlAlley] = new PointF(5, 4);

            Room oPrancingPony = AddRoom("Prancing Pony", "Prancing Pony Tavern");
            AddPermanentMobs(oPrancingPony, MobTypeEnum.Bartender, MobTypeEnum.Bartender, MobTypeEnum.Waitress, MobTypeEnum.Waitress, MobTypeEnum.Waitress);
            AddBidirectionalExits(oPearlAlley, oPrancingPony, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oPrancingPony] = new PointF(5.8F, 4);

            Room oHobbitsHideawayEntrance = AddRoom("Hideaway Entrance", "Entrance to the Hobbit's Hideaway");
            e = AddExit(orderOfLove, oHobbitsHideawayEntrance, "cubbyhole");
            e.Hidden = true;
            e.MaximumLevel = 8;
            AddExit(oHobbitsHideawayEntrance, orderOfLove, "west");
            breeStreetsGraph.Rooms[oHobbitsHideawayEntrance] = new PointF(16, 2);

            Room oHobbitClearing = AddRoom("Hobbit Clearing", "Hobbit Clearing");
            AddBidirectionalExits(oHobbitsHideawayEntrance, oHobbitClearing, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oHobbitClearing] = new PointF(17, 2);

            Room oChiefsHole = AddRoom("Chief's Hole", "Chief's Hole");
            AddBidirectionalExitsWithOut(oHobbitClearing, oChiefsHole, "chief's");
            breeStreetsGraph.Rooms[oChiefsHole] = new PointF(16, 1);

            Room oBranco = AddRoom("Branco", "The Chief's Bedchambers");
            AddPermanentMobs(oBranco, MobTypeEnum.BrancoTheHobbitsChief);
            AddBidirectionalExitsWithOut(oChiefsHole, oBranco, "bedchambers");
            breeStreetsGraph.Rooms[oBranco] = new PointF(15, 1);

            Room oHobbitsTemple = AddRoom("Temple", "The Hobbit's Temple");
            AddBidirectionalExitsWithOut(oHobbitClearing, oHobbitsTemple, "temple");
            breeStreetsGraph.Rooms[oHobbitsTemple] = new PointF(16, 2.5F);

            Room oBeneathTheHobbitsAltar = AddRoom("Under Altar", "Beneath the Hobbit's Altar");
            AddPermanentMobs(oBeneathTheHobbitsAltar, MobTypeEnum.LuthicTheHighPriestess);
            oBeneathTheHobbitsAltar.IsTrapRoom = true;
            e = AddExit(oHobbitsTemple, oBeneathTheHobbitsAltar, "altar");
            e.Hidden = true;
            AddExit(oBeneathTheHobbitsAltar, oHobbitsTemple, "up");
            breeStreetsGraph.Rooms[oBeneathTheHobbitsAltar] = new PointF(17, 2.5F);

            breeEastGateOutside = AddRoom("East Gate Outside", "East Gate of Bree");
            breeStreetsGraph.Rooms[breeEastGateOutside] = new PointF(18, 3);

            oCemetery = AddRoom("Cemetery", "The Cemetery");
            e = AddExit(breeEastGateOutside, oCemetery, "path");
            e.Hidden = true;
            e.RequiresDay = true;
            e = AddExit(oCemetery, oHobbitClearing, "west");
            e.MaximumLevel = 8;
            breeStreetsGraph.Rooms[oCemetery] = new PointF(18, 2);
            AddMapBoundaryPoint(breeEastGateOutside, oCemetery, MapType.BreeToImladris, MapType.BreeStreets);

            Room oCommonArea = AddRoom("Common Area", "The Common Area");
            AddBidirectionalExitsWithOut(oHobbitClearing, oCommonArea, "common");
            breeStreetsGraph.Rooms[oCommonArea] = new PointF(17, 1);

            Room oMainDiningHall = AddRoom("Dining Hall", "The Main Dining Hall");
            AddBidirectionalExitsWithOut(oCommonArea, oMainDiningHall, "dining");
            breeStreetsGraph.Rooms[oMainDiningHall] = new PointF(17, 0);

            Room oBigPapaSmallHallway = AddRoom("Small Hallway", "Small hallway");
            e = AddExit(oBigPapa, oBigPapaSmallHallway, "panel");
            e.Hidden = true;
            e = AddExit(oBigPapaSmallHallway, oBigPapa, "panel");
            e.MustOpen = true;
            breeStreetsGraph.Rooms[oBigPapaSmallHallway] = new PointF(8, 4.5F);

            Room oWizardsEye = AddRoom("Wizard's Eye", "The Wizard's Eye");
            e = AddBidirectionalExitsWithOut(breeStreets[5, 0], oWizardsEye, "north");
            e.RequiredClass = ClassType.Mage;
            breeStreetsGraph.Rooms[oWizardsEye] = new PointF(5, 9.75F);

            Room oMageTraining = AddRoom("Mage Training", "Mage Training");
            AddPermanentMobs(oMageTraining, MobTypeEnum.MagorTheInstructor);
            e = AddExit(oWizardsEye, oMageTraining, "archway");
            e.Hidden = true;
            AddExit(oMageTraining, oWizardsEye, "door");
            breeStreetsGraph.Rooms[oMageTraining] = new PointF(5, 9.5F);

            Room oPostOffice = AddRoom("Post Office", "Post Office");
            AddExit(breeStreets[1, 7], oPostOffice, "post office");
            AddExit(oPostOffice, breeStreets[1, 7], "north");
            breeStreetsGraph.Rooms[oPostOffice] = new PointF(1, 3.5F);

            Room oHallOfQuests = AddRoom("Quest Hall", "Hall of Quests");
            AddPermanentMobs(oHallOfQuests, MobTypeEnum.DenethoreTheWise);
            AddExit(breeStreets[1, 7], oHallOfQuests, "hall");
            AddExit(oHallOfQuests, breeStreets[1, 7], "south");
            breeStreetsGraph.Rooms[oHallOfQuests] = new PointF(1, 2.5F);

            Room oHallOfEarth = AddRoom("Earth Hall", "Hall of Earth");
            AddPermanentMobs(oHallOfEarth, MobTypeEnum.EarthenLoremaster);
            AddBidirectionalExitsWithOut(oHallOfQuests, oHallOfEarth, "earth");
            breeStreetsGraph.Rooms[oHallOfEarth] = new PointF(1, 2);

            Room oFarmersMarket = AddRoom("Farmer's Market", "Farmer's Market");
            AddExit(breeStreets[11, 7], oFarmersMarket, "market");
            AddExit(oFarmersMarket, breeStreets[11, 7], "south");
            breeStreetsGraph.Rooms[oFarmersMarket] = new PointF(11, 2.5F);

            Room oBank = AddRoom("Bank", "Bree Municipal Bank");
            AddExit(breeStreets[8, 7], oBank, "bank");
            AddExit(oBank, breeStreets[8, 7], "south");
            breeStreetsGraph.Rooms[oBank] = new PointF(8, 2.5F);

            Room oShrineEntrance = AddRoom("Shrine Entrance", "Entrance to Shrine of Moradin");
            AddExit(breeStreets[8, 7], oShrineEntrance, "shrine");
            AddExit(oShrineEntrance, breeStreets[8, 7], "north");
            breeStreetsGraph.Rooms[oShrineEntrance] = new PointF(8, 3.3F);

            Room oShrineFoyer = AddRoom("Shrine Foyer", "Foyer of Shrine of Moradin");
            AddBidirectionalSameNameExit(oShrineEntrance, oShrineFoyer, "gate");
            breeStreetsGraph.Rooms[oShrineFoyer] = new PointF(8, 3.8F);

            Room oShrineWest = AddRoom("Thoringil", "High Priest's Chambers");
            AddPermanentMobs(oShrineWest, MobTypeEnum.ThoringilTheHoly);
            e = AddExit(oShrineFoyer, oShrineWest, "west door");
            e.MustOpen = true;
            AddExit(oShrineWest, oShrineFoyer, "east");
            breeStreetsGraph.Rooms[oShrineWest] = new PointF(7.3F, 3.8F);

            Room oShrineEast = AddRoom("Prayer Room", "Prayer room");
            e = AddExit(oShrineFoyer, oShrineEast, "east door");
            e.MustOpen = true;
            AddExit(oShrineEast, oShrineFoyer, "west");
            breeStreetsGraph.Rooms[oShrineEast] = new PointF(8.7F, 3.8F);

            Room oGreatHallOfMoradin = AddRoom("Great Hall", "Great Hall of Moradin");
            AddBidirectionalExits(oShrineFoyer, oGreatHallOfMoradin, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oGreatHallOfMoradin] = new PointF(8, 4.3F);

            Room oChapel = AddRoom("Princess Bride", "The Chapel");
            AddPermanentMobs(oChapel, MobTypeEnum.BrideNamedPrincess);
            AddExit(breeStreets[6, 7], oChapel, "chapel");
            AddExit(oChapel, breeStreets[6, 7], "north");
            breeStreetsGraph.Rooms[oChapel] = new PointF(6, 3.5F);

            Room oShepherdsShop = AddRoom("Shepherd's Shop", "Shepherd's Shop");
            AddExit(breeStreets[6, 7], oShepherdsShop, "shop");
            AddExit(oShepherdsShop, breeStreets[6, 7], "south");
            breeStreetsGraph.Rooms[oShepherdsShop] = new PointF(6, 2.5F);

            Room oMonkTraining = AddRoom("Caladon Monastery", "Monastary");
            AddPermanentMobs(oMonkTraining, MobTypeEnum.AgedMonk);
            e = AddExit(breeStreets[7, 4], oMonkTraining, "monastary");
            e.RequiredClass = ClassType.Monk;
            AddExit(oMonkTraining, breeStreets[7, 4], "east");
            breeStreetsGraph.Rooms[oMonkTraining] = new PointF(6, 6.75F);
            breeStreetsGraph.Rooms[breeStreets[7, 4]] = new PointF(7, 6.75F);

            Room oOldClassRoom = AddRoom("Old Classroom", "Old Classroom");
            AddExit(oBreeTownSquare, oOldClassRoom, "school");
            AddExit(oOldClassRoom, oBreeTownSquare, "south");
            breeStreetsGraph.Rooms[oOldClassRoom] = new PointF(5, 2.5F);

            Room oClassroomDesk = AddRoom("Classroom Desk", Room.UNKNOWN_ROOM);
            AddExit(oOldClassRoom, oClassroomDesk, "north");
            breeStreetsGraph.Rooms[oClassroomDesk] = new PointF(5, 2);

            Room oGrandBallroom = AddRoom("Grant Ballroom", "The Isengard Grand Ballroom");
            AddBidirectionalExitsWithOut(breeStreets[4, 7], oGrandBallroom, "ballroom");
            breeStreetsGraph.Rooms[oGrandBallroom] = new PointF(4, 3.5F);

            Room oCityHall = AddRoom("City Hall", "Bree City Hall");
            AddPermanentItems(oCityHall, ItemTypeEnum.InformationKiosk, ItemTypeEnum.TownMap);
            AddExit(breeStreets[4, 7], oCityHall, "hall");
            AddExit(oCityHall, breeStreets[4, 7], "south");
            breeStreetsGraph.Rooms[oCityHall] = new PointF(4, 2.5F);

            Room oStocks = AddRoom("Stocks", "The Stocks");
            AddPermanentMobs(oStocks, MobTypeEnum.Rex, MobTypeEnum.Accuser);
            AddExit(oCityHall, oStocks, "stocks");
            AddExit(oStocks, oCityHall, "office");
            AddExit(oStocks, breeStreets[3, 9], "west");
            AddExit(breeStreets[3, 8], oStocks, "stocks");
            AddExit(breeStreets[3, 9], oStocks, "stocks");
            breeStreetsGraph.Rooms[oStocks] = new PointF(4, 2);

            Room oExecutionersChamber = AddRoom("Execution Chamber", "Executioner's Chamber");
            AddBidirectionalExits(oStocks, oExecutionersChamber, BidirectionalExitType.UpDown);
            breeStreetsGraph.Rooms[oExecutionersChamber] = new PointF(4, 1.5F);

            Room oHallOfAvatars = AddRoom("Avatar Hall", "Hall of Avatars");
            AddBidirectionalExitsWithOut(oCityHall, oHallOfAvatars, "avatar hall");
            breeStreetsGraph.Rooms[oHallOfAvatars] = new PointF(3.33F, 2.67F);

            Room oHallOfAvatars2 = AddRoom("Avatar Hall", "Hall of Avatars");
            AddBidirectionalSameNameExit(oHallOfAvatars, oHallOfAvatars2, "curtain");
            breeStreetsGraph.Rooms[oHallOfAvatars2] = new PointF(3.33F, 2.33F);

            Room oClans = AddRoom("Clans", "The Clans of Middle Earth");
            AddExit(breeStreets[7, 6], oClans, "hall");
            AddExit(oClans, breeStreets[7, 6], "east");
            breeStreetsGraph.Rooms[oClans] = new PointF(6.4F, 4);

            AddUndergroundTempleOfLolth(breeStreets);

            Room oReadyRoom = AddRoom("Ready Room", "Ready Room");
            AddExit(oToArena, oReadyRoom, "arena");
            AddExit(oReadyRoom, oToArena, "north");
            breeStreetsGraph.Rooms[oReadyRoom] = new PointF(5, 0.375F);

            Room oArenaWarmup = AddRoom("Warmup", "Arena Warmup");
            AddBidirectionalExits(oReadyRoom, oArenaWarmup, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oArenaWarmup] = new PointF(5, 0.75F);

            Room oMainArena = AddRoom("Main", "Main Arena");
            AddBidirectionalExits(oArenaWarmup, oMainArena, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oMainArena] = new PointF(5, 1.125F);

            Room oArenaChallenge = AddRoom("Challenge", "Arena Challenge");
            AddBidirectionalExits(oMainArena, oArenaChallenge, BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[oArenaChallenge] = new PointF(5, 1.5F);

            Room oArenaFirstAid = AddHealingRoom("First Aid", "Arena First Aid", HealingRoom.BreeArena);
            AddBidirectionalExits(oArenaFirstAid, oArenaChallenge, BidirectionalExitType.SouthwestNortheast);
            breeStreetsGraph.Rooms[oArenaFirstAid] = new PointF(6, 0.5F);

            Room oChampionsArena = AddRoom("Champion", "Champion's Arena");
            AddBidirectionalExits(oArenaChallenge, oChampionsArena, BidirectionalExitType.WestEast);
            breeStreetsGraph.Rooms[oChampionsArena] = new PointF(5.67F, 1.5F);

            Room oChampionsWarmup = AddRoom("Warmup", "Warmup for Champion's Arena");
            AddBidirectionalExits(oChampionsArena, oChampionsWarmup, BidirectionalExitType.WestEast);
            AddExit(oChampionsWarmup, breeStreets[7, 8], "east");
            e = AddExit(breeStreets[7, 8], oChampionsWarmup, "arena");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oChampionsWarmup] = new PointF(6.33F, 1.5F);
            breeStreetsGraph.Rooms[breeStreets[7, 8]] = new PointF(7, 1.5F);

            Room oHallOfGuilds = AddRoom("Guild Hall", "Hall of Guilds");
            e = AddExit(oToGuildHall, oHallOfGuilds, "hall");
            e.MustOpen = true;
            AddExit(oHallOfGuilds, oToGuildHall, "door");
            breeStreetsGraph.Rooms[oHallOfGuilds] = new PointF(8, 0);

            accursedGuildHall = AddRoom("Accursed Guild", "Accursed Guild Hall");
            AddBidirectionalExitsWithOut(oHallOfGuilds, accursedGuildHall, "accursed");
            breeStreetsGraph.Rooms[accursedGuildHall] = new PointF(6, -1);

            Room oDeathbringer = AddRoom("Deathbringer", "Chamber of the Accursed");
            AddPermanentMobs(oDeathbringer, MobTypeEnum.Deathbringer);
            AddBidirectionalSameNameExit(accursedGuildHall, oDeathbringer, "curtain");
            breeStreetsGraph.Rooms[oDeathbringer] = new PointF(5, -1);

            Room oItemsOfDeath = AddRoom("Death Items", "Items of Death");
            AddBidirectionalExitsWithOut(accursedGuildHall, oItemsOfDeath, "desk");
            breeStreetsGraph.Rooms[oItemsOfDeath] = new PointF(5, -0.5F);

            crusaderGuildHall = AddRoom("Crusader Guild", "Crusader's Guild");
            AddBidirectionalExitsWithOut(oHallOfGuilds, crusaderGuildHall, "crusader");
            breeStreetsGraph.Rooms[crusaderGuildHall] = new PointF(8, -1);

            Room oLadyGwyneth = AddRoom("Lady Gwyneth", "Guardian of the Crusader's Guild");
            AddBidirectionalExitsWithOut(crusaderGuildHall, oLadyGwyneth, "door");
            breeStreetsGraph.Rooms[oLadyGwyneth] = new PointF(7, -1);

            thievesGuildHall = AddRoom("Thieves Guild", "Thieves Guild");
            AddBidirectionalExitsWithOut(oHallOfGuilds, thievesGuildHall, "thief");
            breeStreetsGraph.Rooms[thievesGuildHall] = new PointF(9, -1);

            Room oWilliamTasker = AddRoom("William Tasker", "Dark Meeting Hall");
            AddBidirectionalExitsWithOut(thievesGuildHall, oWilliamTasker, "beads");
            breeStreetsGraph.Rooms[oWilliamTasker] = new PointF(10, -1);

            Room oHonestBobsDiscountWares = AddRoom("Discount Wares", "Honest Bob's Discount Wares");
            AddExit(thievesGuildHall, oHonestBobsDiscountWares, "counter");
            AddExit(oHonestBobsDiscountWares, thievesGuildHall, "hall");
            breeStreetsGraph.Rooms[oHonestBobsDiscountWares] = new PointF(10, -0.5F);

            Room oSmithy = AddRoom("Smithy", "U'tral Smiths Inc.");
            AddExit(oToSmithy, oSmithy, "smithy");
            AddExit(oSmithy, oToSmithy, "south");
            breeStreetsGraph.Rooms[oSmithy] = new PointF(13, 6.5F);

            Room oMagicShop = AddRoom("Magic Shop", "Magic Shop");
            AddBidirectionalExitsWithOut(oToMagicShop, oMagicShop, "shop");
            breeStreetsGraph.Rooms[oMagicShop] = new PointF(9, 5);

            Room oShippingWarehouse = AddRoom("Shipping Warehouse", "Shipping Warehouse");
            AddExit(breeStreets[11, 3], oShippingWarehouse, "warehouse");
            AddExit(oShippingWarehouse, breeStreets[11, 3], "north");
            AddExit(breeStreets[10, 2], oShippingWarehouse, "warehouse");
            AddExit(oShippingWarehouse, breeStreets[10, 2], "west");
            breeStreetsGraph.Rooms[oShippingWarehouse] = new PointF(11, 8);

            Room oSouthernDump = AddRoom("Dump", "The Southern Dump");
            AddExit(breeStreets[10, 1], oSouthernDump, "dump");
            AddExit(oSouthernDump, breeStreets[10, 1], "west");
            AddExit(breeStreets[11, 0], oSouthernDump, "dump");
            AddExit(oSouthernDump, breeStreets[11, 0], "south");
            breeStreetsGraph.Rooms[oSouthernDump] = new PointF(11, 9);

            Room oRavensRuseTavern = AddRoom("Raven's Ruse", "Raven's Ruse Tavern");
            AddExit(breeStreets[13, 0], oRavensRuseTavern, "tavern");
            AddExit(oRavensRuseTavern, breeStreets[13, 0], "south");
            AddExit(breeStreets[14, 1], oRavensRuseTavern, "tavern");
            AddExit(oRavensRuseTavern, breeStreets[14, 1], "east");
            breeStreetsGraph.Rooms[oRavensRuseTavern] = new PointF(13, 9);

            Room oThievesDen = AddRoom("Thieves Den", "Thieve's Den");
            e = AddExit(oRavensRuseTavern, oThievesDen, "sliding panel");
            e.Hidden = true;
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            AddExit(oThievesDen, oRavensRuseTavern, "door");
            breeStreetsGraph.Rooms[oThievesDen] = new PointF(13, 8.67F);

            Room oRavensBackroom = AddRoom("Backroom", "Raven's Backroom");
            e = AddExit(oThievesDen, oRavensBackroom, "hatchway");
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            e.Hidden = true;
            AddExit(oRavensBackroom, oThievesDen, "hatchway");
            breeStreetsGraph.Rooms[oRavensBackroom] = new PointF(13, 8.33F);

            Room oReneesElvenLeatherWorks = AddRoom("Leather Works", "Renee's Elven Leather Works");
            AddExit(breeStreets[10, 8], oReneesElvenLeatherWorks, "shoppe");
            AddExit(oReneesElvenLeatherWorks, breeStreets[10, 8], "west");
            breeStreetsGraph.Rooms[oReneesElvenLeatherWorks] = new PointF(11, 2);

            Room oHagbardsFineWeapons = AddRoom("Hagbard's Weapons", "Hagbard's Fine Weapons");
            AddExit(breeStreets[8, 3], oHagbardsFineWeapons, "shop");
            AddExit(oHagbardsFineWeapons, breeStreets[8, 3], "south");
            breeStreetsGraph.Rooms[oHagbardsFineWeapons] = new PointF(8, 6.5F);

            Room oBreeParkAndRecreation = AddRoom("Park and Recreation", "Bree Park and Recreation");
            AddExit(breeStreets[0, 4], oBreeParkAndRecreation, "park");
            AddExit(oBreeParkAndRecreation, breeStreets[0, 4], "west");
            breeStreetsGraph.Rooms[oBreeParkAndRecreation] = new PointF(1, 6);

            Room oKandyAndToyShoppe = AddRoom("Kandy/Toy Shop", "The Kandy and Toy Shoppe");
            e = AddExit(breeStreets[3, 6], oKandyAndToyShoppe, "shoppe");
            e.MaximumLevel = 6;
            AddExit(oKandyAndToyShoppe, breeStreets[3, 6], "east");
            breeStreetsGraph.Rooms[oKandyAndToyShoppe] = new PointF(2, 4);
            
            Room oBardConservatory = AddRoom("Bard Conservatory", "Bard Conservatory");
            AddPermanentMobs(oBardConservatory, MobTypeEnum.AgedBard);
            e = AddExit(breeStreets[4, 3], oBardConservatory, "conservatory");
            e.RequiredClass = ClassType.Bard;
            AddExit(oBardConservatory, breeStreets[4, 3], "north");
            breeStreetsGraph.Rooms[oBardConservatory] = new PointF(4, 8);

            AddPermanentItems(breeStreets[2, 0], ItemTypeEnum.AStatuetteOfBalthazar);
            Room oGoodwillShop = AddRoom("Goodwill", "Goodwill shop");
            AddPermanentItems(oGoodwillShop, ItemTypeEnum.Chest);
            AddExit(breeStreets[2, 0], oGoodwillShop, "shop");
            AddExit(oGoodwillShop, breeStreets[2, 0], "south");
            breeStreetsGraph.Rooms[oGoodwillShop] = new PointF(2, 9.75F);

            Room oWaitingRoom = AddRoom("Waiting Room", "Waiting Room");
            AddBidirectionalExitsWithOut(oGoodwillShop, oWaitingRoom, "door");
            breeStreetsGraph.Rooms[oWaitingRoom] = new PointF(2, 9.5F);

            Room oOffice = AddRoom("Office", "Office");
            AddPermanentItems(oOffice, ItemTypeEnum.Chest);
            AddBidirectionalExitsWithOut(oWaitingRoom, oOffice, "office");
            breeStreetsGraph.Rooms[oOffice] = new PointF(2, 9.25F);

            Room oBreesMerchantQuarter = AddRoom("Merchant Quarter", "Bree's Merchant Quarter");
            AddExit(breeStreets[7, 0], oBreesMerchantQuarter, "gates");
            AddExit(oBreesMerchantQuarter, breeStreets[7, 0], "southeast");
            breeStreetsGraph.Rooms[oBreesMerchantQuarter] = new PointF(6, 9);

            AddHauntedMansion(oHauntedMansionEntrance);
        }

        private void AddUndergroundTempleOfLolth(Room[,] breeStreets)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            Room oTempleOfLolth = AddRoom("Lolth Temple", "Temple of Lolth");
            AddExit(breeStreets[0, 9], oTempleOfLolth, "temple");
            AddExit(oTempleOfLolth, breeStreets[0, 9], "west");
            breeStreetsGraph.Rooms[oTempleOfLolth] = new PointF(-1.5F, 1);

            Room oUndergroundTemple = AddRoom("Drow Elfs", "Underground Temple of Lolth");
            oUndergroundTemple.IsTrapRoom = true;
            AddPermanentMobs(oUndergroundTemple, MobTypeEnum.DrowElf, MobTypeEnum.DrowElf, MobTypeEnum.DrowElf);
            Exit e = AddExit(oTempleOfLolth, oUndergroundTemple, "underground temple");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oUndergroundTemple] = new PointF(-1.5F, 1.5F);

            Room oSecretUndergroundPassage1 = AddRoom("Passage", "Secret Underground Passage");
            e = AddExit(oUndergroundTemple, oSecretUndergroundPassage1, "southeast");
            e.Hidden = true;
            e = AddExit(oSecretUndergroundPassage1, oUndergroundTemple, "up");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oSecretUndergroundPassage1] = new PointF(-0.75F, 2);

            Room oSecretUndergroundPassage2 = AddRoom("Passage", "Secret Underground Passage");
            AddBidirectionalExits(oSecretUndergroundPassage2, oSecretUndergroundPassage1, BidirectionalExitType.SouthwestNortheast);
            breeStreetsGraph.Rooms[oSecretUndergroundPassage2] = new PointF(0, 1.5F);

            Room oElvenSacrificialTemple = AddRoom("Sacrifice Temple", "Elven Sacrificial Temple");
            AddPermanentMobs(oElvenSacrificialTemple, MobTypeEnum.EvilHighPriestess);
            AddPermanentItems(oElvenSacrificialTemple, ItemTypeEnum.Anvil);
            AddBidirectionalExits(oElvenSacrificialTemple, oSecretUndergroundPassage2, BidirectionalExitType.SoutheastNorthwest);
            breeStreetsGraph.Rooms[oElvenSacrificialTemple] = new PointF(-0.5F, 1);

            Room oSecretUndergroundPassage3 = AddRoom("Gr Slime Lolth", "Secret Underground Passage");
            AddPermanentMobs(oSecretUndergroundPassage3, MobTypeEnum.GreenSlime);
            AddBidirectionalExits(oSecretUndergroundPassage3, oSecretUndergroundPassage2, BidirectionalExitType.SouthwestNortheast);
            breeStreetsGraph.Rooms[oSecretUndergroundPassage3] = new PointF(0.75F, 1);

            Room oCrematorium = AddRoom("Crematorium", "Crematorium");
            AddBidirectionalSameNameHiddenExit(oSecretUndergroundPassage3, oCrematorium, "passage");
            breeStreetsGraph.Rooms[oCrematorium] = new PointF(0.75F, 1.5F);

            Room oMausoleum = AddRoom("Mausoleum", "Mausoleum");
            AddExit(oCrematorium, oMausoleum, "up");
            e = AddExit(oMausoleum, oCrematorium, "down");
            e.Hidden = true;
            breeStreetsGraph.Rooms[oMausoleum] = new PointF(1.5F, 1);

            Room oDabinsFuneralHome = AddRoom("Funeral Home", "Dabin's Funeral Home");
            AddExit(breeStreets[3, 9], oDabinsFuneralHome, "home");
            AddExit(oDabinsFuneralHome, breeStreets[3, 9], "east");
            breeStreetsGraph.Rooms[oDabinsFuneralHome] = new PointF(2.25F, 1);

            Room oFuneralHomeCemetery = AddRoom("Cemetery", "Cemetery");
            AddPermanentMobs(oFuneralHomeCemetery, MobTypeEnum.Caretaker);
            e = AddExit(oDabinsFuneralHome, oFuneralHomeCemetery, "northwest");
            e.Hidden = true;
            AddExit(oFuneralHomeCemetery, oDabinsFuneralHome, "southeast");
            e = AddExit(oFuneralHomeCemetery, oMausoleum, "door");
            e.Hidden = true;
            //CSRTODO: hidden door exit that doesn't seem to be returnable since the return exit is locked
            //caretaker drops tin key but that doesn't work
            breeStreetsGraph.Rooms[oFuneralHomeCemetery] = new PointF(1.5F, 0.5F);
        }

        private void AddHauntedMansion(Room hauntedMansionEntrance)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            RoomGraph hauntedMansionGraph = _graphs[MapType.BreeHauntedMansion];

            hauntedMansionGraph.Rooms[hauntedMansionEntrance] = new PointF(2, 8);

            Room oOldGardener = AddRoom("Old Gardener", "Path to Mansion");
            AddPermanentMobs(oOldGardener, MobTypeEnum.OldGardener);
            Exit e = AddExit(hauntedMansionEntrance, oOldGardener, "gate");
            e.KeyType = SupportedKeysFlags.SilverKey;
            e.MustOpen = true;
            AddExit(oOldGardener, hauntedMansionEntrance, "gate");
            breeStreetsGraph.Rooms[oOldGardener] = new PointF(2, 2.5F);
            hauntedMansionGraph.Rooms[oOldGardener] = new PointF(2, 7);
            AddMapBoundaryPoint(hauntedMansionEntrance, oOldGardener, MapType.BreeStreets, MapType.BreeHauntedMansion);

            Room oFoyer = AddRoom("Foyer", "Foyer of the Old Mansion");
            e = AddBidirectionalExitsWithOut(oOldGardener, oFoyer, "door");
            e.KeyType = SupportedKeysFlags.SilverKey;
            e.MustOpen = true;
            hauntedMansionGraph.Rooms[oFoyer] = new PointF(2, 6);

            Room oDiningHall1 = AddRoom("Dining Hall", "The Mansion Dining Hall");
            AddBidirectionalExits(oDiningHall1, oFoyer, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oDiningHall1] = new PointF(1, 6);

            Room oDiningHall2 = AddRoom("Dining Hall", "North end of the Dining Hall");
            AddBidirectionalExits(oDiningHall2, oDiningHall1, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oDiningHall2] = new PointF(1, 5);

            Room oKitchen = AddRoom("Kitchen", "Kitchen");
            AddBidirectionalExits(oDiningHall2, oKitchen, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oKitchen] = new PointF(1.5F, 5);

            Room oDarkHallway = AddRoom("Dark Hallway", "Dark Hallway");
            AddBidirectionalExits(oDarkHallway, oDiningHall2, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oDarkHallway] = new PointF(1, 4);

            Room oStudy = AddRoom("Damaged Skeleton", "Study");
            AddPermanentMobs(oStudy, MobTypeEnum.DamagedSkeleton);
            e = AddExit(oDarkHallway, oStudy, "door");
            e.MustOpen = true;
            AddExit(oStudy, oDarkHallway, "door");
            hauntedMansionGraph.Rooms[oStudy] = new PointF(1, 3);

            Room oLivingRoom = AddRoom("Living Room", "Living Room");
            AddBidirectionalExits(oFoyer, oLivingRoom, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oLivingRoom] = new PointF(3, 6);

            Room oHallway = AddRoom("Hallway", "Hallway");
            AddBidirectionalExits(oHallway, oLivingRoom, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oHallway] = new PointF(3, 5);

            Room oBedroom = AddRoom("Bedroom", "Bedroom");
            e = AddExit(oHallway, oBedroom, "door");
            e.MustOpen = true;
            AddExit(oBedroom, oHallway, "door");
            hauntedMansionGraph.Rooms[oBedroom] = new PointF(3, 4);

            Room oStairwellTop = AddRoom("Stairwell Top", "Top of the Stairwell");
            AddBidirectionalExits(oStairwellTop, oFoyer, BidirectionalExitType.UpDown);
            hauntedMansionGraph.Rooms[oStairwellTop] = new PointF(2, 2);

            Room oSoutheasternHallwayCorner = AddRoom("Hallway", "Southeastern Hallway Corner");
            AddBidirectionalExits(oStairwellTop, oSoutheasternHallwayCorner, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oSoutheasternHallwayCorner] = new PointF(3, 2);

            Room oEasternHallway = AddRoom("Hallway", "Eastern Hallway");
            AddBidirectionalExits(oEasternHallway, oSoutheasternHallwayCorner, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oEasternHallway] = new PointF(3, 1);

            Room oChildsBedroom = AddRoom("Child's Bedroom", "Child's Bedroom");
            e = AddExit(oEasternHallway, oChildsBedroom, "door");
            e.MustOpen = true;
            AddExit(oChildsBedroom, oEasternHallway, "door");
            hauntedMansionGraph.Rooms[oChildsBedroom] = new PointF(2, 1);

            Room oGhostlyFencer = AddRoom("Ghostly Fencer", "Decrepit Training Room");
            AddPermanentMobs(oGhostlyFencer, MobTypeEnum.GhostlyFencer);
            AddExit(oEasternHallway, oGhostlyFencer, "north");
            AddExit(oGhostlyFencer, oEasternHallway, "southeast");
            hauntedMansionGraph.Rooms[oGhostlyFencer] = new PointF(2, 0);

            Room oWesternHallway = AddRoom("Hallway", "Western Hallway");
            AddExit(oWesternHallway, oGhostlyFencer, "north");
            AddExit(oGhostlyFencer, oWesternHallway, "southwest");
            hauntedMansionGraph.Rooms[oWesternHallway] = new PointF(0, 1);

            Room oSouthwesternHallwayCorner = AddRoom("Hallway", "Southwestern Hallway Corner");
            AddBidirectionalExits(oWesternHallway, oSouthwesternHallwayCorner, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oSouthwesternHallwayCorner] = new PointF(0, 2);

            Room oWesternHallway3 = AddRoom("Hallway", "Hallway");
            AddExit(oSouthwesternHallwayCorner, oWesternHallway3, "east");
            AddBidirectionalExits(oWesternHallway3, oStairwellTop, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oWesternHallway3] = new PointF(1, 2);

            Room oDen = AddRoom("Den", "Den");
            e = AddExit(oWesternHallway3, oDen, "door");
            e.MustOpen = true;
            AddExit(oDen, oWesternHallway3, "door");
            hauntedMansionGraph.Rooms[oDen] = new PointF(1, 1);

            Room oSecretAlcove = AddRoom("Secret Alcove", "Secret Alcove");
            AddPermanentItems(oSecretAlcove, ItemTypeEnum.RustyKey); //hidden
            e = AddExit(oLivingRoom, oSecretAlcove, "alcove");
            e.Hidden = true;
            AddExit(oSecretAlcove, oLivingRoom, "south");
            hauntedMansionGraph.Rooms[oSecretAlcove] = new PointF(4, 5);

            Room oMustyBasementEntrance = AddRoom("Basement Entrance", "Musty Basement Entrance");
            e = AddExit(oSecretAlcove, oMustyBasementEntrance, "hatch");
            e.KeyType = SupportedKeysFlags.RustyKey;
            e.MustOpen = true;
            AddExit(oMustyBasementEntrance, oSecretAlcove, "hatch");
            hauntedMansionGraph.Rooms[oMustyBasementEntrance] = new PointF(6, 2);

            Room oWesternBasement = AddRoom("Western Basement", "Western Basement");
            AddBidirectionalExits(oWesternBasement, oMustyBasementEntrance, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oWesternBasement] = new PointF(4, 2);

            Room oBasementCorner = AddRoom("Corner", "Southwest Corner of the Basement");
            AddBidirectionalExits(oWesternBasement, oBasementCorner, BidirectionalExitType.NorthSouth);
            hauntedMansionGraph.Rooms[oBasementCorner] = new PointF(4, 3);

            Room oBasementCorner2 = AddRoom("Corner", "Southeastern Corner of the Basement");
            AddBidirectionalExits(oBasementCorner, oBasementCorner2, BidirectionalExitType.WestEast);
            hauntedMansionGraph.Rooms[oBasementCorner2] = new PointF(4.75F, 3);

            Room oBasementCorner3 = AddRoom("Corner", "Northeastern corner of the Basement");
            AddPermanentMobs(oBasementCorner3, MobTypeEnum.GhostOfEvanNildredge);
            AddBidirectionalExits(oBasementCorner3, oWesternBasement, BidirectionalExitType.SouthwestNortheast);
            hauntedMansionGraph.Rooms[oBasementCorner3] = new PointF(5, 1);
        }

        private void AddUnderBree(Room oNorthBridge, Room oOuthouse, Room oSewerPipeExit)
        {
            RoomGraph underBreeGraph = _graphs[MapType.UnderBree];
            RoomGraph breeToImladrisGraph = _graphs[MapType.BreeToImladris];
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            Room droolie = AddRoom("Droolie", "Under North Bridge");
            AddPermanentMobs(droolie, MobTypeEnum.DroolieTheTroll);
            Exit e = AddExit(oNorthBridge, droolie, "rope");
            e.Hidden = true;
            AddExit(droolie, oNorthBridge, "up");
            breeStreetsGraph.Rooms[droolie] = new PointF(9, 3.3F);
            AddMapBoundaryPoint(oNorthBridge, droolie, MapType.BreeStreets, MapType.UnderBree);

            underBreeGraph.Rooms[oNorthBridge] = new PointF(-1, 0);
            underBreeGraph.Rooms[droolie] = new PointF(-1, 0.5F);
            underBreeGraph.Rooms[oOuthouse] = new PointF(8, 12);
            underBreeGraph.Rooms[oSewerPipeExit] = new PointF(7, 2);

            Room oCatchBasin = AddRoom("Catch Basin", "Catch Basin");
            e = AddBidirectionalExitsWithOut(oOuthouse, oCatchBasin, "hole");
            e.Hidden = true;
            underBreeGraph.Rooms[oCatchBasin] = new PointF(8, 11);
            breeToImladrisGraph.Rooms[oCatchBasin] = new PointF(-1.25F, 6.5F);
            AddMapBoundaryPoint(oOuthouse, oCatchBasin, MapType.BreeToImladris, MapType.UnderBree);

            Room oSepticTank = AddRoom("Septic Tank", "Septic Tank");
            AddBidirectionalSameNameExit(oCatchBasin, oSepticTank, "grate");
            underBreeGraph.Rooms[oSepticTank] = new PointF(8, 10);

            Room oDrainPipe1 = AddRoom("Drain Pipe", "Drain Pipe");
            AddBidirectionalSameNameExit(oSepticTank, oDrainPipe1, "pipe");
            underBreeGraph.Rooms[oDrainPipe1] = new PointF(8, 9);

            Room oDrainPipe2 = AddRoom("Drain Pipe", "Drain Pipe");
            AddBidirectionalSameNameExit(oDrainPipe1, oDrainPipe2, "culvert");
            underBreeGraph.Rooms[oDrainPipe2] = new PointF(8, 8);

            Room oBrandywineRiverShore = AddRoom("Brandywine Shore", "Southeastern Shore of the Brandywine River");
            AddBidirectionalExitsWithOut(oBrandywineRiverShore, oDrainPipe2, "grate");
            underBreeGraph.Rooms[oBrandywineRiverShore] = new PointF(8, 7);

            Room oSewerDitch = AddRoom("Ditch", "Sewer Ditch");
            AddBidirectionalExitsWithOut(oBrandywineRiverShore, oSewerDitch, "ditch");
            underBreeGraph.Rooms[oSewerDitch] = new PointF(8, 6);

            Room oSewerTunnel1 = AddRoom("Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel1, oSewerDitch, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTunnel1] = new PointF(8, 5);

            Room oBoardedSewerTunnel = AddRoom("to Monster", "Boarded Sewer Tunnel");
            e = AddExit(oSewerTunnel1, oBoardedSewerTunnel, "east");
            e.FloatRequirement = FloatRequirement.Levitation;
            AddExit(oBoardedSewerTunnel, oSewerTunnel1, "west");
            underBreeGraph.Rooms[oBoardedSewerTunnel] = new PointF(9, 5);

            Room oSewagePit = AddRoom("Sewage Pit", "Sewage Pit");
            AddPermanentMobs(oSewagePit, MobTypeEnum.Monster);
            AddPermanentItems(oSewagePit, ItemTypeEnum.KelpNecklace);
            oSewagePit.DamageType = RoomDamageType.Poison;
            e = AddExit(oSewagePit, oBoardedSewerTunnel, "up");
            e.FloatRequirement = FloatRequirement.Levitation;
            e = AddExit(oSewerTunnel1, oSewagePit, "east");
            e.FloatRequirement = FloatRequirement.NoLevitation;
            e.IsTrapExit = true;
            underBreeGraph.Rooms[oSewagePit] = new PointF(10, 4.5F);

            Room oStagnantCesspool = AddRoom("Stagnant Cesspool", "Stagnant Cesspool");
            AddBidirectionalExits(oSewagePit, oStagnantCesspool, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oStagnantCesspool] = new PointF(10, 5);

            Room oSewerTConnection = AddRoom("T-Connection", "Sewer T-Connection");
            AddBidirectionalExits(oSewerTConnection, oSewerTunnel1, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTConnection] = new PointF(8, 4);

            Room oSewerTunnel2 = AddRoom("Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel2, oSewerTConnection, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerTunnel2] = new PointF(7, 4);

            Room oSewerPipe = AddRoom("Sewer Pipe", "Sewer Pipe");
            AddExit(oSewerTunnel2, oSewerPipe, "pipe");
            AddExit(oSewerPipe, oSewerTunnel2, "down");
            AddExit(oSewerPipe, oSewerPipeExit, "up");
            underBreeGraph.Rooms[oSewerPipe] = new PointF(7, 3);
            breeStreetsGraph.Rooms[oSewerPipe] = new PointF(10, 11);
            AddMapBoundaryPoint(oSewerPipe, oSewerPipeExit, MapType.UnderBree, MapType.BreeStreets);

            Room oSalamander = AddRoom("Salamander", "The Brandywine River");
            AddPermanentMobs(oSalamander, MobTypeEnum.Salamander);
            AddExit(oBrandywineRiverShore, oSalamander, "reeds");
            AddExit(oSalamander, oBrandywineRiverShore, "shore");
            underBreeGraph.Rooms[oSalamander] = new PointF(7, 7);

            AddBrandywineRiverFromSalamander(oSalamander);

            Room oBrandywineRiver1 = AddRoom("River", "The Brandywine River");
            oBrandywineRiver1.DamageType = RoomDamageType.Water;
            AddExit(droolie, oBrandywineRiver1, "down");
            e = AddExit(oBrandywineRiver1, droolie, "rope");
            e.FloatRequirement = FloatRequirement.Fly;
            underBreeGraph.Rooms[oBrandywineRiver1] = new PointF(0, 1);

            Room oBrandywineRiver2 = AddRoom("River", "The Brandywine River");
            oBrandywineRiver2.DamageType = RoomDamageType.Water;
            AddBidirectionalExits(oBrandywineRiver1, oBrandywineRiver2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver2] = new PointF(1, 1);

            Room oBrandywineRiver3 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver3, oBrandywineRiver2, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver3] = new PointF(1, 0);

            Room oBrandywineRiver4 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver4, oBrandywineRiver1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBrandywineRiver4, oBrandywineRiver3, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver4] = new PointF(0, 0);

            Room oBrandywineRiver5 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver5, oBrandywineRiver4, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver5] = new PointF(0, -1);

            Room oBrandywineRiver6 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver6, oBrandywineRiver3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBrandywineRiver5, oBrandywineRiver6, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver6] = new PointF(1, -1);

            Room oBrandywineRiver7 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver7, oBrandywineRiver5, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver7] = new PointF(0, -2);

            Room oBrandywineRiver8 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver7, oBrandywineRiver8, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oBrandywineRiver8, oBrandywineRiver6, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver8] = new PointF(1, -2);

            Room oBrandywineRiver9 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver9, oBrandywineRiver7, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver9] = new PointF(0, -3);

            Room oBrandywineRiver10 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver9, oBrandywineRiver10, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oBrandywineRiver10, oBrandywineRiver8, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver10] = new PointF(1, -3);

            Room oBrandywineRiver11 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver11, oBrandywineRiver9, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver11] = new PointF(0, -4);

            Room oBrandywineRiver12 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver11, oBrandywineRiver12, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oBrandywineRiver12, oBrandywineRiver10, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver12] = new PointF(1, -4);

            Room oBrandywineRiver13 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver13, oBrandywineRiver11, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver13] = new PointF(0, -5);

            Room oLargeBoulder = AddRoom("Large Boulder", "Large Boulder");
            AddNonPermanentMobs(oLargeBoulder, MobTypeEnum.Griffon);
            AddExit(oBrandywineRiver13, oLargeBoulder, "boulder");
            AddExit(oLargeBoulder, oBrandywineRiver13, "river");
            underBreeGraph.Rooms[oLargeBoulder] = new PointF(-1, -5);

            Room oBrandywineRiver14 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver14, oBrandywineRiver12, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver14] = new PointF(1, -5);
            //CSRTODO: north, west (both requiring fly)

            Room oBrandywineRiver15 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver15, oBrandywineRiver13, BidirectionalExitType.NorthSouth);
            AddExit(oBrandywineRiver15, oBrandywineRiver14, "east");
            underBreeGraph.Rooms[oBrandywineRiver15] = new PointF(0, -6);

            Room oBrandywineRiver16 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver16, oBrandywineRiver15, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver16] = new PointF(0, -7);
            //CSRTODO: north

            Room oBrandywineRiver17 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver16, oBrandywineRiver17, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver17] = new PointF(1, -7);
            //CSRTODO: north, south (both requiring fly)

            Room oOohlgrist = AddRoom("Oohlgrist", "Small Boat");
            Trades[ItemTypeEnum.KelpNecklace] = MobTypeEnum.Oohlgrist;
            AddPermanentMobs(oOohlgrist, MobTypeEnum.Oohlgrist);
            AddExit(oBrandywineRiver2, oOohlgrist, "boat");
            AddExit(oOohlgrist, oBrandywineRiver2, "river");
            underBreeGraph.Rooms[oOohlgrist] = new PointF(2, 1);

            Room oBrandywineRiverBoathouse = AddRoom("Boathouse", "Brandywine River Boathouse");
            AddExit(oOohlgrist, oBrandywineRiverBoathouse, "shore");
            AddExit(oBrandywineRiverBoathouse, oOohlgrist, "boat");
            underBreeGraph.Rooms[oBrandywineRiverBoathouse] = new PointF(3, 1);

            Room oRockyBeach1 = AddRoom("Rocky Beach", "Rocky Beach");
            AddBidirectionalExits(oBrandywineRiverBoathouse, oRockyBeach1, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oRockyBeach1] = new PointF(4, 1);

            Room oRockyBeach2 = AddRoom("Rocky Beach", "Rocky Beach");
            AddBidirectionalExits(oRockyBeach1, oRockyBeach2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oRockyBeach2] = new PointF(5, 1);

            Room oHermitsCave = AddRoom("Hermit Fisher", "Hermit's Cave");
            AddPermanentMobs(oHermitsCave, MobTypeEnum.HermitFisher);
            e = AddBidirectionalExitsWithOut(oRockyBeach2, oHermitsCave, "cave");
            e.Hidden = true;
            underBreeGraph.Rooms[oHermitsCave] = new PointF(6, 1);

            Room oRockyAlcove = AddRoom("Rocky Alcove", "Rocky Alcove");
            AddExit(oRockyBeach1, oRockyAlcove, "alcove");
            AddExit(oRockyAlcove, oRockyBeach1, "north");
            underBreeGraph.Rooms[oRockyAlcove] = new PointF(5, 0);

            Room oSewerDrain = AddRoom("Drain", "Sewer Drain");
            AddBidirectionalSameNameExit(oRockyAlcove, oSewerDrain, "grate");
            underBreeGraph.Rooms[oSewerDrain] = new PointF(7, 0);

            Room oDrainTunnel2 = AddRoom("Drain Tunnel", "Drain Tunnel");
            underBreeGraph.Rooms[oDrainTunnel2] = new PointF(8, 1.5F);

            Room oDrainTunnel3 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddBidirectionalExits(oDrainTunnel2, oDrainTunnel3, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oDrainTunnel3] = new PointF(8, 2);

            Room oDrainTunnel4 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddBidirectionalExits(oDrainTunnel3, oDrainTunnel4, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oDrainTunnel4] = new PointF(8, 2.5F);

            Room oDrainTunnelDeadEnd1 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddExit(oDrainTunnel2, oDrainTunnelDeadEnd1, "north");
            AddExit(oDrainTunnelDeadEnd1, oDrainTunnel2, "north");
            AddExit(oSewerDrain, oDrainTunnelDeadEnd1, "south");
            underBreeGraph.Rooms[oDrainTunnelDeadEnd1] = new PointF(8, 1);

            Room oDrainTunnelDeadEnd2 = AddRoom("Drain Tunnel", "Drain Tunnel");
            AddExit(oDrainTunnelDeadEnd1, oDrainTunnelDeadEnd2, "south");
            AddExit(oDrainTunnelDeadEnd2, oDrainTunnelDeadEnd1, "south");
            underBreeGraph.Rooms[oDrainTunnelDeadEnd2] = new PointF(8, 0.5F);

            Room sewerTunnelToTConnection = AddRoom("Tunnel", "Sewer Tunnel");
            AddBidirectionalExits(oDrainTunnel4, sewerTunnelToTConnection, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(sewerTunnelToTConnection, oSewerTConnection, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[sewerTunnelToTConnection] = new PointF(8, 3);

            Room oBoardedSewerTunnel2 = AddRoom("to Sewer Orc Guards", "Boarded Sewer Tunnel");
            AddBidirectionalExits(sewerTunnelToTConnection, oBoardedSewerTunnel2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBoardedSewerTunnel2] = new PointF(9, 3);

            Room oSewerOrcChamber = AddRoom("Sewer Orc Guards", "Sewer Orc Chamber");
            AddPermanentMobs(oSewerOrcChamber, MobTypeEnum.SewerOrcGuard, MobTypeEnum.SewerOrcGuard);
            e = AddExit(oBoardedSewerTunnel2, oSewerOrcChamber, "busted board");
            e.Hidden = true;
            AddExit(oSewerOrcChamber, oBoardedSewerTunnel2, "busted board");
            underBreeGraph.Rooms[oSewerOrcChamber] = new PointF(10, 3);

            Room oSewerOrcLair = AddRoom("Sewer Orc Lair", "Sewer Orc Lair");
            AddPermanentMobs(oSewerOrcLair, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerRat, MobTypeEnum.SewerRat);
            AddBidirectionalExits(oSewerOrcLair, oSewerOrcChamber, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerOrcLair] = new PointF(10, 2);

            Room oSewerPassage = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oSewerOrcLair, oSewerPassage, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerPassage] = new PointF(11, 2);

            Room oSewerOrcStorageRoom = AddRoom("Storage Room", "Sewer Orc Storage Room");
            AddBidirectionalExits(oSewerPassage, oSewerOrcStorageRoom, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerOrcStorageRoom] = new PointF(12, 2);

            Room oSlopingSewerPassage = AddRoom("Sloping Passage", "Sloping Sewer Passage");
            AddBidirectionalExits(oSewerOrcStorageRoom, oSlopingSewerPassage, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSlopingSewerPassage] = new PointF(12, 3);

            Room oSewerPassageInFrontOfGate = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oSlopingSewerPassage, oSewerPassageInFrontOfGate, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerPassageInFrontOfGate] = new PointF(12, 4);

            Room oSmoothedSewerPassage = AddRoom("Smoothed Passage", "Smoothed Sewer Passage");
            AddBidirectionalExits(oSewerPassageInFrontOfGate, oSmoothedSewerPassage, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSmoothedSewerPassage] = new PointF(12, 5);

            Room oSlopingSewerPassage2 = AddRoom("Sewer Wolf", "Sloping Sewer Passage");
            AddPermanentMobs(oSlopingSewerPassage2, MobTypeEnum.SewerWolf);
            AddBidirectionalExits(oStagnantCesspool, oSlopingSewerPassage2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oSlopingSewerPassage2, oSmoothedSewerPassage, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSlopingSewerPassage2] = new PointF(11, 5);

            Room oGatedPassage = AddRoom("Gated Passage", "Gated Passage");
            e = AddExit(oSewerPassageInFrontOfGate, oGatedPassage, "gate");
            AddExit(oGatedPassage, oSewerPassageInFrontOfGate, "gate");
            e.KeyType = SupportedKeysFlags.TombKey;
            e.MustOpen = true;
            underBreeGraph.Rooms[oGatedPassage] = new PointF(13, 4);

            Room oDustyPassage1 = AddRoom("Dusty Passage", "Dusty Passage");
            AddBidirectionalExits(oGatedPassage, oDustyPassage1, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oDustyPassage1] = new PointF(14, 4);

            Room oDustyPassage2 = AddRoom("Dusty Passage", "Dusty Passage");
            AddBidirectionalExits(oDustyPassage1, oDustyPassage2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oDustyPassage2] = new PointF(15, 4);

            Room oDustyPassage3 = AddRoom("Dusty Passage", "Dusty Passage");
            AddExit(oDustyPassage2, oDustyPassage3, "east");
            AddExit(oDustyPassage3, oDustyPassage2, "east");
            underBreeGraph.Rooms[oDustyPassage3] = new PointF(16, 4);

            Room oDustyPassage4 = AddRoom("Dusty Passage", "Dusty Passage");
            AddExit(oDustyPassage3, oDustyPassage4, "west");
            AddExit(oDustyPassage4, oDustyPassage1, "west");
            AddExit(oDustyPassage4, oDustyPassage3, "east");
            underBreeGraph.Rooms[oDustyPassage4] = new PointF(15, 3.5F);

            Room oDustyGrotto = AddRoom("Dusty Grotto", "Dusty Grotto");
            e = AddExit(oDustyPassage4, oDustyGrotto, "down");
            e.Hidden = true;
            AddExit(oDustyGrotto, oDustyPassage4, "up");
            underBreeGraph.Rooms[oDustyGrotto] = new PointF(16, 3.5F);

            Room oFungalGrotto = AddRoom("Fungal Grotto", "Fungal Grotto");
            AddBidirectionalExits(oDustyGrotto, oFungalGrotto, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oFungalGrotto] = new PointF(17, 3.5F);

            Room oFungalSewerCrypt = AddRoom("Fungal Sewer Crypt", "Sewer Crypt");
            AddBidirectionalExits(oFungalGrotto, oFungalSewerCrypt, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oFungalSewerCrypt] = new PointF(18, 3.5F);

            Room oSewerOrcTomb1 = AddRoom("Sewer Orc Tomb", "Sewer Orc Tomb");
            AddBidirectionalExits(oSewerOrcTomb1, oFungalSewerCrypt, BidirectionalExitType.SoutheastNorthwest);
            underBreeGraph.Rooms[oSewerOrcTomb1] = new PointF(17, 2.5F);

            Room oSewerOrcTomb2 = AddRoom("Sewer Orc Tomb", "Sewer Orc Tomb");
            AddBidirectionalExits(oSewerOrcTomb2, oFungalSewerCrypt, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerOrcTomb2] = new PointF(18, 2.5F);

            Room oSewerOrcTomb3 = AddRoom("Mummy Threshold", "Sewer Orc Tomb");
            AddBidirectionalExits(oSewerOrcTomb3, oFungalSewerCrypt, BidirectionalExitType.SouthwestNortheast);
            underBreeGraph.Rooms[oSewerOrcTomb3] = new PointF(19, 2.5F);

            Room oSewerOrcTomb4 = AddRoom("Sewer Orc Tomb", "Sewer Orc Tomb");
            AddBidirectionalExits(oFungalSewerCrypt, oSewerOrcTomb4, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerOrcTomb4] = new PointF(18, 4.5F);

            Room oSewerOrcTomb5 = AddRoom("Sewer Orc Tomb", "Sewer Orc Tomb");
            AddBidirectionalExits(oFungalSewerCrypt, oSewerOrcTomb5, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerOrcTomb5] = new PointF(19, 3.5F);

            Room oScrawledPit = AddRoom("Sewer Orc Mummy", "Scrawled Pit");
            AddPermanentMobs(oScrawledPit, MobTypeEnum.SewerOrcMummy);
            e = AddExit(oSewerOrcTomb3, oScrawledPit, "sarcophagus");
            e.Hidden = true;
            AddExit(oScrawledPit, oSewerOrcTomb3, "up");
            underBreeGraph.Rooms[oScrawledPit] = new PointF(19, 1.75F);

            Room oDirtHole = AddRoom("Dirt Hole", "Dirt Hole");
            AddPermanentItems(oDirtHole, ItemTypeEnum.MauveScroll);
            e = AddBidirectionalExitsWithOut(oScrawledPit, oDirtHole, "hole");
            e.RequiresNoItems = true;
            underBreeGraph.Rooms[oDirtHole] = new PointF(19, 1.5F);
        }

        private void AddBrandywineRiverFromSalamander(Room oSalamander)
        {
            RoomGraph underBreeGraph = _graphs[MapType.UnderBree];

            Room oBrandywineRiverFromSalamander = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oSalamander, oBrandywineRiverFromSalamander, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiverFromSalamander] = new PointF(6, 9);

            Room oBrandywineRiver2FromSalamander = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiverFromSalamander, oBrandywineRiver2FromSalamander, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver2FromSalamander] = new PointF(6, 10);

            Room oBrandywineRiver3FromSalamander = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver2FromSalamander, oBrandywineRiver3FromSalamander, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver3FromSalamander] = new PointF(6, 11);

            Room oBrandywineRiver4FromSalamander = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver3FromSalamander, oBrandywineRiver4FromSalamander, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oBrandywineRiver4FromSalamander] = new PointF(6, 12);
            //CSRTODO: east, west

            Room oBrandywineRiver5 = AddRoom("River", "The Brandywine River");
            AddExit(oBrandywineRiverFromSalamander, oBrandywineRiver5, "west");
            AddExit(oBrandywineRiver5, oBrandywineRiver3FromSalamander, "east");
            underBreeGraph.Rooms[oBrandywineRiver5] = new PointF(5, 9);

            Room oBrandywineRiver6 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver5, oBrandywineRiver6, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBrandywineRiver6, oBrandywineRiver2FromSalamander, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver6] = new PointF(5, 10);

            Room oBrandywineRiver7 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver6, oBrandywineRiver7, BidirectionalExitType.NorthSouth);
            AddExit(oBrandywineRiver7, oBrandywineRiver3FromSalamander, "west");
            underBreeGraph.Rooms[oBrandywineRiver7] = new PointF(5, 11);
            //CSRTODO: south

            Room oBrandywineRiver8 = AddRoom("River", "The Brandywine River");
            AddExit(oBrandywineRiver5, oBrandywineRiver8, "north");
            AddExit(oBrandywineRiver8, oBrandywineRiver2FromSalamander, "east");
            AddExit(oBrandywineRiver8, oBrandywineRiverFromSalamander, "north");
            underBreeGraph.Rooms[oBrandywineRiver8] = new PointF(5, 8);

            Room oBrandywineRiver9 = AddRoom("River", "The Brandywine River");
            AddExit(oBrandywineRiverFromSalamander, oBrandywineRiver9, "east");
            AddExit(oBrandywineRiver9, oBrandywineRiver2FromSalamander, "west");
            underBreeGraph.Rooms[oBrandywineRiver9] = new PointF(7, 9);
            //CSRTODO: south

            Room oBrandywineRiver10 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver10, oBrandywineRiver9, BidirectionalExitType.NorthSouth);
            AddExit(oBrandywineRiver10, oSalamander, "north");
            AddExit(oBrandywineRiver10, oBrandywineRiver2FromSalamander, "west");
            underBreeGraph.Rooms[oBrandywineRiver10] = new PointF(7, 8);

            Room oBrandywineRiver11 = AddRoom("River", "The Brandywine River");
            AddExit(oBrandywineRiver2FromSalamander, oBrandywineRiver11, "east");
            AddExit(oBrandywineRiver11, oBrandywineRiver4FromSalamander, "west");
            AddBidirectionalExits(oBrandywineRiver9, oBrandywineRiver11, BidirectionalExitType.NorthSouth);
            //CSRTODO: south
            underBreeGraph.Rooms[oBrandywineRiver11] = new PointF(7, 10);

            Room oBrandywineRiver12 = AddRoom("River", "The Brandywine River");
            AddBidirectionalExits(oBrandywineRiver4FromSalamander, oBrandywineRiver12, BidirectionalExitType.NorthSouth);
            AddExit(oBrandywineRiver7, oBrandywineRiver12, "east");
            underBreeGraph.Rooms[oBrandywineRiver12] = new PointF(6, 13);

            Room oBrandywineRiver13 = AddRoom("River", "The Brandywine River");
            AddExit(oBrandywineRiver3FromSalamander, oBrandywineRiver13, "east");
            underBreeGraph.Rooms[oBrandywineRiver13] = new PointF(7, 11);
            //CSRTODO: north, south, east
        }

        private void AddBreeSewers(Room[,] breeStreets, Room[,] breeSewers, out Room oSmoulderingVillage)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];
            RoomGraph breeSewersGraph = _graphs[MapType.BreeSewers];

            //add exits for the sewers. due to screwiness on periwinkle this can't be done automatically.
            AddBidirectionalExits(breeSewers[0, 10], breeSewers[0, 9], BidirectionalExitType.NorthSouth);
            breeStreetsGraph.Rooms[breeSewers[0, 10]] = new PointF(-3.25F, -1);
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
            breeSewersGraph.Rooms[breeStreets[0, 10]] = new PointF(5, 1);
            breeSewersGraph.Rooms[breeSewers[0, 10]] = new PointF(5, 2);
            breeSewersGraph.Rooms[breeSewers[0, 9]] = new PointF(5, 3);
            breeSewersGraph.Rooms[breeSewers[0, 8]] = new PointF(5, 4);
            breeSewersGraph.Rooms[breeSewers[0, 7]] = new PointF(5, 5);
            breeSewersGraph.Rooms[breeSewers[0, 6]] = new PointF(5, 6);
            breeSewersGraph.Rooms[breeSewers[0, 5]] = new PointF(5, 7);
            breeSewersGraph.Rooms[breeSewers[0, 4]] = new PointF(5, 8);
            breeSewersGraph.Rooms[breeSewers[0, 3]] = new PointF(5, 9);
            breeSewersGraph.Rooms[breeSewers[1, 3]] = new PointF(6, 9);
            breeSewersGraph.Rooms[breeSewers[2, 3]] = new PointF(7, 8);
            breeSewersGraph.Rooms[breeSewers[3, 3]] = new PointF(8, 9);
            breeSewersGraph.Rooms[breeSewers[4, 3]] = new PointF(9, 9);
            breeSewersGraph.Rooms[breeSewers[5, 3]] = new PointF(10, 9);
            breeSewersGraph.Rooms[breeSewers[6, 3]] = new PointF(11, 9);
            breeSewersGraph.Rooms[breeSewers[7, 3]] = new PointF(12, 9);

            Room oTunnel = AddRoom("Tunnel", "Tunnel");
            Exit e = AddExit(breeSewers[0, 10], oTunnel, "tunnel");
            e.Hidden = true;
            AddExit(oTunnel, breeSewers[0, 10], "tunnel");
            breeSewersGraph.Rooms[oTunnel] = new PointF(4, 2);

            Room oLatrine = AddRoom("Latrine", "Latrine");
            oLatrine.DamageType = RoomDamageType.Wind;
            AddExit(oTunnel, oLatrine, "south");
            e = AddExit(oLatrine, oTunnel, "north");
            e.Hidden = true;
            breeSewersGraph.Rooms[oLatrine] = new PointF(4, 3);

            Room oEugenesDungeon = AddRoom("Eugene's Dungeon", "Eugene's Dungeon");
            AddBidirectionalExits(oEugenesDungeon, oLatrine, BidirectionalExitType.SouthwestNortheast);
            breeSewersGraph.Rooms[oEugenesDungeon] = new PointF(3, 2);

            Room oShadowOfIncendius = AddRoom("Shadow of Incendius", "Honorary Holding");
            AddPermanentMobs(oShadowOfIncendius, MobTypeEnum.ShadowOfIncendius);
            AddBidirectionalExits(oShadowOfIncendius, oEugenesDungeon, BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[oShadowOfIncendius] = new PointF(2, 2);

            Room oEugeneTheExecutioner = AddRoom("Eugene the Executioner", "Torture Room");
            AddPermanentMobs(oEugeneTheExecutioner, MobTypeEnum.EugeneTheExecutioner);
            AddPermanentItems(oEugeneTheExecutioner, ItemTypeEnum.CarvedIvoryKey);
            oEugeneTheExecutioner.DamageType = RoomDamageType.Fire;
            AddExit(oEugenesDungeon, oEugeneTheExecutioner, "up");
            oEugeneTheExecutioner.IsTrapRoom = true;
            breeSewersGraph.Rooms[oEugeneTheExecutioner] = new PointF(3, 1);

            Room oBurnedRemainsOfNimrodel = AddRoom("Nimrodel", "Cellar");
            AddPermanentMobs(oBurnedRemainsOfNimrodel, MobTypeEnum.BurnedRemainsOfNimrodel);
            AddBidirectionalExitsWithOut(oBurnedRemainsOfNimrodel, oEugeneTheExecutioner, "door");
            breeSewersGraph.Rooms[oBurnedRemainsOfNimrodel] = new PointF(2, 1);

            Room aqueduct = AddRoom("Aqueduct", "Aqueduct");
            AddBidirectionalExitsWithOut(oBurnedRemainsOfNimrodel, aqueduct, "pipe");
            breeSewersGraph.Rooms[aqueduct] = new PointF(1, 2);

            Room oShirriff = breeSewers[7, 3];
            AddPermanentMobs(oShirriff, MobTypeEnum.Shirriff, MobTypeEnum.Shirriff);
            AddPermanentItems(oShirriff, ItemTypeEnum.PotHelm, ItemTypeEnum.Torch);

            Room oValveChamber = AddRoom("Valve Chamber", "Valve chamber");
            e = AddExit(breeSewers[7, 3], oValveChamber, "valve");
            e.Hidden = true;
            AddExit(oValveChamber, breeSewers[7, 3], "south");
            breeSewersGraph.Rooms[oValveChamber] = new PointF(12, 8);

            Room oSewerPassageFromValveChamber = AddRoom("Sewer Passage", "Sewer Passage");
            AddBidirectionalExits(oSewerPassageFromValveChamber, oValveChamber, BidirectionalExitType.NorthSouth);
            breeSewersGraph.Rooms[oSewerPassageFromValveChamber] = new PointF(12, 7);

            Room oCentralSewerChannels = AddRoom("Central Sewer Channels", "Central Sewer Channels");
            //CSRTODO
            //oCentralSewerChannels.Mob1 = "demon";
            AddBidirectionalExits(oCentralSewerChannels, oSewerPassageFromValveChamber, BidirectionalExitType.SoutheastNorthwest);
            breeSewersGraph.Rooms[oCentralSewerChannels] = new PointF(11, 6);

            Room oSewerPassageToSewerDemon = AddRoom("Passage", "Sewer Passage");
            oSewerPassageToSewerDemon.DamageType = RoomDamageType.Earth;
            e = AddExit(oCentralSewerChannels, oSewerPassageToSewerDemon, "northwest");
            e.Hidden = true;
            AddExit(oSewerPassageToSewerDemon, oCentralSewerChannels, "southeast");
            breeSewersGraph.Rooms[oSewerPassageToSewerDemon] = new PointF(10, 5);

            Room oSewerPassageFromCentChannel = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oCentralSewerChannels, oSewerPassageFromCentChannel, BidirectionalExitType.SouthwestNortheast);
            breeSewersGraph.Rooms[oSewerPassageFromCentChannel] = new PointF(10, 7);

            Room oSewerTIntersection = AddRoom("T-Intersection", "Sewer T-Intersection");
            AddBidirectionalExits(oSewerTIntersection, oSewerPassageFromCentChannel, BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[oSewerTIntersection] = new PointF(9, 7);

            Room oSewerValveChamber2 = AddRoom("Valve Chamber", "Sewer Valve Chamber");
            AddBidirectionalExits(oSewerValveChamber2, oSewerTIntersection, BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[oSewerValveChamber2] = new PointF(8, 7);

            Room oSewerPassageToDrainageChamber = AddRoom("Passage", "Sewer Passage");
            AddBidirectionalExits(oSewerTIntersection, oSewerPassageToDrainageChamber, BidirectionalExitType.NorthSouth);
            breeSewersGraph.Rooms[oSewerPassageToDrainageChamber] = new PointF(8, 7.5F);

            Room oDrainageChamber = AddRoom("Drainage Chamber", "Drainage Chamber");
            e = AddExit(oSewerPassageToDrainageChamber, oDrainageChamber, "door");
            e.MustOpen = true;
            AddExit(oDrainageChamber, oSewerPassageToDrainageChamber, "door");
            breeSewersGraph.Rooms[oDrainageChamber] = new PointF(8, 8);

            oSmoulderingVillage = AddRoom("Smoldering Village", "Smoldering village");
            breeSewersGraph.Rooms[oSmoulderingVillage] = new PointF(1, -1);

            Room oFirePit = AddRoom("Fire Pit", "Fire Pit");
            AddExit(oSmoulderingVillage, oFirePit, "fire pit");
            AddExit(oFirePit, oSmoulderingVillage, "village");
            breeSewersGraph.Rooms[oFirePit] = new PointF(0, -1);

            Room oCeremonialPit = AddRoom("Ceremonial Pit", "Ceremonial Pit");
            e = AddExit(oFirePit, oCeremonialPit, "down");
            e.Hidden = true;
            AddExit(oCeremonialPit, oFirePit, "fire");
            breeSewersGraph.Rooms[oCeremonialPit] = new PointF(0, -0.5F);

            Room oMasterCeremonyRoom = AddRoom("Ceremony Room", "Master Ceremony Room");
            AddExit(oCeremonialPit, oMasterCeremonyRoom, "west");
            AddExit(oMasterCeremonyRoom, oCeremonialPit, "pit");
            AddExit(oMasterCeremonyRoom, oSmoulderingVillage, "village");
            breeSewersGraph.Rooms[oMasterCeremonyRoom] = new PointF(0, 0);

            Room oBurntHut = AddRoom("Burnt Hut", "Burnt Hut");
            AddBidirectionalExitsWithOut(oSmoulderingVillage, oBurntHut, "hut");
            breeSewersGraph.Rooms[oBurntHut] = new PointF(2, -1);

            Room oEaldsHideout = AddRoom("Eald the Wise", "Eald's Hideout");
            AddPermanentMobs(oEaldsHideout, MobTypeEnum.EaldTheWise);
            e = AddBidirectionalExitsWithOut(oBurntHut, oEaldsHideout, "sliding");
            e.Hidden = true;
            e.MustOpen = true;
            e = AddExit(oEaldsHideout, oBurnedRemainsOfNimrodel, "trap door");
            e.IsUnknownKnockableKeyType = true;
            AddExit(oBurnedRemainsOfNimrodel, oEaldsHideout, "up");
            breeSewersGraph.Rooms[oEaldsHideout] = new PointF(2, 0);

            Room oWell = AddRoom("Well", "Well");
            AddExit(oSmoulderingVillage, oWell, "well");
            AddExit(oWell, oSmoulderingVillage, "ladder");
            breeSewersGraph.Rooms[oWell] = new PointF(1, 0);

            Room oKasnarTheGuard = AddRoom("Kasnar", "Water Pipe");
            AddPermanentMobs(oKasnarTheGuard, MobTypeEnum.KasnarTheGuard);
            AddExit(oWell, oKasnarTheGuard, "pipe");
            AddExit(oKasnarTheGuard, oWell, "north");
            breeSewersGraph.Rooms[oKasnarTheGuard] = new PointF(1, 1);

            AddExit(aqueduct, oKasnarTheGuard, "north");
            e = AddExit(oKasnarTheGuard, aqueduct, "south");
            e.KeyType = SupportedKeysFlags.KasnarsRedKey;
            e.MustOpen = true;

            Room oOldMansReadingRoom = AddRoom("Reading Room", "Old man's reading room");
            AddBidirectionalSameNameExit(oBurnedRemainsOfNimrodel, oOldMansReadingRoom, "hallway");
            breeSewersGraph.Rooms[oOldMansReadingRoom] = new PointF(3, 0);
            //CSRTODO: safe
        }

        private void AddGridBidirectionalExits(Room[,] grid, int x, int y)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];
            Room r = grid[x, y];
            if (r != null)
            {
                float dX = x == 0 ? -2.25F : x;
                breeStreetsGraph.Rooms[r] = new PointF(dX, 10 - y);

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
            graphMillwoodMansion.Rooms[oPathToMansion2] = new PointF(1, 2);
            graphMillwoodMansion.Rooms[oConstructionSite] = new PointF(1, 1);
            breeStreetsGraph.Rooms[oPathToMansion2] = new PointF(11, 1.2F);
            AddMapBoundaryPoint(oConstructionSite, oPathToMansion2, MapType.BreeStreets, MapType.MillwoodMansion);

            Room oPathToMansion3 = AddRoom("The South Wall", "The South Wall");
            AddBidirectionalExits(oPathToMansion2, oPathToMansion3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion3] = new PointF(1, 3);

            Room oPathToMansion4WarriorBardsx2 = AddRoom("Warrior Bards (Path)", "Stone Path");
            AddPermanentMobs(oPathToMansion4WarriorBardsx2, MobTypeEnum.WarriorBard, MobTypeEnum.WarriorBard);
            AddExit(oPathToMansion3, oPathToMansion4WarriorBardsx2, "stone");
            AddExit(oPathToMansion4WarriorBardsx2, oPathToMansion3, "north");
            graphMillwoodMansion.Rooms[oPathToMansion4WarriorBardsx2] = new PointF(1, 4);

            Room oPathToMansion5 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion4WarriorBardsx2, oPathToMansion5, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oPathToMansion5] = new PointF(0, 5);

            Room oPathToMansion6 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion5, oPathToMansion6, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion6] = new PointF(0, 6);

            Room oPathToMansion7 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion6, oPathToMansion7, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oPathToMansion7] = new PointF(1, 7);

            Room oPathToMansion8 = AddRoom("Red Oak Tree", "Red Oak Tree");
            AddBidirectionalExits(oPathToMansion7, oPathToMansion8, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion8] = new PointF(1, 8);

            Room oPathToMansion9 = AddRoom("Red Oak Tree", "Red Oak Tree");
            AddBidirectionalExits(oPathToMansion8, oPathToMansion9, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oPathToMansion9] = new PointF(2, 9);

            Room oPathToMansion10 = AddRoom("Red Oak Tree", "Red Oak Tree");
            AddBidirectionalExits(oPathToMansion9, oPathToMansion10, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oPathToMansion10] = new PointF(1, 10);

            Room oPathToMansion11 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion10, oPathToMansion11, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion11] = new PointF(1, 11);

            Room oPathToMansion12 = AddRoom("Stone Path", "Stone Path");
            AddBidirectionalExits(oPathToMansion11, oPathToMansion12, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oPathToMansion12] = new PointF(2, 11);

            Room oGrandPorch = AddRoom("Warrior Bard (Porch)", "Grand Porch");
            AddPermanentMobs(oGrandPorch, MobTypeEnum.WarriorBard);
            AddExit(oPathToMansion12, oGrandPorch, "porch");
            AddExit(oGrandPorch, oPathToMansion12, "path");
            graphMillwoodMansion.Rooms[oGrandPorch] = new PointF(3, 11);

            Room oMansionInside1 = AddRoom("Mansion Inside", "Main Hallway");
            Exit e = AddExit(oGrandPorch, oMansionInside1, "door");
            e.MustOpen = true;
            e.MaximumLevel = 12;
            e = AddExit(oMansionInside1, oGrandPorch, "door");
            e.MustOpen = true;
            graphMillwoodMansion.Rooms[oMansionInside1] = new PointF(4, 11);

            Room oMansionInside2 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionInside1, oMansionInside2, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionInside2] = new PointF(5, 11);

            Room oMansionFirstFloorToNorthStairwell1 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell1, oMansionInside2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell1] = new PointF(5, 10);

            Room oMansionFirstFloorToNorthStairwell2 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell2, oMansionFirstFloorToNorthStairwell1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell2] = new PointF(5, 9);

            Room oMansionFirstFloorToNorthStairwell3 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell3, oMansionFirstFloorToNorthStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell3] = new PointF(5, 8);

            Room oMansionFirstFloorToNorthStairwell4 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell4] = new PointF(5, 7);

            Room oMansionFirstFloorToNorthStairwell5 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell5] = new PointF(6, 7);

            Room oWarriorBardMansionNorth = AddRoom("Stairwell Downstairs", "Northern Stairwell");
            AddPermanentMobs(oWarriorBardMansionNorth, MobTypeEnum.WarriorBard);
            AddBidirectionalExits(oWarriorBardMansionNorth, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oWarriorBardMansionNorth] = new PointF(6, 6);

            Room oMansionFirstFloorToSouthStairwell1 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToSouthStairwell1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell1] = new PointF(5, 12);

            Room oMansionFirstFloorToSouthStairwell2 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell1, oMansionFirstFloorToSouthStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell2] = new PointF(5, 13);

            Room oMansionFirstFloorToSouthStairwell3 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell2, oMansionFirstFloorToSouthStairwell3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell3] = new PointF(5, 14);

            Room oMansionFirstFloorToSouthStairwell4 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell3, oMansionFirstFloorToSouthStairwell4, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell4] = new PointF(5, 15);

            Room oMansionFirstFloorToSouthStairwell5 = AddRoom("Long Hallway", "Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell4, oMansionFirstFloorToSouthStairwell5, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell5] = new PointF(6, 15);

            Room oWarriorBardMansionSouth = AddRoom("Stairwell Downstairs", "Southern Stairwell");
            AddPermanentMobs(oWarriorBardMansionSouth, MobTypeEnum.WarriorBard);
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell5, oWarriorBardMansionSouth, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oWarriorBardMansionSouth] = new PointF(6, 16);

            Room oMansionFirstFloorToEastStairwell1 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToEastStairwell1, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell1] = new PointF(6, 11);

            Room oMansionFirstFloorToEastStairwell2 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell1, oMansionFirstFloorToEastStairwell2, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell2] = new PointF(7, 11);

            Room oMansionFirstFloorToEastStairwell3 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell2, oMansionFirstFloorToEastStairwell3, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell3] = new PointF(8, 11);

            Room oMansionFirstFloorToEastStairwell4 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell3, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell4] = new PointF(9, 11);

            Room oMansionFirstFloorToEastStairwell5 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell5] = new PointF(10, 10);

            Room oMansionFirstFloorToEastStairwell6 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell6] = new PointF(11, 11);

            Room oMansionFirstFloorToEastStairwell7 = AddRoom("Main Hallway", "Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell4, oMansionFirstFloorToEastStairwell7, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell6, oMansionFirstFloorToEastStairwell7, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell7] = new PointF(10, 12);

            Room oWarriorBardMansionEast = AddRoom("Grand Staircase", "Grand Staircase");
            AddPermanentMobs(oWarriorBardMansionEast, MobTypeEnum.WarriorBard);
            AddBidirectionalExits(oWarriorBardMansionEast, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oWarriorBardMansionEast] = new PointF(10, 11);

            Room oNorthHallway1 = AddRoom("North Hallway", "North Hallway");
            AddBidirectionalExits(oNorthHallway1, oMansionFirstFloorToEastStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway1] = new PointF(7, 10);

            Room oNorthHallway2 = AddRoom("North Hallway", "North Hallway");
            AddBidirectionalExits(oNorthHallway2, oNorthHallway1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway2] = new PointF(7, 9);

            Room oNorthHallway3 = AddRoom("North Hallway", "North Hallway");
            AddBidirectionalExits(oNorthHallway3, oNorthHallway2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway3] = new PointF(7, 8);

            Room oDungeonGuardNorth = AddRoom("Dungeon Guard", "North Hallway");
            AddPermanentMobs(oDungeonGuardNorth, MobTypeEnum.DungeonGuard);
            AddBidirectionalExits(oNorthHallway3, oDungeonGuardNorth, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oDungeonGuardNorth] = new PointF(8, 8);

            Room oSouthHallway1 = AddRoom("South Hallway", "South Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell2, oSouthHallway1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway1] = new PointF(7, 12);

            Room oSouthHallway2 = AddRoom("South Hallway", "South Hallway");
            AddBidirectionalExits(oSouthHallway1, oSouthHallway2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway2] = new PointF(7, 13);

            Room oSouthHallway3 = AddRoom("South Hallway", "South Hallway");
            AddBidirectionalExits(oSouthHallway2, oSouthHallway3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway3] = new PointF(7, 14);

            Room oDungeonGuardSouth = AddRoom("Dungeon Guard", "South Hallway");
            AddPermanentMobs(oDungeonGuardSouth, MobTypeEnum.DungeonGuard);
            AddBidirectionalExits(oSouthHallway3, oDungeonGuardSouth, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oDungeonGuardSouth] = new PointF(8, 14);

            AddMillwoodMansionUpstairs(oWarriorBardMansionNorth, oWarriorBardMansionSouth, oWarriorBardMansionEast);
        }

        private void AddMillwoodMansionUpstairs(Room northStairwell, Room southStairwell, Room eastStairwell)
        {
            RoomGraph millwoodMansionUpstairsGraph = _graphs[MapType.MillwoodMansionUpstairs];
            RoomGraph millwoodMansionGraph = _graphs[MapType.MillwoodMansion];

            millwoodMansionUpstairsGraph.Rooms[northStairwell] = new PointF(1, 0);
            millwoodMansionUpstairsGraph.Rooms[southStairwell] = new PointF(1, 12);
            millwoodMansionUpstairsGraph.Rooms[eastStairwell] = new PointF(5, 5);

            Room oGrandStaircaseUpstairs = AddRoom("Grand Staircase", "Grand Staircase");
            AddBidirectionalExits(oGrandStaircaseUpstairs, eastStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oGrandStaircaseUpstairs] = new PointF(5, 6);
            millwoodMansionGraph.Rooms[oGrandStaircaseUpstairs] = new PointF(10, 10.5F);
            AddMapBoundaryPoint(eastStairwell, oGrandStaircaseUpstairs, MapType.MillwoodMansion, MapType.MillwoodMansionUpstairs);

            Room oRoyalHallwayUpstairs = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oGrandStaircaseUpstairs, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayUpstairs] = new PointF(4, 6);

            Room oRoyalHallwayToMayor = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oRoyalHallwayToMayor, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayToMayor] = new PointF(4, 7);

            Room oRoyalHallwayToChancellor = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayToChancellor, oRoyalHallwayUpstairs, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayToChancellor] = new PointF(4, 5);

            Room oRoyalHallway1 = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallway1, oRoyalHallwayUpstairs, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway1] = new PointF(3, 6);

            Room oRoyalHallway2 = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallway2, oRoyalHallway1, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway2] = new PointF(2, 6);

            Room oRoyalHallway3 = AddRoom("Royal Hallway", "Royal Hallway");
            AddBidirectionalExits(oRoyalHallway3, oRoyalHallway2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway3] = new PointF(1, 6);

            Room oNorthCorridor1 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor1, oRoyalHallway3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor1] = new PointF(1, 5);

            Room oNorthCorridor2 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor2, oNorthCorridor1, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor2] = new PointF(1, 4);

            Room oDiningArea = AddRoom("Dining Area", "Dining Area");
            AddBidirectionalExits(oDiningArea, oNorthCorridor2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oDiningArea] = new PointF(0, 4);

            Room oNorthCorridor3 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor3, oNorthCorridor2, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor3] = new PointF(1, 3);

            Room oNorthCorridor4 = AddRoom("North Corridor", "North Corridor");
            AddBidirectionalExits(oNorthCorridor4, oNorthCorridor3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor4] = new PointF(1, 2);

            Room oMeditationChamber = AddHealingRoom("Meditation Chamber", "Meditation Chamber", HealingRoom.MillwoodMansion);
            AddBidirectionalExitsWithOut(oNorthCorridor4, oMeditationChamber, "door", true);
            millwoodMansionUpstairsGraph.Rooms[oMeditationChamber] = new PointF(0, 2);

            Room oNorthernStairwell = AddRoom("Northern Stairwell", "Northern Stairwell");
            AddBidirectionalExits(oNorthernStairwell, oNorthCorridor4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthernStairwell, northStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oNorthernStairwell] = new PointF(1, 1);
            millwoodMansionGraph.Rooms[oNorthernStairwell] = new PointF(7, 6);
            AddMapBoundaryPoint(northStairwell, oNorthernStairwell, MapType.MillwoodMansion, MapType.MillwoodMansionUpstairs);

            Room oSouthCorridor1 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oRoyalHallway3, oSouthCorridor1, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor1] = new PointF(1, 7);

            Room oSouthCorridor2 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oSouthCorridor1, oSouthCorridor2, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor2] = new PointF(1, 8);

            Room oKnightsQuarters = AddRoom("Knights' Quarters", "Knights' Quarters");
            AddBidirectionalExits(oKnightsQuarters, oSouthCorridor2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oKnightsQuarters] = new PointF(0, 8);

            Room oSouthCorridor3 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oSouthCorridor2, oSouthCorridor3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor3] = new PointF(1, 9);

            Room oSouthCorridor4 = AddRoom("South Corridor", "South Corridor");
            AddBidirectionalExits(oSouthCorridor3, oSouthCorridor4, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor4] = new PointF(1, 10);

            Room oStorageRoom = AddRoom("Storage Room", "Storage Room");
            AddBidirectionalExitsWithOut(oSouthCorridor4, oStorageRoom, "door", true);
            millwoodMansionUpstairsGraph.Rooms[oStorageRoom] = new PointF(0, 10);

            Room oSouthernStairwell = AddRoom("Southern Stairwell", "Southern Stairwell");
            AddBidirectionalExits(oSouthCorridor4, oSouthernStairwell, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSouthernStairwell, southStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oSouthernStairwell] = new PointF(1, 11);
            millwoodMansionGraph.Rooms[oSouthernStairwell] = new PointF(7, 16);
            AddMapBoundaryPoint(southStairwell, oSouthernStairwell, MapType.MillwoodMansion, MapType.MillwoodMansionUpstairs);

            Room oMayorMillwood = AddRoom("Mayor Millwood", "Royal Chamber");
            AddPermanentMobs(oMayorMillwood, MobTypeEnum.MayorMillwood);
            AddBidirectionalExitsWithOut(oRoyalHallwayToMayor, oMayorMillwood, "chamber", true);
            millwoodMansionUpstairsGraph.Rooms[oMayorMillwood] = new PointF(4, 8);

            Room oChancellorOfProtection = AddRoom("Chancellor of Protection", "The Chancellor of Protection's Chambers");
            AddPermanentMobs(oChancellorOfProtection, MobTypeEnum.ChancellorOfProtection);
            AddBidirectionalExitsWithOut(oRoyalHallwayToChancellor, oChancellorOfProtection, "chamber", true);
            millwoodMansionUpstairsGraph.Rooms[oChancellorOfProtection] = new PointF(4, 4);
        }

        private void AddBreeToImladris(out Room oOuthouse, Room breeEastGateInside, Room breeEastGateOutside, out Room imladrisWestGateOutside, Room oCemetery, out Room southernBrethilNWEdge, out Room southernBrethilForestSW, out Room southernBrethilForestSE, out Room southernBrethilForestNE, out Room smugglersVillage2)
        {
            RoomGraph breeToImladrisGraph = _graphs[MapType.BreeToImladris];

            breeToImladrisGraph.Rooms[breeEastGateInside] = new PointF(-4, 4);
            breeToImladrisGraph.Rooms[oCemetery] = new PointF(-4, 3);

            AddExit(breeEastGateInside, breeEastGateOutside, "gate");
            breeToImladrisGraph.Rooms[breeEastGateOutside] = new PointF(-3, 4);
            Exit e = AddExit(breeEastGateOutside, breeEastGateInside, "gate");
            e.RequiresDay = true;
            AddMapBoundaryPoint(breeEastGateInside, breeEastGateOutside, MapType.BreeStreets, MapType.BreeToImladris);

            Room oGreatEastRoad1 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(breeEastGateOutside, oGreatEastRoad1, BidirectionalExitType.WestEast);
            AddToFarmHouseAndUglies(oGreatEastRoad1, out oOuthouse, breeToImladrisGraph);
            breeToImladrisGraph.Rooms[oGreatEastRoad1] = new PointF(-2, 4);

            Room oGreatEastRoad2 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad1, oGreatEastRoad2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad2] = new PointF(5, 4);

            Room oGreatEastRoad3 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad2, oGreatEastRoad3, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad3] = new PointF(6, 4);

            AddGalbasiDowns(oGreatEastRoad2, oGreatEastRoad3, breeToImladrisGraph);

            Room oGreatEastRoad4 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad3, oGreatEastRoad4, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad4] = new PointF(7, 4);

            Room oGreatEastRoad5 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad4, oGreatEastRoad5, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad5] = new PointF(8, 4);

            Room oNorthOfGreatEastRoadUnknown = AddRoom("North of Road Unknown", Room.UNKNOWN_ROOM);
            e = AddExit(oGreatEastRoad5, oNorthOfGreatEastRoadUnknown, "north");
            e.MinimumLevel = 15;
            breeToImladrisGraph.Rooms[oNorthOfGreatEastRoadUnknown] = new PointF(8, 3);

            Room oGreatEastRoad6 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad5, oGreatEastRoad6, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad6] = new PointF(9, 4);

            Room oGreatEastRoadGoblinAmbushGobLrgLrg = AddRoom("Gob Ambush #1", "Great East Road");
            AddPermanentMobs(oGreatEastRoadGoblinAmbushGobLrgLrg, MobTypeEnum.Goblin, MobTypeEnum.LargeGoblin, MobTypeEnum.LargeGoblin);
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad6, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oGreatEastRoadGoblinAmbushGobLrgLrg] = new PointF(10, 3);

            Room oGreatEastRoad8 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad8, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oGreatEastRoad8] = new PointF(11, 4);

            Room oGreatEastRoad9 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad8, oGreatEastRoad9, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad9] = new PointF(12, 4);

            Room oGreatEastRoad10 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad9, oGreatEastRoad10, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad10] = new PointF(13, 4);

            Room oGreatEastRoad11 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad10, oGreatEastRoad11, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad11] = new PointF(14, 4);

            Room oGreatEastRoad12 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad11, oGreatEastRoad12, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad12] = new PointF(15, 4);

            Room oPathThroughForest = AddRoom("Forest Path", "Path through Forest");
            AddBidirectionalExits(oGreatEastRoad12, oPathThroughForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oPathThroughForest] = new PointF(15, 5);

            Room oGreatEastRoad13 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad12, oGreatEastRoad13, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad13] = new PointF(16, 4);

            Room oGreatEastRoad14 = AddRoom("Great East Road", "Great East Road");
            AddBidirectionalExits(oGreatEastRoad13, oGreatEastRoad14, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad14] = new PointF(17, 4);

            Room oThickFog = AddRoom("Thick Fog", "Thick Fog");
            AddExit(oGreatEastRoad14, oThickFog, "north");
            e = AddExit(oThickFog, oGreatEastRoad12, "east");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oThickFog] = new PointF(17, 3);

            imladrisWestGateOutside = AddRoom("West Gate Outside", "West Gate of Imladris");
            AddBidirectionalExits(oGreatEastRoad14, imladrisWestGateOutside, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[imladrisWestGateOutside] = new PointF(18, 4);

            Room oNorthBrethilForest1 = AddRoom("Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oGreatEastRoadGoblinAmbushGobLrgLrg, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest1] = new PointF(10, 2);

            Room oNorthBrethilForest2 = AddRoom("Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oNorthBrethilForest2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest2] = new PointF(11, 2);

            Room oDarkFootpath = AddRoom("Dark Footpath", "Dark Footpath");
            e = AddExit(oGreatEastRoad10, oDarkFootpath, "north");
            e.Hidden = true;
            AddExit(oDarkFootpath, oGreatEastRoad10, "south");
            AddExit(oNorthBrethilForest2, oDarkFootpath, "east");
            e = AddExit(oDarkFootpath, oNorthBrethilForest2, "west");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oDarkFootpath] = new PointF(13, 2);

            Room oNorthBrethilForest3 = AddRoom("Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest3, oDarkFootpath, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest3] = new PointF(13, 1);

            Room oNorthBrethilForest4 = AddRoom("Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest4, oNorthBrethilForest3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest4] = new PointF(13, 0);

            Room oNorthBrethilForest5GobAmbush = AddRoom("Gob Ambush #2", "North Brethil Forest");
            AddPermanentMobs(oNorthBrethilForest5GobAmbush, MobTypeEnum.GoblinWarrior, MobTypeEnum.GoblinWarrior, MobTypeEnum.LargeGoblin);
            AddBidirectionalExits(oNorthBrethilForest4, oNorthBrethilForest5GobAmbush, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oNorthBrethilForest5GobAmbush, oNorthBrethilForest3, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest5GobAmbush] = new PointF(14, 0);

            Room oNorthBrethilForest5 = AddRoom("Forest", "North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest5GobAmbush, oNorthBrethilForest5, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthBrethilForest4, oNorthBrethilForest5, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oNorthBrethilForest3, oNorthBrethilForest5, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest5] = new PointF(14, 1);

            Room oEntranceChamber = AddRoom("Entrance Chamber", "Entrance Chamber");
            e = AddExit(oNorthBrethilForest5GobAmbush, oEntranceChamber, "hole");
            e.Hidden = true;
            AddExit(oEntranceChamber, oNorthBrethilForest5GobAmbush, "up");
            breeToImladrisGraph.Rooms[oEntranceChamber] = new PointF(15, 0);

            Room oRadbug = AddRoom("Radbug", "Tunnel Intersection");
            AddPermanentMobs(oRadbug, MobTypeEnum.Radbug);
            AddBidirectionalExits(oEntranceChamber, oRadbug, BidirectionalExitType.UpDown);
            breeToImladrisGraph.Rooms[oRadbug] = new PointF(16, 0);

            //South Brethil Forest
            Room oDeepForest1 = AddRoom("Deep Forest", "Deep Forest");
            e = AddExit(oGreatEastRoad9, oDeepForest1, "south");
            e.Hidden = true;
            AddExit(oDeepForest1, oGreatEastRoad9, "north");
            breeToImladrisGraph.Rooms[oDeepForest1] = new PointF(12, 4.5F);

            Room oNathalin = AddRoom("Nathalin", "Trading Post");
            Trades[ItemTypeEnum.Diamond] = MobTypeEnum.NathalinTheTrader; //mask of distortion
            Trades[ItemTypeEnum.GoblinBlade] = MobTypeEnum.NathalinTheTrader; //platinum ring
            Trades[ItemTypeEnum.Emerald] = MobTypeEnum.NathalinTheTrader; //flint blade
            //CSRTODO: sprite sword
            AddPermanentMobs(oNathalin, MobTypeEnum.NathalinTheTrader);
            AddBidirectionalExitsWithOut(oDeepForest1, oNathalin, "tree");
            breeToImladrisGraph.Rooms[oNathalin] = new PointF(13, 4.5F);

            Room oDeepForest2 = AddRoom("Deep Forest", "Deep Forest");
            e = AddExit(oDeepForest1, oDeepForest2, "west");
            e.Hidden = true;
            AddExit(oDeepForest2, oDeepForest1, "east");
            breeToImladrisGraph.Rooms[oDeepForest2] = new PointF(11, 4.5F);

            Room oSkeletalOak = AddRoom("Skeletal Oak", Room.UNKNOWN_ROOM);
            e = AddExit(oDeepForest2, oSkeletalOak, "skeletal oak");
            e.MinimumLevel = 15;
            breeToImladrisGraph.Rooms[oSkeletalOak] = new PointF(11, 4.25F);

            Room oBrethilForest = AddRoom("Brethil Forest", "Brethil Forest");
            AddPermanentItems(oBrethilForest, ItemTypeEnum.PipeWeed);
            AddBidirectionalExits(oDeepForest1, oBrethilForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oBrethilForest] = new PointF(12, 6);

            Room oSpriteGuards = AddRoom("Sprite Guards", "Brethil Forest");
            AddPermanentMobs(oSpriteGuards, MobTypeEnum.SpriteGuard, MobTypeEnum.SpriteGuard);
            e = AddExit(oBrethilForest, oSpriteGuards, "brush");
            e.Hidden = true;
            AddExit(oSpriteGuards, oBrethilForest, "east");
            breeToImladrisGraph.Rooms[oSpriteGuards] = new PointF(10, 6);

            Room oSpriteForest = AddRoom("Sprite Forest", "Sprite Forest");
            AddBidirectionalExits(oSpriteForest, oSpriteGuards, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oSpriteForest] = new PointF(10, 5.5F);

            Room oHirluinsClearing = AddRoom("Hirluin's Clearing", "Hirluin's Clearing");
            AddBidirectionalExits(oHirluinsClearing, oSpriteForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oHirluinsClearing] = new PointF(10, 5);

            Room oDarkForest1 = AddRoom("Dark Forest", "Dark Forest");
            AddBidirectionalExits(oDarkForest1, oSpriteForest, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oDarkForest1, oHirluinsClearing, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDarkForest1] = new PointF(9, 5);

            Room oDarkForest2 = AddRoom("Dark Forest", "Dark Forest");
            AddBidirectionalExits(oDarkForest2, oSpriteForest, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oHirluinsClearing, oDarkForest2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDarkForest2] = new PointF(11, 5);

            Room oDarkForest3 = AddRoom("Dark Forest", "Dark Forest");
            AddBidirectionalExits(oDarkForest3, oDarkForest1, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oDarkForest3, oHirluinsClearing, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDarkForest3, oDarkForest2, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oDarkForest3] = new PointF(10, 4.5F);

            Room oOvergrownPath = AddRoom("Overgrown Path", "Overgrown Path");
            AddBidirectionalExits(oBrethilForest, oOvergrownPath, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oOvergrownPath] = new PointF(12, 7);

            Room oDarkTree = AddRoom("Dark Tree", "Dark Tree");
            AddPermanentMobs(oDarkTree, MobTypeEnum.HerbDealer);
            e = AddBidirectionalExitsWithOut(oOvergrownPath, oDarkTree, "tree");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oDarkTree] = new PointF(13, 6);

            Room oDirtPath = AddRoom("Dirt Path", "Dirt Path");
            AddBidirectionalExits(oOvergrownPath, oDirtPath, BidirectionalExitType.WestEast);
            AddExit(oPathThroughForest, oDirtPath, "forest");
            AddExit(oDirtPath, oPathThroughForest, "northeast");
            breeToImladrisGraph.Rooms[oDirtPath] = new PointF(13, 7);

            Room oForest = AddRoom("Forest", "Brethil Forest");
            AddBidirectionalExits(oDarkForest1, oForest, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oForest] = new PointF(8.5F, 5.5F);

            Room oForest2 = AddRoom("Forest", "Brethil Forest");
            AddBidirectionalExits(oForest, oForest2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oForest2] = new PointF(8.5F, 6);

            southernBrethilForestNE = AddRoom("Forest", "Southern Brethil Forest");
            AddExit(southernBrethilForestNE, oForest, "east");
            breeToImladrisGraph.Rooms[southernBrethilForestNE] = new PointF(7.75F, 5.5F);

            southernBrethilForestSE = AddRoom("Forest", "Southern Brethil Forest");
            AddBidirectionalExits(southernBrethilForestNE, southernBrethilForestSE, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[southernBrethilForestSE] = new PointF(7.75F, 6);

            southernBrethilNWEdge = AddRoom("Forest", "Southern Brethil Forest");
            AddBidirectionalExits(southernBrethilNWEdge, southernBrethilForestNE, BidirectionalExitType.WestEast);
            AddExit(oForest2, southernBrethilNWEdge, "west");
            breeToImladrisGraph.Rooms[southernBrethilNWEdge] = new PointF(7, 5.5F);

            southernBrethilForestSW = AddRoom("Forest", "Southern Brethil Forest");
            AddBidirectionalExits(southernBrethilNWEdge, southernBrethilForestSW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(southernBrethilForestSW, southernBrethilForestSE, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[southernBrethilForestSW] = new PointF(7, 6);

            Room oForestEdge = AddRoom("Forest Edge", "Edge of the Brethil Forest");
            AddBidirectionalExits(oForestEdge, southernBrethilNWEdge, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oForestEdge] = new PointF(6.25F, 5.5F);

            Room oDarkForest = AddRoom("Dark Forest", "Dark Forest");
            AddBidirectionalExits(oForest2, oDarkForest, BidirectionalExitType.SouthwestNortheast);
            AddExit(oForest, oDarkForest, "northwest");
            AddExit(oDarkForest, southernBrethilNWEdge, "west");
            AddExit(oDarkForest, southernBrethilForestSW, "south");
            AddExit(oDarkForest, southernBrethilForestSW, "southwest");
            breeToImladrisGraph.Rooms[oDarkForest] = new PointF(7.75F, 6.5F);

            Room oGrasslands1 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oForestEdge, oGrasslands1, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oGrasslands1] = new PointF(5.75F, 6);

            Room oGrasslands2 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands2, oGrasslands1, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGrasslands2] = new PointF(5, 6);

            Room oGrasslands3 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands3, oGrasslands2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGrasslands3] = new PointF(4.25F, 6);

            Room oGrasslands4 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands1, oGrasslands4, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGrasslands4] = new PointF(5.75F, 6.5F);

            Room oGrasslands5 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands5, oGrasslands4, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oGrasslands2, oGrasslands5, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGrasslands5] = new PointF(5, 6.5F);

            Room oGrasslands6 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands6, oGrasslands5, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oGrasslands3, oGrasslands6, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGrasslands6] = new PointF(4.25F, 6.5F);

            Room oGrasslands7 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands4, oGrasslands7, BidirectionalExitType.NorthSouth);
            AddExit(oGrasslands4, oGrasslands7, "east");
            AddExit(oGrasslands7, oGrasslands4, "east");
            breeToImladrisGraph.Rooms[oGrasslands7] = new PointF(5.75F, 7);

            Room oGrasslands8 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands8, oGrasslands7, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oGrasslands5, oGrasslands8, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGrasslands8] = new PointF(5, 7);

            Room oGrasslands9 = AddRoom("Grassland", "Grasslands");
            AddBidirectionalExits(oGrasslands9, oGrasslands8, BidirectionalExitType.WestEast);
            AddExit(oGrasslands9, oGrasslands6, "north");
            breeToImladrisGraph.Rooms[oGrasslands9] = new PointF(4.25F, 7);

            Room oTallGrass = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass, oGrasslands9, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oTallGrass] = new PointF(3.5F, 7);

            Room oTallGrass2 = AddRoom("Venus Fly Trap", "Tall Grass");
            AddPermanentMobs(oTallGrass2, MobTypeEnum.GiantVenusFlyTrap);
            AddBidirectionalExits(oTallGrass, oTallGrass2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oTallGrass2] = new PointF(3.5F, 7.5F);

            Room oTallGrass3 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass3, oTallGrass, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oTallGrass3] = new PointF(2.75F, 7);

            Room oTallGrass4 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass3, oTallGrass4, BidirectionalExitType.NorthSouth);
            AddExit(oTallGrass4, oTallGrass2, "east");
            breeToImladrisGraph.Rooms[oTallGrass4] = new PointF(2.75F, 7.5F);

            Room oTallGrass5 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass5, oTallGrass4, BidirectionalExitType.WestEast);
            AddExit(oTallGrass5, oTallGrass4, "west");
            breeToImladrisGraph.Rooms[oTallGrass5] = new PointF(2, 7.5F);

            Room oTallGrass6 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass6, oTallGrass5, BidirectionalExitType.NorthSouth);
            AddExit(oTallGrass6, oTallGrass4, "east");
            breeToImladrisGraph.Rooms[oTallGrass6] = new PointF(2, 7);

            Room oTallGrass7 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass7, oTallGrass6, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oTallGrass7] = new PointF(1.25F, 7);

            Room oTallGrass8 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass5, oTallGrass8, BidirectionalExitType.NorthSouth);
            AddExit(oTallGrass8, oTallGrass2, "east");
            breeToImladrisGraph.Rooms[oTallGrass8] = new PointF(2, 8);

            Room oTallGrass9 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrass7, oTallGrass9, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oTallGrass9] = new PointF(1.25F, 7.5F);

            Room oShortGrass1 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oTallGrass9, oShortGrass1, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oShortGrass1] = new PointF(1.25F, 8);

            Room oShortGrass2 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass1, oShortGrass2, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oShortGrass2] = new PointF(0.5F, 8.25F);

            Room oShortGrass3 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass2, oShortGrass3, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrass3] = new PointF(1.25F, 8.5F);

            Room oShortGrass4 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass3, oShortGrass4, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oShortGrass4] = new PointF(0.5F, 8.75F);

            Room oShortGrass5 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass4, oShortGrass5, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrass5] = new PointF(1.25F, 9);

            Room oShortGrass6 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass5, oShortGrass6, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oShortGrass6] = new PointF(0.5F, 9.25F);

            Room oShortGrass7 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass6, oShortGrass7, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrass7] = new PointF(1.25F, 9.5F);

            Room oShortGrass8 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass7, oShortGrass8, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oShortGrass8] = new PointF(0.5F, 10);

            Room oShortGrass9 = AddRoom("Short Grass", "Short Grass");
            breeToImladrisGraph.Rooms[oShortGrass9] = new PointF(-0.25F, 10.5F);

            Room oShortGrass10 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrass9, oShortGrass10, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrass10] = new PointF(0.5F, 10.75F);

            Room oNorthEdgeOfErynVorn = AddRoom("Eryn Vorn North", "North Edge of Eryn Vorn");
            AddBidirectionalExits(oShortGrass10, oNorthEdgeOfErynVorn, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oNorthEdgeOfErynVorn] = new PointF(1.25F, 11);

            Room oRiverBank1 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank1, oShortGrass2, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oRiverBank1] = new PointF(-0.25F, 8);

            Room oRiverBank2 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oShortGrass2, oRiverBank2, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oRiverBank1, oRiverBank2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank2] = new PointF(-0.25F, 8.375F);

            Room oRiverBank3 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank2, oRiverBank3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oRiverBank3, oShortGrass4, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oRiverBank3] = new PointF(-0.25F, 8.625F);

            Room oRiverBank4 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank3, oRiverBank4, BidirectionalExitType.SouthwestNortheast);
            AddExit(oRiverBank4, oShortGrass4, "east");
            AddExit(oShortGrass4, oRiverBank4, "southwest");
            breeToImladrisGraph.Rooms[oRiverBank4] = new PointF(-1, 8.75F);

            Room oRiverBank5 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank4, oRiverBank5, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oRiverBank5, oShortGrass6, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oRiverBank5] = new PointF(-1, 9);

            Room oRiverBank6 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank5, oRiverBank6, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oShortGrass6, oRiverBank6, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oRiverBank6] = new PointF(-1, 9.5F);

            Room oRiverBank7 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank6, oRiverBank7, BidirectionalExitType.SoutheastNorthwest);
            AddExit(oRiverBank7, oShortGrass6, "southeast");
            AddExit(oShortGrass8, oRiverBank7, "northwest");
            breeToImladrisGraph.Rooms[oRiverBank7] = new PointF(-0.25F, 9.75F);

            Room oRiverBank8 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank7, oRiverBank8, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oShortGrass8, oRiverBank8, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oRiverBank8] = new PointF(-0.25F, 10.25F);

            Room oRiverBank9 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank8, oRiverBank9, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oRiverBank9, oShortGrass9, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oRiverBank9] = new PointF(-1, 10.5F);

            Room oRiverBank10 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank9, oRiverBank10, BidirectionalExitType.SouthwestNortheast);
            AddExit(oShortGrass9, oRiverBank10, "southwest");
            breeToImladrisGraph.Rooms[oRiverBank10] = new PointF(-1.75F, 10.75F);

            Room oRiverBank11 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank10, oRiverBank11, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank11] = new PointF(-1.75F, 11);

            Room oRiverBank12 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank11, oRiverBank12, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oRiverBank12] = new PointF(-1.75F, 11.25F);

            Room oBrandywineEstuary = AddRoom("Estuary", "Brandywine Estuary");
            oBrandywineEstuary.DamageType = RoomDamageType.Water;
            AddBidirectionalExits(oRiverBank12, oBrandywineEstuary, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oBrandywineEstuary] = new PointF(-2.5F, 11.5F);

            Room oHiddenCave = AddRoom("Hidden Cave", "Hidden Cave");
            AddPermanentMobs(oHiddenCave, MobTypeEnum.GiantOctopus);
            AddPermanentItems(oHiddenCave, ItemTypeEnum.PlatinumConch);
            e = AddBidirectionalExitsWithOut(oBrandywineEstuary, oHiddenCave, "cave");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oHiddenCave] = new PointF(-2.5F, 11.75F);

            Room oBrandywineShore = AddRoom("Shore", "Brandywine Shore");
            AddBidirectionalExits(oBrandywineShore, oBrandywineEstuary, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oBrandywineShore] = new PointF(-3.25F, 11.25F);

            Room oWesternShore1 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore1, oBrandywineShore, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oWesternShore1] = new PointF(-4, 11);

            Room oWesternShore2 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore2, oWesternShore1, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oWesternShore2] = new PointF(-4.75F, 10.75F);

            Room oWesternShore3 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore3, oWesternShore2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oWesternShore3] = new PointF(-4.75F, 10.5F);

            Room oWesternShore4 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore4, oWesternShore3, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oWesternShore4] = new PointF(-5.5F, 10.25F);

            Room oWesternShore5 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore5, oWesternShore4, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oWesternShore5] = new PointF(-6.25F, 10);

            Room oWesternShore6 = AddRoom("Western Shore", "Western Shore");
            //AddExit(oWesternShore5, oWesternShore6, "northwest"); //this exit goes to either western shore 6 or western shore 9
            AddExit(oWesternShore6, oWesternShore5, "southeast");
            breeToImladrisGraph.Rooms[oWesternShore6] = new PointF(-7, 9.75F);

            Room oWesternShore7 = AddRoom("Western Shore", "Western Shore");
            AddPermanentMobs(oWesternShore7, MobTypeEnum.GreatSpider);
            AddBidirectionalExits(oWesternShore7, oWesternShore6, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oWesternShore7] = new PointF(-7, 9.5F);

            Room oSpiderWeb = AddRoom("Spider Web", "Spider Web");
            AddBidirectionalExitsWithOut(oWesternShore7, oSpiderWeb, "web");
            breeToImladrisGraph.Rooms[oSpiderWeb] = new PointF(-6.25F, 9.5F);

            Room oWesternShore8 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore8, oWesternShore7, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oWesternShore8] = new PointF(-7.75F, 9.25F);

            Room oWesternShore9 = AddRoom("Western Shore", "Western Shore");
            AddExit(oWesternShore8, oWesternShore9, "northwest");
            //AddExit(oWesternShore9, oWesternShore8, "southeast"); //this exit goes to either western shore 5 or western shore 8
            breeToImladrisGraph.Rooms[oWesternShore9] = new PointF(-8.5F, 9);

            Room oWesternShore10 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore10, oWesternShore9, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oWesternShore10] = new PointF(-8.5F, 8.75F);

            Room oWesternShore11 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore11, oWesternShore10, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oWesternShore11] = new PointF(-9.25F, 8.5F);

            Room oWesternShore12 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore12, oWesternShore11, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oWesternShore12] = new PointF(-8.5F, 8.25F);

            Room oWesternShore13 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore13, oWesternShore12, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oWesternShore13] = new PointF(-9.25F, 8);

            Room oWesternShore14 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore14, oWesternShore13, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oWesternShore14] = new PointF(-9.25F, 7.75F);

            Room oWesternShore15 = AddRoom("Western Shore", "Western Shore");
            AddBidirectionalExits(oWesternShore15, oWesternShore14, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oWesternShore15] = new PointF(-10, 7.5F);

            Room oSmugglersCove = AddRoom("Smuggler's Cove", "Smuggler's Cove");
            AddBidirectionalExits(oSmugglersCove, oWesternShore15, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oSmugglersCove] = new PointF(-10.75F, 7.25F);

            Room oSmugglersVillage = AddRoom("Smuggler's Village", "Smuggler's Village");
            AddBidirectionalExits(oSmugglersVillage, oSmugglersCove, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oSmugglersVillage] = new PointF(-10.75F, 7);

            Room oWillingWench = AddRoom("Willing Wench", "Willing Wench");
            AddExit(oSmugglersVillage, oWillingWench, "shack");
            AddExit(oWillingWench, oSmugglersVillage, "door");
            breeToImladrisGraph.Rooms[oWillingWench] = new PointF(-9.75F, 7);

            Room oAsimele = AddRoom("Asimele", "Asimele's Quarters");
            AddPermanentMobs(oAsimele, MobTypeEnum.AsimeleThePleasurePriestess);
            AddBidirectionalExitsWithOut(oWillingWench, oAsimele, "back");
            breeToImladrisGraph.Rooms[oAsimele] = new PointF(-8.75F, 7);

            smugglersVillage2 = AddRoom("Smuggler's Village", "Smuggler's Village");
            AddBidirectionalExits(smugglersVillage2, oSmugglersVillage, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[smugglersVillage2] = new PointF(-11.5F, 6.75F);

            Room oStorageShed = AddRoom("Storage Shed", "Storage Shed");
            AddPermanentMobs(oStorageShed, MobTypeEnum.Cat);
            AddBidirectionalExitsWithOut(smugglersVillage2, oStorageShed, "shed");
            breeToImladrisGraph.Rooms[oStorageShed] = new PointF(-12.25F, 7);

            Room oSlashedSail = AddRoom("Slashed Sail", "The Slashed Sail");
            AddBidirectionalExitsWithOut(smugglersVillage2, oSlashedSail, "tavern");
            breeToImladrisGraph.Rooms[oSlashedSail] = new PointF(-10.5F, 6.75F);

            Room oRiverBank13 = AddRoom("River Bank", "River Bank");
            AddExit(oBrandywineShore, oRiverBank13, "northeast");
            AddExit(oRiverBank13, oBrandywineEstuary, "southwest");
            breeToImladrisGraph.Rooms[oRiverBank13] = new PointF(-2.5F, 11);

            Room oRiverBank14 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank14, oRiverBank13, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank14] = new PointF(-2.5F, 10.75F);

            Room oRiverBank15 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank15, oRiverBank14, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank15] = new PointF(-2.5F, 10.5F);

            Room oRiverBank16 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank16, oRiverBank15, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank16] = new PointF(-2.5F, 10.25F);

            Room oRiverBank17 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank17, oRiverBank16, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank17] = new PointF(-2.5F, 10);

            Room oRiverBank18 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank18, oRiverBank17, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank18] = new PointF(-2.5F, 9.75F);

            Room oRiverBank19 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank19, oRiverBank18, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank19] = new PointF(-2.5F, 9.5F);

            Room oRiverBank20 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank20, oRiverBank19, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank20] = new PointF(-2.5F, 9.25F);

            Room oRiverBank21 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank21, oRiverBank20, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank21] = new PointF(-2.5F, 9);

            Room oRiverBank22 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank22, oRiverBank21, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank22] = new PointF(-2.5F, 8.75F);

            Room oRiverBank23 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank23, oRiverBank22, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank23] = new PointF(-2.5F, 8.5F);

            Room oRiverBank24 = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank24, oRiverBank23, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRiverBank24] = new PointF(-2.5F, 8.25F);

            Room oShortGrassWest1 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oRiverBank17, oShortGrassWest1, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oShortGrassWest1, oRiverBank16, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrassWest1] = new PointF(-3.25F, 10.125F);

            Room oShortGrassWest2 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrassWest1, oShortGrassWest2, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oShortGrassWest2] = new PointF(-4, 10.375F);

            Room oShortGrassWest3 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrassWest3, oShortGrassWest1, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrassWest3] = new PointF(-4, 9.875F);

            Room oShortGrassWest4 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrassWest4, oShortGrassWest3, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oShortGrassWest4, oRiverBank18, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oRiverBank19, oShortGrassWest4, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oShortGrassWest4] = new PointF(-3.25F, 9.625F);

            Room oShortGrassWest5 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrassWest5, oShortGrassWest4, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrassWest5] = new PointF(-4, 9.375F);

            Room oShortGrassWest6 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrassWest6, oShortGrassWest5, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oShortGrassWest6, oRiverBank20, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oRiverBank21, oShortGrassWest6, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oShortGrassWest6] = new PointF(-3.25F, 9.125F);

            Room oShortGrassWest7 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrassWest7, oShortGrassWest6, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrassWest7] = new PointF(-4, 8.875F);

            Room oShortGrassWest8 = AddRoom("Short Grass", "Short Grass");
            AddBidirectionalExits(oShortGrassWest8, oShortGrassWest7, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oRiverBank23, oShortGrassWest8, BidirectionalExitType.SouthwestNortheast);
            AddExit(oShortGrassWest8, oRiverBank22, "southeast");
            breeToImladrisGraph.Rooms[oShortGrassWest8] = new PointF(-3.25F, 8.625F);

            Room oShortGrassWest9 = AddRoom("Short Grass", "Short Grass");
            AddPermanentMobs(oShortGrassWest9, MobTypeEnum.Tracker);
            AddBidirectionalExits(oShortGrassWest9, oShortGrassWest8, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oShortGrassWest9] = new PointF(-4, 8.375F);

            Room oTallGrassWest1 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oShortGrassWest9, oTallGrassWest1, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oTallGrassWest1] = new PointF(-4.75F, 8.625F);

            Room oTallGrassWest2 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrassWest1, oTallGrassWest2, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oTallGrassWest2] = new PointF(-5.5F, 8.875F);

            Room oTallGrassWest3 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrassWest2, oTallGrassWest3, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oTallGrassWest3] = new PointF(-6.25F, 9.125F);

            Room oTallGrassWest4 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrassWest4, oTallGrassWest3, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oTallGrassWest4] = new PointF(-7, 8.875F);

            Room oTallGrassWest5 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrassWest5, oTallGrassWest4, BidirectionalExitType.SouthwestNortheast);
            AddExit(oTallGrassWest5, oTallGrassWest2, "southeast");
            breeToImladrisGraph.Rooms[oTallGrassWest5] = new PointF(-6.25F, 8.625F);

            Room oTallGrassWest6 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrassWest6, oTallGrassWest4, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oTallGrassWest6] = new PointF(-7.75F, 8.625F);

            Room oTallGrassWest7 = AddRoom("Tall Grass", "Tall Grass");
            AddBidirectionalExits(oTallGrassWest7, oTallGrassWest6, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oTallGrassWest7, oTallGrassWest5, BidirectionalExitType.SoutheastNorthwest);
            AddExit(oTallGrassWest2, oTallGrassWest7, "northwest");
            breeToImladrisGraph.Rooms[oTallGrassWest7] = new PointF(-6.5F, 8.25F);

            Room oWesternWalkway = AddRoom("Western Walkway", "Western Walkway");
            AddBidirectionalExits(oWesternWalkway, oRiverBank24, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oWesternWalkway] = new PointF(-2.5F, 8);

            Room oValarDam1 = AddRoom("Valar Dam", "The Valar Dam");
            AddPermanentMobs(oValarDam1, MobTypeEnum.BridgeGuard);
            AddExit(oWesternWalkway, oValarDam1, "ladder");
            AddExit(oValarDam1, oWesternWalkway, "down");
            breeToImladrisGraph.Rooms[oValarDam1] = new PointF(-2.5F, 7.75F);

            Room oValarDam2 = AddRoom("Valar Dam", "The Valar Dam");
            AddPermanentMobs(oValarDam2, MobTypeEnum.BridgeGuard);
            AddBidirectionalExits(oValarDam1, oValarDam2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oValarDam2] = new PointF(-1.75F, 7.75F);

            Room oEasternWalkway = AddRoom("Eastern Walkway", "Eastern Walkway");
            AddExit(oValarDam2, oEasternWalkway, "down");
            AddExit(oEasternWalkway, oValarDam2, "ladder");
            breeToImladrisGraph.Rooms[oEasternWalkway] = new PointF(-1.75F, 8);
        }

        private void AddToFarmHouseAndUglies(Room oGreatEastRoad1, out Room oOuthouse, RoomGraph breeToImladrisGraph)
        {
            Room oRoadToFarm1 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oGreatEastRoad1, oRoadToFarm1, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm1] = new PointF(-2, 4.5F);

            Room oRoadToFarm2 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oRoadToFarm1, oRoadToFarm2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm2] = new PointF(-2, 5);

            Room oWheatField = AddRoom("Wheat Field", "Wheat Field");
            AddBidirectionalExits(oWheatField, oRoadToFarm2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oWheatField] = new PointF(-3, 5);

            Room oCornField = AddRoom("Corn Field", "Corn Field");
            AddBidirectionalExits(oCornField, oWheatField, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oCornField] = new PointF(-4, 4.75F);

            Room oLembasField = AddRoom("Lembas Field", "Lembas Field");
            AddBidirectionalExits(oWheatField, oLembasField, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oLembasField] = new PointF(-4, 5.25F);

            Room oRoadToFarm3 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oRoadToFarm2, oRoadToFarm3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm3] = new PointF(-2, 6);

            Room oRoadToFarm4 = AddRoom("Farmland", "Farmland");
            AddBidirectionalExits(oRoadToFarm3, oRoadToFarm4, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm4] = new PointF(-2, 7);

            Room oRoadToFarm5 = AddRoom("Ranch House Path", "Path to Ranch House");
            AddBidirectionalExits(oRoadToFarm5, oRoadToFarm4, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oRoadToFarm5] = new PointF(-3, 7);

            Room oRoadToFarm6 = AddRoom("Front Steps", "Ranch House Front Steps");
            AddBidirectionalExits(oRoadToFarm6, oRoadToFarm5, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oRoadToFarm6] = new PointF(-4, 7);

            Room oGrainSilo = AddRoom("Grain Silo", "Grain Silo");
            AddBidirectionalExits(oRoadToFarm6, oGrainSilo, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGrainSilo] = new PointF(-4, 7.5F);

            oOuthouse = AddRoom("Outhouse", "Outhouse");
            AddPermanentItems(oOuthouse, ItemTypeEnum.OutOfOrderSign);
            AddBidirectionalExits(oRoadToFarm4, oOuthouse, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oOuthouse] = new PointF(-1.25F, 7F);

            Room oSwimmingPond = AddRoom("Swim Pond", "Swimming Pond");
            AddExit(oOuthouse, oSwimmingPond, "pond");
            AddExit(oSwimmingPond, oOuthouse, "west");
            breeToImladrisGraph.Rooms[oSwimmingPond] = new PointF(-0.5F, 7F);

            Room oPond = AddRoom("Pond", "Pond");
            AddPermanentMobs(oPond, MobTypeEnum.WaterTurtle);
            AddBidirectionalExitsWithOut(oSwimmingPond, oPond, "pond");
            breeToImladrisGraph.Rooms[oPond] = new PointF(-0.5F, 6.5F);

            Room oMuddyPath = AddRoom("Muddy Path", "Muddy Path");
            Exit e = AddExit(oSwimmingPond, oMuddyPath, "path");
            e.Hidden = true;
            AddExit(oMuddyPath, oSwimmingPond, "pond");
            breeToImladrisGraph.Rooms[oMuddyPath] = new PointF(0.25F, 7);

            Room oSmallPlayground = AddRoom("Playground", "Small Playground");
            AddBidirectionalExits(oSmallPlayground, oMuddyPath, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oSmallPlayground] = new PointF(2, 6.5F);

            Room oUglyKidSchoolEntrance = AddRoom("School Entrance", "Ugly Kid School Entrance");
            AddBidirectionalSameNameExit(oSmallPlayground, oUglyKidSchoolEntrance, "gate");
            breeToImladrisGraph.Rooms[oUglyKidSchoolEntrance] = new PointF(3, 6.5F);

            Room oMuddyFoyer = AddRoom("Muddy Foyer", "Muddy Foyer");
            e = AddBidirectionalExitsWithOut(oUglyKidSchoolEntrance, oMuddyFoyer, "front");
            e.MaximumLevel = 10;
            breeToImladrisGraph.Rooms[oMuddyFoyer] = new PointF(3, 6);

            Room oUglyKidClassroomK7 = AddRoom("Classroom K-7", "Ugly Kid Classroom K-7");
            AddExit(oMuddyFoyer, oUglyKidClassroomK7, "classroom");
            AddExit(oUglyKidClassroomK7, oMuddyFoyer, "foyer");
            breeToImladrisGraph.Rooms[oUglyKidClassroomK7] = new PointF(3, 5.5F);

            Room oHallway = AddRoom("Hallway", "Hallway");
            AddExit(oUglyKidClassroomK7, oHallway, "hallway");
            AddExit(oHallway, oUglyKidClassroomK7, "classroom");
            breeToImladrisGraph.Rooms[oHallway] = new PointF(3, 5);

            Room oHallwayEnd = AddRoom("Hallway End", "Hallway End");
            AddBidirectionalExits(oHallwayEnd, oHallway, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oHallwayEnd] = new PointF(3, 4.5F);

            Room oRoadToFarm7HoundDog = AddRoom("Hound Dog", "Front Porch");
            AddPermanentMobs(oRoadToFarm7HoundDog, MobTypeEnum.HoundDog);
            e = AddBidirectionalExitsWithOut(oRoadToFarm6, oRoadToFarm7HoundDog, "porch");
            e.MinimumLevel = 7;
            breeToImladrisGraph.Rooms[oRoadToFarm7HoundDog] = new PointF(-3, 6.5F);

            Room oFarmParlorManagerMulloyThreshold = AddRoom("Farm Parlor", "Parlor");
            AddBidirectionalSameNameMustOpenExit(oFarmParlorManagerMulloyThreshold, oRoadToFarm7HoundDog, "door");
            breeToImladrisGraph.Rooms[oFarmParlorManagerMulloyThreshold] = new PointF(-3, 6);

            Room oManagerMulloy = AddRoom("Manager Mulloy", "Study");
            AddPermanentMobs(oManagerMulloy, MobTypeEnum.ManagerMulloy);
            AddBidirectionalExitsWithOut(oFarmParlorManagerMulloyThreshold, oManagerMulloy, "study");
            breeToImladrisGraph.Rooms[oManagerMulloy] = new PointF(-3, 5.5F);

            Room oFarmKitchen = AddRoom("Kitchen", "Kitchen");
            AddExit(oFarmParlorManagerMulloyThreshold, oFarmKitchen, "kitchen");
            AddExit(oFarmKitchen, oFarmParlorManagerMulloyThreshold, "parlor");
            breeToImladrisGraph.Rooms[oFarmKitchen] = new PointF(-4, 5.5F);

            Room oFarmBackPorch = AddRoom("Back Porch", "Back Porch");
            AddExit(oFarmKitchen, oFarmBackPorch, "backdoor");
            AddExit(oFarmBackPorch, oFarmKitchen, "kitchen");
            breeToImladrisGraph.Rooms[oFarmBackPorch] = new PointF(-4, 6);

            Room oFarmCat = AddRoom("Farm Cat", "The Woodshed");
            AddPermanentMobs(oFarmCat, MobTypeEnum.FarmCat);
            oFarmCat.NoFlee = true;
            AddExit(oFarmBackPorch, oFarmCat, "woodshed");
            e = AddExit(oFarmCat, oFarmBackPorch, "out");
            breeToImladrisGraph.Rooms[oFarmCat] = new PointF(-4, 6.5F);

            Room oCrabbe = AddRoom("Crabbe", "Detention Room");
            AddPermanentMobs(oCrabbe, MobTypeEnum.CrabbeTheClassBully);
            AddBidirectionalExitsWithOut(oHallway, oCrabbe, "detention");
            breeToImladrisGraph.Rooms[oCrabbe] = new PointF(2, 5);

            Room oMrWartnose = AddRoom("Mr. Wartnose", "Mr. Wartnose's Office");
            AddPermanentMobs(oMrWartnose, MobTypeEnum.MrWartnose);
            AddBidirectionalExitsWithOut(oUglyKidClassroomK7, oMrWartnose, "office");
            breeToImladrisGraph.Rooms[oMrWartnose] = new PointF(2, 5.5F);
        }

        private void AddGalbasiDowns(Room oGreatEastRoad2, Room oGreatEastRoad3, RoomGraph breeToImladrisGraph)
        {
            Room oGalbasiDownsEntrance = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsEntrance, oGreatEastRoad2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oGalbasiDownsEntrance] = new PointF(5, 3.5F);

            Room oGalbasiDownsSoutheast = AddRoom("Galbasi Downs", "Galbasi Downs");
            Exit e = AddExit(oGreatEastRoad3, oGalbasiDownsSoutheast, "north");
            e.Hidden = true;
            AddExit(oGalbasiDownsSoutheast, oGreatEastRoad3, "south");
            AddBidirectionalExits(oGalbasiDownsEntrance, oGalbasiDownsSoutheast, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGalbasiDownsSoutheast] = new PointF(6, 3.5F);

            Room oGalbasiDownsSouthwest = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsSouthwest, oGalbasiDownsEntrance, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGalbasiDownsSouthwest] = new PointF(4, 3.5F);

            Room oGalbasiDownsNorth = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsEntrance, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsSoutheast, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsSouthwest, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oGalbasiDownsNorth] = new PointF(5, 3);

            Room oGalbasiDownsNorthwest = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorthwest, oGalbasiDownsSouthwest, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGalbasiDownsNorthwest, oGalbasiDownsNorth, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGalbasiDownsNorthwest] = new PointF(4, 3);

            Room oGalbasiDownsNortheast = AddRoom("Galbasi Downs", "Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsNortheast, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGalbasiDownsNortheast] = new PointF(6, 3);

            Room oGalbasiDownsFurthestNorth = AddRoom("Galbasi Downs End", "Galbasi Downs");
            AddExit(oGalbasiDownsFurthestNorth, oGalbasiDownsNortheast, "southeast");
            e = AddExit(oGalbasiDownsNortheast, oGalbasiDownsFurthestNorth, "northwest");
            e.Hidden = true;
            AddExit(oGalbasiDownsFurthestNorth, oGalbasiDownsNorthwest, "southwest");
            e = AddExit(oGalbasiDownsNorthwest, oGalbasiDownsFurthestNorth, "northeast");
            e.Hidden = true;
            breeToImladrisGraph.Rooms[oGalbasiDownsFurthestNorth] = new PointF(5, 2.5F);

            Room oTopOfHill = AddRoom("Hilltop", "Top of Hill");
            e = AddExit(oGalbasiDownsFurthestNorth, oTopOfHill, "hill");
            e.Hidden = true;
            AddExit(oTopOfHill, oGalbasiDownsFurthestNorth, "north");
            breeToImladrisGraph.Rooms[oTopOfHill] = new PointF(5, 2.25F);

            Room oBarrow = AddRoom("Barrow", "Barrow");
            e = AddExit(oTopOfHill, oBarrow, "niche");
            e.Hidden = true;
            e.IsTrapExit = true;
            //This always fails is this always the case or am I just using a too low dexterity/level character?
            //e = AddExit(oBarrow, oTopOfHill, "up");
            //e.IsTrapExit = true;
            breeToImladrisGraph.Rooms[oBarrow] = new PointF(5, 2);

            Room oAntechamber = AddRoom("Antechamber DMG", "Antechamber");
            AddExit(oBarrow, oAntechamber, "altar");
            AddExit(oAntechamber, oBarrow, "up");
            oAntechamber.DamageType = RoomDamageType.Fire;
            breeToImladrisGraph.Rooms[oAntechamber] = new PointF(5, 1.75F);

            Room oGalbasiHalls = AddRoom("Galbasi Halls", "Galbasi Halls");
            e = AddExit(oAntechamber, oGalbasiHalls, "stairway");
            e.Hidden = true;
            AddExit(oGalbasiHalls, oAntechamber, "stairway");
            breeToImladrisGraph.Rooms[oGalbasiHalls] = new PointF(5, 1.5F);

            Room oUnderHallsCorridorsGreenSlime = AddRoom("Gr Slime Underhalls", "Underhalls Corridors");
            AddPermanentMobs(oUnderHallsCorridorsGreenSlime, MobTypeEnum.GreenSlime, MobTypeEnum.GreenSlime);
            AddBidirectionalExits(oUnderHallsCorridorsGreenSlime, oGalbasiHalls, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderHallsCorridorsGreenSlime] = new PointF(5, 1.25F);

            Room oUnderHallsCorridorsBaseJunction = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderHallsCorridorsBaseJunction, oUnderHallsCorridorsGreenSlime, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderHallsCorridorsBaseJunction] = new PointF(5, 1);

            Room oUnderhallsCorridorsWest = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsWest, oUnderHallsCorridorsBaseJunction, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsWest] = new PointF(4.33F, 1);

            Room oDarkCorner = AddRoom("Skeleton", "Dark Corner");
            AddPermanentMobs(oDarkCorner, MobTypeEnum.Skeleton);
            AddBidirectionalExits(oDarkCorner, oUnderhallsCorridorsWest, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDarkCorner] = new PointF(3.67F, 1);

            Room oToCave = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderHallsCorridorsBaseJunction, oToCave, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oToCave] = new PointF(6, 1);

            Room oGlowingCave = AddHealingRoom("Glowing Cave", "Glowing Cave", HealingRoom.Underhalls);
            AddBidirectionalExitsWithOut(oToCave, oGlowingCave, "cave");
            breeToImladrisGraph.Rooms[oGlowingCave] = new PointF(6, 2);

            Room oUnderhallsCorridorsEast = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oToCave, oUnderhallsCorridorsEast, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsEast] = new PointF(7, 1);

            Room oUnderhallsCorridorsToQuarry = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsEast, oUnderhallsCorridorsToQuarry, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToQuarry] = new PointF(8, 1);

            Room oUnderhallsCorridorsSoutheastCorner = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToQuarry, oUnderhallsCorridorsSoutheastCorner, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsSoutheastCorner] = new PointF(9, 1);

            Room oUnderhallsCorridorsEast2 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsEast2, oUnderhallsCorridorsSoutheastCorner, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsEast2] = new PointF(9, 0.5F);

            Room oUnderhallsIronDoor = AddRoom("Iron Door", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsIronDoor, oUnderhallsCorridorsEast2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsIronDoor] = new PointF(9, 0);

            Room oBlackEyeOrcDwelling = AddRoom("Sewer Orcs", "Black Eye Orc Dwelling");
            AddPermanentMobs(oBlackEyeOrcDwelling, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc, MobTypeEnum.SewerOrc);
            e = AddBidirectionalExitsWithOut(oUnderhallsIronDoor, oBlackEyeOrcDwelling, "iron");
            e.MustOpen = true;
            e.IsUnknownKnockableKeyType = true;
            e.IsTrapExit = true;
            breeToImladrisGraph.Rooms[oBlackEyeOrcDwelling] = new PointF(9, -0.5F);

            Room oUnderhallsCorridorsNE = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsNE, oUnderHallsCorridorsBaseJunction, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsNE] = new PointF(6, 0);

            Room oUnderhallsCorridorsN = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsN, oUnderhallsCorridorsNE, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsN] = new PointF(6, -0.5F);

            Room oOrcsQuarry = AddRoom("Orcs' Quarry", "Orcs' Quarry");
            AddBidirectionalExits(oUnderhallsCorridorsNE, oOrcsQuarry, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oOrcsQuarry] = new PointF(8, 0);

            Room oOrcsQuarry2 = AddRoom("Orc Guard", "Orcs' Quarry");
            AddPermanentMobs(oOrcsQuarry2, MobTypeEnum.OrcGuard, MobTypeEnum.OrcMiner, MobTypeEnum.OrcMiner, MobTypeEnum.OrcMiner);
            AddBidirectionalExits(oOrcsQuarry, oOrcsQuarry2, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oUnderhallsCorridorsToQuarry, oOrcsQuarry2, BidirectionalExitType.UpDown);
            breeToImladrisGraph.Rooms[oOrcsQuarry2] = new PointF(8, 0.5F);

            Room oUnderhallsCorridorsFromGreenSlime = AddRoom("Coridor", "Underhalls Corridors");
            e = AddExit(oUnderHallsCorridorsGreenSlime, oUnderhallsCorridorsFromGreenSlime, "west");
            e.Hidden = true;
            AddExit(oUnderhallsCorridorsFromGreenSlime, oUnderHallsCorridorsGreenSlime, "east");
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsFromGreenSlime] = new PointF(3, 1.25F);

            Room oUnderhallsCorridorsToStoneDoor1 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToStoneDoor1, oUnderhallsCorridorsFromGreenSlime, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToStoneDoor1] = new PointF(3, 0);

            Room oUnderhallsCorridorsToStoneDoor2 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToStoneDoor2, oUnderhallsCorridorsToStoneDoor1, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToStoneDoor2] = new PointF(3, -1);

            Room oAmbushedParty = AddRoom("Ambushed Party", "Ambushed Party");
            AddPermanentMobs(oAmbushedParty, MobTypeEnum.Ghoul, MobTypeEnum.Ghoul, MobTypeEnum.Ghoul);
            e = AddExit(oUnderhallsCorridorsToStoneDoor2, oAmbushedParty, "east");
            e.Hidden = true;
            AddExit(oAmbushedParty, oUnderhallsCorridorsToStoneDoor2, "west");
            breeToImladrisGraph.Rooms[oAmbushedParty] = new PointF(4, -1);

            Room oGhostOfMuzgash = AddRoom("Muzgash ghost", "Damp Cell");
            AddPermanentMobs(oGhostOfMuzgash, MobTypeEnum.GhostOfMuzgash);
            e = AddExit(oAmbushedParty, oGhostOfMuzgash, "door");
            e.Hidden = true;
            AddExit(oGhostOfMuzgash, oAmbushedParty, "door");
            breeToImladrisGraph.Rooms[oGhostOfMuzgash] = new PointF(4, -1.5F);

            Room oUndeadFeastingGrounds = AddRoom("Feasting Grounds", "Undead Feasting Grounds");
            e = AddExit(oAmbushedParty, oUndeadFeastingGrounds, "east");
            e.Hidden = true;
            AddExit(oUndeadFeastingGrounds, oAmbushedParty, "west");
            breeToImladrisGraph.Rooms[oUndeadFeastingGrounds] = new PointF(5, -1);

            Room oHallOfTheDead = AddRoom("Hall of the Dead", "Hall of the Dead");
            AddBidirectionalExits(oUndeadFeastingGrounds, oHallOfTheDead, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oHallOfTheDead] = new PointF(6, -1);

            Room oLichsLair = AddRoom("Lich's Lair", "Lich's Lair");
            AddPermanentMobs(oLichsLair, MobTypeEnum.MinorLich, MobTypeEnum.MinorLich);
            e = AddExit(oHallOfTheDead, oLichsLair, "hidden");
            e.Hidden = true;
            e.FloatRequirement = FloatRequirement.Levitation;
            e.IsTrapExit = true;
            AddExit(oLichsLair, oHallOfTheDead, "walkway");
            breeToImladrisGraph.Rooms[oLichsLair] = new PointF(6, -1.5F);

            Room oBottomOfAbyss = AddRoom("Bottom of Abyss", "Bottom of Abyss");
            e = AddExit(oHallOfTheDead, oBottomOfAbyss, "hidden");
            e.Hidden = true;
            e.FloatRequirement = FloatRequirement.NoLevitation;
            e.IsTrapExit = true;
            breeToImladrisGraph.Rooms[oBottomOfAbyss] = new PointF(7, -1.5F);

            Room oUnderhallsCorridorsStoneDoor = AddRoom("To Stone Door", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsStoneDoor, oUnderhallsCorridorsToStoneDoor2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsStoneDoor] = new PointF(3, -2);

            Room oUnderhallsBugbearsLair = AddRoom("Bugbear Lair", "Bugbears' Lair");
            AddPermanentMobs(oUnderhallsBugbearsLair, MobTypeEnum.Bugbear, MobTypeEnum.Bugbear, MobTypeEnum.Bugbear);
            AddBidirectionalSameNameExit(oUnderhallsCorridorsStoneDoor, oUnderhallsBugbearsLair, "stone", (exit) => { exit.IsUnknownKnockableKeyType = true; exit.MustOpen = true; });
            breeToImladrisGraph.Rooms[oUnderhallsBugbearsLair] = new PointF(4, -2);

            Room oHiddenCubicle = AddRoom("Hidden Cubicle", "Hidden Cubicle");
            e = AddBidirectionalExitsWithOut(oUnderhallsBugbearsLair, oHiddenCubicle, "hidden");
            e.Hidden = true;
            e.IsUnknownKnockableKeyType = true;
            e.MustOpen = true;
            breeToImladrisGraph.Rooms[oHiddenCubicle] = new PointF(5, -2);

            Room oUnderhallsCorridorsToOtherDoor1 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor1, oUnderhallsCorridorsFromGreenSlime, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor1] = new PointF(2, 0);

            Room oUnderhallsCorridorsToOtherDoor2 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor2, oUnderhallsCorridorsToOtherDoor1, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor2] = new PointF(1, 0);

            Room oDenseFog = AddRoom("Dense Fog", "Dense Fog");
            AddBidirectionalExits(oDenseFog, oUnderhallsCorridorsToOtherDoor2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDenseFog] = new PointF(0, 0);

            Room oUnderhallsCorridorsToOtherDoor3 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor3, oDenseFog, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor3] = new PointF(-1, 0);

            Room oUnderhallsCorridorsToOtherDoor4 = AddRoom("Corridor", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsToOtherDoor4, oUnderhallsCorridorsToOtherDoor3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsToOtherDoor4] = new PointF(-1, -1);

            Room oUnderhallsCorridorsOtherDoor = AddRoom("To Door", "Underhalls Corridors");
            AddPermanentMobs(oUnderhallsCorridorsOtherDoor, MobTypeEnum.DoorMimic);
            AddBidirectionalExits(oUnderhallsCorridorsOtherDoor, oUnderhallsCorridorsToOtherDoor4, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsCorridorsOtherDoor] = new PointF(-1, -2);

            Room oDisposalPit = AddRoom("Disposal Pit", "Disposal Pit");
            AddBidirectionalExitsWithOut(oUnderhallsCorridorsOtherDoor, oDisposalPit, "door");
            breeToImladrisGraph.Rooms[oDisposalPit] = new PointF(0, -2);

            Room oInsideThePit = AddRoom("Inside Pit", "Inside the Pit");
            AddPermanentMobs(oInsideThePit, MobTypeEnum.Otyugh, MobTypeEnum.Otyugh);
            AddExit(oDisposalPit, oInsideThePit, "trapdoor");
            AddExit(oInsideThePit, oDisposalPit, "up");
            breeToImladrisGraph.Rooms[oInsideThePit] = new PointF(1, -2);

            Room oUnderhallsToAntechamber = AddRoom("To Antechamber", "Underhalls Corridors");
            AddBidirectionalExits(oUnderhallsCorridorsFromGreenSlime, oUnderhallsToAntechamber, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oUnderhallsToAntechamber] = new PointF(3, 2);

            Room oUnderhallsAntechamber = AddRoom("Antechamber", "Antechamber");
            AddPermanentMobs(oUnderhallsAntechamber, MobTypeEnum.Dervish);
            AddBidirectionalExits(oUnderhallsToAntechamber, oUnderhallsAntechamber, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oUnderhallsAntechamber] = new PointF(4, 2);
        }

        private void AddImladrisCity(out Room oImladrisSouthGateInside, out Room oEastGateOfImladrisOutside, Room imladrisWestGateOutside, out Room healingHand, out Room oEastGateOfImladrisInside)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];
            RoomGraph breeToImladrisGraph = _graphs[MapType.BreeToImladris];

            Room imladrisWestGateInside = AddRoom("West Gate Inside", "West Gate of Imladris");
            AddExit(imladrisWestGateInside, imladrisWestGateOutside, "gate");
            Exit e = AddExit(imladrisWestGateOutside, imladrisWestGateInside, "gate");
            e.RequiresDay = true;
            imladrisGraph.Rooms[imladrisWestGateOutside] = new PointF(-1, 5);
            imladrisGraph.Rooms[imladrisWestGateInside] = new PointF(0, 5);
            breeToImladrisGraph.Rooms[imladrisWestGateInside] = new PointF(19, 4);
            AddMapBoundaryPoint(imladrisWestGateOutside, imladrisWestGateInside, MapType.BreeToImladris, MapType.Imladris);

            Room oImladrisCircle1 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle1, imladrisWestGateInside, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle1] = new PointF(5F / 3, 5 - (4F / 3));

            Room oImladrisCircle2 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle2, oImladrisCircle1, BidirectionalExitType.SouthwestNortheast);
            PointF p = new PointF(10F / 3, 5 - (8F / 3));
            imladrisGraph.Rooms[oImladrisCircle2] = p;

            Room oAsylumCourtyard = AddRoom("Asylum Courtyard", "Asylum Courtyard");
            AddExit(oImladrisCircle2, oAsylumCourtyard, "east");
            AddExit(oAsylumCourtyard, oImladrisCircle2, "road");
            imladrisGraph.Rooms[oAsylumCourtyard] = new PointF(p.X + 1, p.Y);
            AddImladrisAsylum(oAsylumCourtyard);

            Room oImladrisCircle3 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle2, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle3] = new PointF(5, 1);

            Room oElvenTrainingGrounds = AddRoom("Training Grounds", "Elven Training Grounds");
            AddBidirectionalExits(oImladrisCircle3, oElvenTrainingGrounds, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oElvenTrainingGrounds] = new PointF(5, 1.75F);

            Room oHandToHandArena = AddRoom("Hand2Hand Arena", "Hand-To-Hand Combat Arena");
            AddBidirectionalExits(oElvenTrainingGrounds, oHandToHandArena, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oHandToHandArena] = new PointF(5.5F, 2);

            Room oDragonArena = AddRoom("Dragon Arena", "The Dragon Arena");
            AddBidirectionalExits(oElvenTrainingGrounds, oDragonArena, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oDragonArena] = new PointF(4.5F, 2);

            Room oLoreElesStronghold = AddRoom("Lore Ele's Stronghold", "Lore Ele's Stronghold");
            AddPermanentMobs(oLoreElesStronghold, MobTypeEnum.EleHonorGuard, MobTypeEnum.EleHonorGuard);
            AddBidirectionalSameNameExit(oImladrisCircle3, oLoreElesStronghold, "gate");
            imladrisGraph.Rooms[oLoreElesStronghold] = new PointF(5, 0);

            Room oImladrisCircle4 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle4, BidirectionalExitType.SoutheastNorthwest);
            p = new PointF(6 + (4F / 3), 1 + (4F / 3));
            imladrisGraph.Rooms[oImladrisCircle4] = p;

            Room oSmithy = AddRoom("Smithy", "Axel's Repair Shoppe");
            AddBidirectionalExits(oSmithy, oImladrisCircle4, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oSmithy] = new PointF(p.X - 1, p.Y);

            Room oImladrisCircle5 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle4, oImladrisCircle5, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle5] = new PointF(6 + (8F / 3), 1 + (8F / 3));

            Room oImladrisMainStreet1 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(imladrisWestGateInside, oImladrisMainStreet1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet1] = new PointF(1, 5);

            Room oImladrisMainStreet2 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet1, oImladrisMainStreet2, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet2] = new PointF(2.3F, 5);

            Room oImladrisMainStreet3 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet2, oImladrisMainStreet3, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet3] = new PointF(3, 5);

            Room oImladrisMainStreet4 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet3, oImladrisMainStreet4, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet4] = new PointF(4, 5);

            Room oImladrisMainStreet5 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet4, oImladrisMainStreet5, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet5] = new PointF(5, 5);

            Room oImladrisAlley3 = AddRoom("Side Alley", "Side Alley North");
            AddBidirectionalExits(oImladrisMainStreet5, oImladrisAlley3, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisAlley3] = new PointF(6, 5);

            Room oImladrisAlley4 = AddRoom("Side Alley", "Side Alley North");
            e = AddExit(oImladrisAlley3, oImladrisAlley4, "south");
            e.Hidden = true;
            AddExit(oImladrisAlley4, oImladrisAlley3, "north");
            imladrisGraph.Rooms[oImladrisAlley4] = new PointF(6, 6);

            Room oImladrisAlley5 = AddRoom("Side Alley", "Side Alley South");
            AddBidirectionalExits(oImladrisAlley4, oImladrisAlley5, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisAlley5] = new PointF(6, 7);

            Room oImladrisSmallAlley1 = AddRoom("Small Alley", "Small Alley");
            AddExit(oImladrisAlley3, oImladrisSmallAlley1, "alley");
            AddExit(oImladrisSmallAlley1, oImladrisAlley3, "south");
            imladrisGraph.Rooms[oImladrisSmallAlley1] = new PointF(6, 4);

            Room oImladrisSmallAlley2 = AddRoom("Small Alley", "Small Alley");
            AddBidirectionalExits(oImladrisSmallAlley2, oImladrisSmallAlley1, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisSmallAlley2] = new PointF(6, 3);

            Room oImladrisPawnShop = AddPawnShoppeRoom("Sharkey's Pawn Shop", "Sharkey's Pawn Shoppe", PawnShoppe.Imladris);
            AddBidirectionalSameNameExit(oImladrisPawnShop, oImladrisSmallAlley2, "door");
            imladrisGraph.Rooms[oImladrisPawnShop] = new PointF(5, 3);

            Room oImladrisTownCircle = AddRoom("Town Circle", "Imladris Town Circle");
            AddBidirectionalExits(oImladrisAlley3, oImladrisTownCircle, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisTownCircle] = new PointF(7, 5);

            Room oThenardiersInn = AddRoom("Thenardier's Inn", "Thenardier's Inn");
            AddBidirectionalExits(oThenardiersInn, oImladrisTownCircle, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oThenardiersInn] = new PointF(7, 4.5F);

            Room oImladrisMainStreet6 = AddRoom("Main", "Imladris Main Street");
            AddBidirectionalExits(oImladrisTownCircle, oImladrisMainStreet6, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet6] = new PointF(8, 5);

            Room oJewelry = AddRoom("Jewelry", "Zarlow's Fine Jewelry");
            AddBidirectionalExits(oJewelry, oImladrisMainStreet6, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oJewelry] = new PointF(8, 4.5F);

            Room oPostOffice = AddRoom("Post Office", "Imladris Post Office");
            AddBidirectionalExits(oImladrisMainStreet6, oPostOffice, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oPostOffice] = new PointF(8, 5.5F);

            oEastGateOfImladrisInside = AddRoom("East Gate Inside", "East Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle5, oEastGateOfImladrisInside, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisMainStreet6, oEastGateOfImladrisInside, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oEastGateOfImladrisInside] = new PointF(10, 5);

            oEastGateOfImladrisOutside = AddRoom("East Gate Outside", "Gates of Imladris");
            e = AddExit(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, "gate");
            e.MinimumLevel = 3;
            AddExit(oEastGateOfImladrisOutside, oEastGateOfImladrisInside, "gate");
            imladrisGraph.Rooms[oEastGateOfImladrisOutside] = new PointF(11, 5);
            AddMapBoundaryPoint(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, MapType.Imladris, MapType.EastOfImladris);

            Room oImladrisCircle6 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oEastGateOfImladrisInside, oImladrisCircle6, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle6] = new PointF(10 - (4F / 3), 5 + (4F / 3));

            Room oImladrisCircle7 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle6, oImladrisCircle7, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle7] = new PointF(10 - (8F / 3), 5 + (8F / 3));

            Room oImladrisCircle10 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(imladrisWestGateInside, oImladrisCircle10, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle10] = new PointF(5 - (10F / 3), 9 - (8F / 3));

            Room oImladrisCircle9 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle10, oImladrisCircle9, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle9] = new PointF(5 - (5F / 3), 9 - (4F / 3));

            Room oImladrisCircle8 = AddRoom("Circle", "Imladris Circle");
            AddBidirectionalExits(oImladrisCircle9, oImladrisCircle8, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisCircle7, oImladrisCircle8, BidirectionalExitType.SouthwestNortheast);
            AddExit(oImladrisAlley5, oImladrisCircle8, "south");
            imladrisGraph.Rooms[oImladrisCircle8] = new PointF(5, 9);

            Room oRearAlley = AddRoom("Rear Alley", "Rear Alley");
            e = AddExit(oImladrisCircle10, oRearAlley, "east");
            e.Hidden = true;
            AddExit(oRearAlley, oImladrisCircle10, "west");
            AddBidirectionalExits(oRearAlley, oImladrisAlley5, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oRearAlley] = new PointF(5, 7);

            Room oPoisonedDagger = AddRoom("Master Assassins", "The Poisoned Dagger");
            AddPermanentMobs(oPoisonedDagger, MobTypeEnum.MasterAssassin, MobTypeEnum.MasterAssassin);
            e = AddExit(oRearAlley, oPoisonedDagger, "door");
            e.Hidden = true;
            AddExit(oPoisonedDagger, oRearAlley, "door");
            imladrisGraph.Rooms[oPoisonedDagger] = new PointF(5, 6.5F);

            oImladrisSouthGateInside = AddRoom("South Gate Inside", "Southern Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle8, oImladrisSouthGateInside, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisSouthGateInside] = new PointF(5, 10);

            Room oImladrisCityDump = AddRoom("City Dump", "The Imladris City Dump");
            AddBidirectionalExits(oImladrisCityDump, oImladrisCircle8, BidirectionalExitType.NorthSouth);
            e = AddExit(oImladrisCityDump, oRearAlley, "north");
            e.Hidden = true;
            imladrisGraph.Rooms[oImladrisCityDump] = new PointF(5, 8);

            healingHand = AddHealingRoom("Healing Hand", "The Healing Hand", HealingRoom.Imladris);
            AddBidirectionalExits(healingHand, oImladrisMainStreet5, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[healingHand] = new PointF(5, 4.5F);

            Room oTyriesPriestSupplies = AddRoom("Tyrie's Priest Supplies", "Tyrie's Priest Supplies");
            AddBidirectionalExits(oImladrisMainStreet5, oTyriesPriestSupplies, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oTyriesPriestSupplies] = new PointF(5, 5.5F);

            Room oImladrisArmory = AddRoom("Armory", "Imladris Armory");
            AddBidirectionalExits(oImladrisArmory, oImladrisMainStreet4, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisArmory] = new PointF(4, 4.5F);

            Room oDriscollsPoolHall = AddRoom("Pool Hall", "Driscoll's Pool Hall");
            AddPermanentMobs(oDriscollsPoolHall, MobTypeEnum.Innkeeper);
            AddBidirectionalExits(oImladrisMainStreet4, oDriscollsPoolHall, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oDriscollsPoolHall] = new PointF(4, 5.5F);

            Room oImladrisGeneralStore = AddRoom("General Store", "Imladris General Store");
            AddBidirectionalExits(oImladrisGeneralStore, oImladrisMainStreet3, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisGeneralStore] = new PointF(3, 4.5F);

            Room oImladrisMageClub = AddRoom("Mage Club", "Imladris Mage's Club");
            AddBidirectionalExits(oImladrisMainStreet3, oImladrisMageClub, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisMageClub] = new PointF(3, 5.5F);

            Room oImladrisBank = AddRoom("Bank", "Bank of Imladris");
            AddBidirectionalExits(oImladrisMainStreet2, oImladrisBank, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisBank] = new PointF(2.3F, 5.5F);

            Room oCombatMall = AddRoom("Combat Mall", "Combat Mall");
            AddBidirectionalExits(oCombatMall, oImladrisMainStreet2, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oCombatMall] = new PointF(2.3F, 4.2F);

            Room oParmasPolearms = AddRoom("Polearms", "Parma's Polearms");
            AddBidirectionalExits(oParmasPolearms, oCombatMall, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oParmasPolearms] = new PointF(1.5F, 4.2F);

            Room oWeaponsmith = AddRoom("Weaponsmith", "Elven Weaponsmith");
            AddBidirectionalExits(oWeaponsmith, oCombatMall, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oWeaponsmith] = new PointF(2.3F, 3.7F);

            Room oArchery = AddRoom("Archery", "Feanaro's Archery Shoppe");
            AddBidirectionalExits(oCombatMall, oArchery, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oArchery] = new PointF(3.3F, 4.2F);

            Room oImladrisCityJail = AddRoom("City Jail", "Imladris City Jail");
            e = AddBidirectionalExitsWithOut(oImladrisAlley4, oImladrisCityJail, "door");
            e.MustOpen = true;
            imladrisGraph.Rooms[oImladrisCityJail] = new PointF(7, 6);

            Room oCaveTrollCell = AddRoom("Troll Cell", "Cave Troll Cell");
            AddPermanentMobs(oCaveTrollCell, MobTypeEnum.CaveTroll);
            e = AddBidirectionalExitsWithOut(oImladrisCityJail, oCaveTrollCell, "grate");
            e.MustOpen = true;
            imladrisGraph.Rooms[oCaveTrollCell] = new PointF(7, 5.5F);
        }

        private void AddImladrisAsylum(Room asylumCourtyard)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];

            Room oAsylumFoyer = AddRoom("Asylum Foyer", "Asylum Foyer");
            AddExit(asylumCourtyard, oAsylumFoyer, "asylum");
            AddExit(oAsylumFoyer, asylumCourtyard, "doors");
            imladrisGraph.Rooms[oAsylumFoyer] = new PointF(1, 1.5F);

            Room oAsylumWestHallway = AddRoom("West Hallway", "West Hallway");
            AddBidirectionalExits(oAsylumWestHallway, oAsylumFoyer, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oAsylumWestHallway] = new PointF(0, 1.5F);

            Room oAsylumEastHallway = AddRoom("East Hallway", "East Hallway");
            AddBidirectionalExits(oAsylumFoyer, oAsylumEastHallway, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oAsylumEastHallway] = new PointF(2, 1.5F);

            Room oNorthCourt = AddRoom("North Court", "North Court");
            AddBidirectionalExits(oNorthCourt, oAsylumFoyer, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oNorthCourt] = new PointF(1, 1);

            Room oWestStairwayDownstairs = AddRoom("Stairway", "Stairway");
            AddExit(oAsylumWestHallway, oWestStairwayDownstairs, "staircase");
            AddExit(oWestStairwayDownstairs, oAsylumWestHallway, "hallway");
            imladrisGraph.Rooms[oWestStairwayDownstairs] = new PointF(0, 1);

            Room oWestStairwayUpstairs = AddRoom("Stairway", "Stairway");
            AddBidirectionalExits(oWestStairwayUpstairs, oWestStairwayDownstairs, BidirectionalExitType.UpDown);
            imladrisGraph.Rooms[oWestStairwayUpstairs] = new PointF(0, 0.5F);

            Room oWestHallwayUpstairs = AddRoom("Hallway", "West Hallway");
            AddExit(oWestStairwayUpstairs, oWestHallwayUpstairs, "hallway");
            AddExit(oWestHallwayUpstairs, oWestStairwayUpstairs, "staircase");
            imladrisGraph.Rooms[oWestHallwayUpstairs] = new PointF(0, 0);

            Room oUpstairsLobby = AddRoom("Upstairs Lobby", "Upstairs Lobby");
            AddBidirectionalExits(oWestHallwayUpstairs, oUpstairsLobby, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oUpstairsLobby] = new PointF(1, 0);

            Room oNorthHallway = AddRoom("North Hallway", "North Hallway");
            Exit e = AddExit(oUpstairsLobby, oNorthHallway, "door");
            e.MustOpen = true;
            AddExit(oNorthHallway, oUpstairsLobby, "door");
            imladrisGraph.Rooms[oNorthHallway] = new PointF(1, -0.5F);

            Room oAsylumTop = AddRoom("Asylum Top", "The Top of the Asylum");
            e = AddExit(oNorthHallway, oAsylumTop, "ladder");
            e.Hidden = true;
            imladrisGraph.Rooms[oAsylumTop] = new PointF(1, -1);

            Room oEastHallwayUpstairs = AddRoom("Hallway", "East Hallway");
            AddBidirectionalExits(oUpstairsLobby, oEastHallwayUpstairs, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oEastHallwayUpstairs] = new PointF(2, 0);

            Room oEastStairwayUpstairs = AddRoom("Stairway", "Stairway");
            AddExit(oEastHallwayUpstairs, oEastStairwayUpstairs, "staircase");
            AddExit(oEastStairwayUpstairs, oEastHallwayUpstairs, "hallway");
            imladrisGraph.Rooms[oEastStairwayUpstairs] = new PointF(2, 0.5F);

            Room oEastStairwayDownstairs = AddRoom("Stairway", "Stairway");
            AddBidirectionalExits(oEastStairwayUpstairs, oEastStairwayDownstairs, BidirectionalExitType.UpDown);
            AddExit(oEastStairwayDownstairs, oAsylumEastHallway, "hallway");
            AddExit(oAsylumEastHallway, oEastStairwayDownstairs, "staircase");
            imladrisGraph.Rooms[oEastStairwayDownstairs] = new PointF(2, 1);
        }

        private void AddEastOfImladris(Room oEastGateOfImladrisOutside, Room oEastGateOfImladrisInside, out Room westGateOfEsgaroth)
        {
            RoomGraph eastOfImladrisGraph = _graphs[MapType.EastOfImladris];

            eastOfImladrisGraph.Rooms[oEastGateOfImladrisOutside] = new PointF(0, 6);
            eastOfImladrisGraph.Rooms[oEastGateOfImladrisInside] = new PointF(-1, 6);

            Room oMountainPath1 = AddRoom("Mountain Path", "Mountain Path");
            AddBidirectionalExits(oEastGateOfImladrisOutside, oMountainPath1, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainPath1] = new PointF(1, 6);

            Room oMountainPath2 = AddRoom("Mountain Path", "Mountain Path");
            AddBidirectionalExits(oMountainPath2, oMountainPath1, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPath2] = new PointF(2, 5);

            Room oSouthernFootpath1 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oMountainPath2, oSouthernFootpath1, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oSouthernFootpath1] = new PointF(3, 6);

            Room oSouthernFootpath2 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oSouthernFootpath1, oSouthernFootpath2, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oSouthernFootpath2] = new PointF(3, 7);

            Room oSouthernFootpath3 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oSouthernFootpath2, oSouthernFootpath3, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oSouthernFootpath3] = new PointF(4, 8);

            Room oSouthernFootpath4 = AddRoom("Southern Footpath", "Southern Footpath");
            AddBidirectionalExits(oSouthernFootpath3, oSouthernFootpath4, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oSouthernFootpath4] = new PointF(5, 9);

            Room oGiantRockslide = AddRoom("Giant Rockslide", "Giant Rockslide");
            AddBidirectionalExits(oSouthernFootpath4, oGiantRockslide, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oGiantRockslide] = new PointF(6, 9);
            //CSRTODO: trail (hidden

            Room oMountainTrail1 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail1, oMountainPath2, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oMountainTrail1] = new PointF(2, 4);

            Room oIorlasThreshold = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oIorlasThreshold, oMountainTrail1, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oIorlasThreshold] = new PointF(3, 3);

            Room oMountainTrailEastOfIorlas1 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oIorlasThreshold, oMountainTrailEastOfIorlas1, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas1] = new PointF(4, 3);

            Room oMountainTrailEastOfIorlas2 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrailEastOfIorlas1, oMountainTrailEastOfIorlas2, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas2] = new PointF(5, 3);

            Room oMountainTrailEastOfIorlas3 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrailEastOfIorlas2, oMountainTrailEastOfIorlas3, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas3] = new PointF(6, 3);

            Room oMountainTrailEastOfIorlas4 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrailEastOfIorlas3, oMountainTrailEastOfIorlas4, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oMountainTrailEastOfIorlas4] = new PointF(7, 4);

            Room oCarrockPlains = AddRoom("Carrock Plains", "Carrock Plains");
            AddBidirectionalExits(oMountainTrailEastOfIorlas4, oCarrockPlains, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[oCarrockPlains] = new PointF(8, 4);

            westGateOfEsgaroth = AddRoom("West Gate Outside", "West Entrance to Esgaroth");
            AddBidirectionalExits(oCarrockPlains, westGateOfEsgaroth, BidirectionalExitType.WestEast);
            eastOfImladrisGraph.Rooms[westGateOfEsgaroth] = new PointF(9, 4);

            Room oMountainTrail3 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail3, oIorlasThreshold, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainTrail3] = new PointF(4, 2);

            Room oMountainPass1 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass1, oMountainTrail3, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPass1] = new PointF(5, 1);

            Room oMountainPass2 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass2, oMountainPass1, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPass2] = new PointF(6, 0);

            Room oMountainPass3 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass1, oMountainPass3, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oMountainPass3] = new PointF(6, 2);

            Room oMountainPass4 = AddRoom("Mountain Pass", "Mountain Pass");
            AddBidirectionalExits(oMountainPass2, oMountainPass4, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oMountainPass4, oMountainPass3, BidirectionalExitType.SouthwestNortheast);
            eastOfImladrisGraph.Rooms[oMountainPass4] = new PointF(7, 1);
            //CSRTODO: down to ituk glacer (hidden)

            Room oLoftyTrail1 = AddRoom("Lofty Trail", "A Lofty Trail");
            AddBidirectionalExits(oLoftyTrail1, oMountainPass2, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oLoftyTrail1] = new PointF(6, -0.5F);

            Room oLoftyTrail2 = AddRoom("Lofty Trail", "A Lofty Trail");
            AddBidirectionalExits(oLoftyTrail2, oLoftyTrail1, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oLoftyTrail2] = new PointF(6, -1);

            Room oLoftyTrail3 = AddRoom("Lofty Trail", "Lofty Trail");
            AddExit(oLoftyTrail2, oLoftyTrail3, "up");
            Exit e = AddExit(oLoftyTrail3, oLoftyTrail2, "down");
            e.IsTrapExit = true;
            eastOfImladrisGraph.Rooms[oLoftyTrail3] = new PointF(6, -1.5F);

            Room oLoftyTrail4 = AddRoom("Lofty Trail", "Lofty Trail");
            AddExit(oLoftyTrail3, oLoftyTrail4, "up");
            e = AddExit(oLoftyTrail4, oLoftyTrail3, "down");
            e.IsTrapExit = true;
            eastOfImladrisGraph.Rooms[oLoftyTrail4] = new PointF(6, -2);

            Room oMountainDragon = AddRoom("Mountain Dragon", "Lofty Trail");
            AddPermanentMobs(oMountainDragon, MobTypeEnum.MountainDragon);
            AddExit(oLoftyTrail4, oMountainDragon, "up");
            AddExit(oMountainDragon, oLoftyTrail4, "down");
            eastOfImladrisGraph.Rooms[oMountainDragon] = new PointF(6, -2.5F);
            //CSRTODO: up

            Room oMountainTrail4 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail4, oMountainTrail3, BidirectionalExitType.SoutheastNorthwest);
            eastOfImladrisGraph.Rooms[oMountainTrail4] = new PointF(3, 1);

            Room oMistyMountainsCave = AddRoom("Cave", "Cave in the Misty Mountains");
            AddBidirectionalExitsWithOut(oMountainTrail4, oMistyMountainsCave, "cave");
            eastOfImladrisGraph.Rooms[oMistyMountainsCave] = new PointF(2, 1);

            Room oMountainTrail5 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail5, oMountainTrail4, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oMountainTrail5] = new PointF(3, 0.5F);

            Room oMountainTrail6 = AddRoom("Mountain Trail", "Mountain Trail");
            AddBidirectionalExits(oMountainTrail6, oMountainTrail5, BidirectionalExitType.NorthSouth);
            eastOfImladrisGraph.Rooms[oMountainTrail6] = new PointF(3, 0);

            Room oLarsMagnusGrunwald = AddRoom("Lars Magnus Grunwald", "The Greywold Goldmine");
            AddPermanentMobs(oLarsMagnusGrunwald, MobTypeEnum.LarsMagnusGrunwald);
            AddBidirectionalSameNameExit(oMountainTrail6, oLarsMagnusGrunwald, "gate");
            eastOfImladrisGraph.Rooms[oLarsMagnusGrunwald] = new PointF(3, -0.5F);

            Room oCarlArl = AddRoom("Carl Arl", "Great Hall of the Greywold Goldmine");
            AddPermanentMobs(oCarlArl, MobTypeEnum.CarlArl);
            AddBidirectionalSameNameExit(oLarsMagnusGrunwald, oCarlArl, "door");
            eastOfImladrisGraph.Rooms[oCarlArl] = new PointF(3, -1);

            Room oIorlas = AddRoom("Iorlas", "Hermit's Shack");
            AddPermanentMobs(oIorlas, MobTypeEnum.IorlasTheHermit);
            AddPermanentItems(oIorlas, ItemTypeEnum.LittleBrownJug);
            AddExit(oIorlasThreshold, oIorlas, "shack");
            AddExit(oIorlas, oIorlasThreshold, "door");
            eastOfImladrisGraph.Rooms[oIorlas] = new PointF(2, 3);
        }

        private void AddBreeToHobbiton(Room oBreeWestGateInside, Room oSmoulderingVillage, out Room westronRoadToMithlond, out Room valleyRoad, out Room westernRoadWestOfHobbiton)
        {
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];
            RoomGraph westOfBreeMap = _graphs[MapType.WestOfBree];
            RoomGraph breeSewersMap = _graphs[MapType.BreeSewers];
            RoomGraph mithlondGraph = _graphs[MapType.Mithlond];

            westOfBreeMap.Rooms[oBreeWestGateInside] = new PointF(15, 0);

            Room oBreeWestGateOutside = AddRoom("West Gate Outside", "West Gate of Bree");
            AddBidirectionalSameNameExit(oBreeWestGateInside, oBreeWestGateOutside, "gate");
            breeStreetsGraph.Rooms[oBreeWestGateOutside] = new PointF(-3.25F, 3);
            westOfBreeMap.Rooms[oBreeWestGateOutside] = new PointF(14, 0);
            AddMapBoundaryPoint(oBreeWestGateInside, oBreeWestGateOutside, MapType.BreeStreets, MapType.WestOfBree);

            Room oGrandIntersection = AddRoom("Grand Intersection", "The Grand Intersection - Leviathan Way/North Fork Road/Western Road");
            AddBidirectionalExits(oGrandIntersection, oBreeWestGateOutside, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oGrandIntersection] = new PointF(13, 0);

            Room oLeviathanWay1 = AddRoom("Leviathan Way", "Leviathan Way");
            AddBidirectionalExits(oGrandIntersection, oLeviathanWay1, BidirectionalExitType.SouthwestNortheast);
            westOfBreeMap.Rooms[oLeviathanWay1] = new PointF(12, 1);

            Room oLeviathanWay2 = AddRoom("Leviathan Way", "Leviathan Way");
            AddBidirectionalExits(oLeviathanWay1, oLeviathanWay2, BidirectionalExitType.SouthwestNortheast);
            westOfBreeMap.Rooms[oLeviathanWay2] = new PointF(11, 2);

            Room oGrassyField = AddRoom("Grassy Field", "Grassy Field");
            Exit e = AddExit(oGrandIntersection, oGrassyField, "southeast");
            e.MaximumLevel = 12;
            AddExit(oGrassyField, oGrandIntersection, "northwest");
            westOfBreeMap.Rooms[oGrassyField] = new PointF(14, 1);

            Room oNorthFork1 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork1, oGrandIntersection, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oNorthFork1] = new PointF(12, -1);

            Room oNorthFork2 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork2, oNorthFork1, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oNorthFork2] = new PointF(11, -2);

            Room oTravelersShrine = AddRoom("Traveler's Shrine", "Traveler's Shrine");
            AddBidirectionalExitsWithOut(oNorthFork2, oTravelersShrine, "shrine");
            westOfBreeMap.Rooms[oTravelersShrine] = new PointF(12, -2);

            Room oNorthFork3 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork3, oNorthFork2, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oNorthFork3] = new PointF(11, -3);

            Room oNorthFork4 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork4, oNorthFork3, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oNorthFork4] = new PointF(10, -4);

            Room oNorthFork5 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork5, oNorthFork4, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oNorthFork5] = new PointF(10, -5);

            Room oNorthFork6 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork6, oNorthFork5, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oNorthFork6] = new PointF(10, -6);

            Room oNorthFork7 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork7, oNorthFork6, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oNorthFork7] = new PointF(10, -7);

            Room oNorthFork8 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork8, oNorthFork7, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oNorthFork8] = new PointF(10, -8);

            Room oNorthFork9 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork9, oNorthFork8, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oNorthFork9] = new PointF(10, -9);

            Room oNorthFork10 = AddRoom("North Fork Road", "North Fork Road");
            AddBidirectionalExits(oNorthFork10, oNorthFork9, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oNorthFork10] = new PointF(10, -10);

            Room oNorthForkEnd = AddRoom("North Fork End", "End of the North Fork Road");
            AddBidirectionalExits(oNorthForkEnd, oNorthFork10, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oNorthForkEnd] = new PointF(9, -11);

            Room oOvergrownRoad1 = AddRoom("Overgrown Road", "Overgrown Road");
            e = AddExit(oNorthForkEnd, oOvergrownRoad1, "brush");
            e.Hidden = true;
            AddExit(oOvergrownRoad1, oNorthForkEnd, "road");
            westOfBreeMap.Rooms[oOvergrownRoad1] = new PointF(9, -12);

            Room oOvergrownRoad2 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad2, oOvergrownRoad1, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oOvergrownRoad2] = new PointF(8, -12);

            Room oOvergrownRoad3 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad3, oOvergrownRoad2, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oOvergrownRoad3] = new PointF(7, -12);

            Room oOvergrownRoad4 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad4, oOvergrownRoad3, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad4] = new PointF(6, -13);

            Room oOvergrownRoad5 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad5, oOvergrownRoad4, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad5] = new PointF(5, -14);

            Room oOvergrownRoad6 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad6, oOvergrownRoad5, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad6] = new PointF(5, -15);

            Room oOvergrownRoad7 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad7, oOvergrownRoad6, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad7] = new PointF(4, -16);

            Room oOvergrownRoad8 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad8, oOvergrownRoad7, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad8] = new PointF(4, -17);

            Room oOvergrownRoad9 = AddRoom("Dwarven Wagoneer", "Overgrown Road");
            AddPermanentMobs(oOvergrownRoad9, MobTypeEnum.DwarvenWagoneer);
            AddBidirectionalExits(oOvergrownRoad9, oOvergrownRoad8, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad9] = new PointF(4, -18);

            Room oOvergrownRoad10 = AddRoom("Overturned Wagon", "Overgrown Road");
            AddPermanentMobs(oOvergrownRoad10, MobTypeEnum.DwarvenWagoneer);
            AddPermanentItems(oOvergrownRoad10, ItemTypeEnum.OverturnedWagon);
            AddBidirectionalExits(oOvergrownRoad10, oOvergrownRoad9, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad10] = new PointF(3, -19);

            Room oOvergrownRoad11 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad11, oOvergrownRoad10, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad11] = new PointF(3, -20);

            Room oGrassyFieldGlade = AddRoom("Grassy Field", "Grassy Field");
            AddExit(oOvergrownRoad11, oGrassyFieldGlade, "glade");
            AddExit(oGrassyFieldGlade, oOvergrownRoad11, "road");
            westOfBreeMap.Rooms[oGrassyFieldGlade] = new PointF(4, -20);

            Room oPalisadeGate = AddRoom("Palisade Gate", "Palisade Gate");
            AddPermanentMobs(oPalisadeGate, MobTypeEnum.RaiderGuard, MobTypeEnum.RaiderGuard);
            AddBidirectionalExits(oGrassyFieldGlade, oPalisadeGate, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oPalisadeGate] = new PointF(5, -20);

            Room oOvergrownRoad12 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad12, oOvergrownRoad11, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad12] = new PointF(3, -21);

            Room oOvergrownRoad13 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad13, oOvergrownRoad12, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad13] = new PointF(3, -22);

            Room oOvergrownRoad14 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad14, oOvergrownRoad13, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad14] = new PointF(3, -23);

            Room oOvergrownRoad15 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad15, oOvergrownRoad14, BidirectionalExitType.SouthwestNortheast);
            westOfBreeMap.Rooms[oOvergrownRoad15] = new PointF(4, -24);

            Room oQueensBridge = AddRoom("Queens Bridge", "Queens Bridge");
            AddBidirectionalExits(oQueensBridge, oOvergrownRoad15, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oQueensBridge] = new PointF(3, -25);

            Room oOvergrownRoad16 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad16, oQueensBridge, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad16] = new PointF(2, -26);

            Room oOvergrownRoad17 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad17, oOvergrownRoad16, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad17] = new PointF(2, -27);

            Room oOvergrownRoad18 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad18, oOvergrownRoad17, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad18] = new PointF(2, -28);

            Room oOvergrownRoad19 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad19, oOvergrownRoad18, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oOvergrownRoad19] = new PointF(2, -29);

            Room oOvergrownRoad20 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad20, oOvergrownRoad19, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad20] = new PointF(1, -30);

            Room oOvergrownRoad21 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad21, oOvergrownRoad20, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad21] = new PointF(0, -31);

            Room oOvergrownRoad22 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad22, oOvergrownRoad21, BidirectionalExitType.SouthwestNortheast);
            westOfBreeMap.Rooms[oOvergrownRoad22] = new PointF(1, -32);

            Room oOvergrownRoad23 = AddRoom("Overgrown Road", "Overgrown Road");
            AddBidirectionalExits(oOvergrownRoad23, oOvergrownRoad22, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oOvergrownRoad23] = new PointF(0, -33);

            Room oGigglersHill = AddRoom("Giggler's Hill", "Giggler's Hill");
            AddBidirectionalExits(oGigglersHill, oOvergrownRoad23, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oGigglersHill] = new PointF(0, -34);

            Room oGigglersHill2 = AddRoom("Giggler's Hill", "Giggler's Hill");
            AddBidirectionalExits(oGigglersHill2, oGigglersHill, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oGigglersHill2] = new PointF(-1, -35);

            Room oPrecipice = AddRoom("Precipice", "Precipice");
            AddBidirectionalExits(oPrecipice, oGigglersHill2, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oPrecipice] = new PointF(-2, -35);

            Room oWesternRoad1 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad1, oGrandIntersection, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad1] = new PointF(12, 0);

            Room oWesternRoad2 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad2, oWesternRoad1, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad2] = new PointF(11, 0);

            Room oWesternRoad3 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad3, oWesternRoad2, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad3] = new PointF(10, 0);

            Room oWesternRoad4 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad4, oWesternRoad3, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad4] = new PointF(9, 0);

            Room oWesternRoad5 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad5, oWesternRoad4, BidirectionalExitType.WestEast);
            //CSRTODO: north
            westOfBreeMap.Rooms[oWesternRoad5] = new PointF(8, 0);

            Room oWesternRoad6 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad6, oWesternRoad5, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad6] = new PointF(7, 0);

            Room oWesternRoad7 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad7, oWesternRoad6, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad7] = new PointF(6, 0);

            Room oWesternRoad8 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad8, oWesternRoad7, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad8] = new PointF(5, 0);

            Room oWesternRoad9 = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(oWesternRoad9, oWesternRoad8, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oWesternRoad9] = new PointF(4, 0);

            Room oVillageOfHobbiton1 = AddRoom("Village of Hobbiton", "The Village of Hobbiton");
            AddBidirectionalExits(oVillageOfHobbiton1, oWesternRoad9, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oVillageOfHobbiton1] = new PointF(3, 0);

            Room oMainSquareOfHobbiton = AddRoom("Main Square of Hobbiton", "Main Square of Hobbiton");
            AddBidirectionalExits(oMainSquareOfHobbiton, oVillageOfHobbiton1, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oMainSquareOfHobbiton] = new PointF(2, 0);

            Room oSpreadEagleTavern = AddRoom("Spread Eagle Tavern", "Spread Eagle Tavern");
            AddBidirectionalExits(oSpreadEagleTavern, oMainSquareOfHobbiton, BidirectionalExitType.SouthwestNortheast);
            westOfBreeMap.Rooms[oSpreadEagleTavern] = new PointF(3, -1);

            Room oShrapsLaundering = AddRoom("Shrap's Laundering", "Shrap's Laundering");
            AddPermanentMobs(oShrapsLaundering, MobTypeEnum.SoulOfClaudia);
            AddBidirectionalExits(oShrapsLaundering, oSpreadEagleTavern, BidirectionalExitType.UpDown);
            westOfBreeMap.Rooms[oShrapsLaundering] = new PointF(3, -1.25F);

            Room oVillageOfHobbiton2 = AddRoom("Village of Hobbiton", "The Village of Hobbiton");
            e = AddExit(oMainSquareOfHobbiton, oVillageOfHobbiton2, "south");
            e.MinimumLevel = 4;
            AddExit(oVillageOfHobbiton2, oMainSquareOfHobbiton, "north");
            westOfBreeMap.Rooms[oVillageOfHobbiton2] = new PointF(2, 1);

            Room arholTheMoneyChanger = AddRoom("Arhol Moneychanger", "Arhol the Money-Changer");
            AddBidirectionalExits(arholTheMoneyChanger, oVillageOfHobbiton2, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[arholTheMoneyChanger] = new PointF(1, 1);

            Room oHobbitonBuildingComplex = AddRoom("Building Complex", "Hobbiton Building Complex");
            AddExit(oVillageOfHobbiton2, oHobbitonBuildingComplex, "building");
            AddExit(oHobbitonBuildingComplex, oVillageOfHobbiton2, "street");
            westOfBreeMap.Rooms[oHobbitonBuildingComplex] = new PointF(3, 1);

            Room oVillageOfHobbiton3 = AddRoom("Village of Hobbiton", "The Village of Hobbiton");
            AddBidirectionalExits(oVillageOfHobbiton3, oMainSquareOfHobbiton, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oVillageOfHobbiton3] = new PointF(2, -1);

            Room oVillageOfHobbiton4 = AddRoom("Villlage of Hobbiton", "The Village of Hobbiton");
            AddBidirectionalExits(oVillageOfHobbiton4, oMainSquareOfHobbiton, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[oVillageOfHobbiton4] = new PointF(1, 0);

            Room oHobbitonMarketSquare = AddRoom("Market Square", "Market Square");
            AddBidirectionalExits(oHobbitonMarketSquare, oVillageOfHobbiton4, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oHobbitonMarketSquare] = new PointF(1, -0.5F);

            Room oGatheringOfAddicts = AddRoom("Addict Gathering", "Gathering of Addicts");
            AddBidirectionalExits(oVillageOfHobbiton4, oGatheringOfAddicts, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[oGatheringOfAddicts] = new PointF(1, 0.5F);

            westernRoadWestOfHobbiton = AddRoom("Western Road", "Western Road");
            AddBidirectionalExits(westernRoadWestOfHobbiton, oVillageOfHobbiton4, BidirectionalExitType.WestEast);
            westOfBreeMap.Rooms[westernRoadWestOfHobbiton] = new PointF(0, 0);

            AddNarwe(westernRoadWestOfHobbiton);

            westronRoadToMithlond = AddRoom("Westron Road", "Westron Road");
            AddBidirectionalExits(westronRoadToMithlond, oVillageOfHobbiton3, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[westronRoadToMithlond] = new PointF(2, -2);
            mithlondGraph.Rooms[oVillageOfHobbiton3] = new PointF(9, 20);
            AddMapBoundaryPoint(westronRoadToMithlond, oVillageOfHobbiton3, MapType.Mithlond, MapType.WestOfBree);

            valleyRoad = AddRoom("Valley Road", "Valley Road");
            AddBidirectionalExits(oVillageOfHobbiton2, valleyRoad, BidirectionalExitType.NorthSouth);
            westOfBreeMap.Rooms[valleyRoad] = new PointF(2, 2);

            Room oHillAtBagEnd = AddRoom("Bag End Hill", "The Hill at Bag End");
            AddBidirectionalExits(valleyRoad, oHillAtBagEnd, BidirectionalExitType.SoutheastNorthwest);
            westOfBreeMap.Rooms[oHillAtBagEnd] = new PointF(3, 2);

            Room oBilboFrodoHobbitHoleCondo = AddRoom("Hobbit Hole Condo", "Bilbo and Frodo's Hobbit Hole Condo");
            AddBidirectionalExitsWithOut(oHillAtBagEnd, oBilboFrodoHobbitHoleCondo, "down");
            westOfBreeMap.Rooms[oBilboFrodoHobbitHoleCondo] = new PointF(3, 2.25F);

            Room oShortHallway = AddRoom("Short Hallway", "Short Hallway");
            AddExit(oBilboFrodoHobbitHoleCondo, oShortHallway, "hallway");
            AddExit(oShortHallway, oBilboFrodoHobbitHoleCondo, "entryway");
            westOfBreeMap.Rooms[oShortHallway] = new PointF(4, 2.25F);

            Room oStorageRoom = AddRoom("Storage Room", "Storage Room");
            AddBidirectionalSameNameMustOpenExit(oShortHallway, oStorageRoom, "door");
            westOfBreeMap.Rooms[oStorageRoom] = new PointF(4, 2);

            Room oBilboFrodoCommonArea = AddRoom("Common Area", "Common Area");
            AddBidirectionalSameNameExit(oBilboFrodoHobbitHoleCondo, oBilboFrodoCommonArea, "curtain");
            westOfBreeMap.Rooms[oBilboFrodoCommonArea] = new PointF(3, 2.5F);

            Room oPrivateStudy = AddRoom("Private Study", "Bilbo's Private Study");
            AddBidirectionalSameNameMustOpenExit(oBilboFrodoCommonArea, oPrivateStudy, "door");
            westOfBreeMap.Rooms[oPrivateStudy] = new PointF(2, 2.5F);

            Room oEastwingHallway = AddRoom("Eastwing Hallway", "Eastwing Hallway");
            AddExit(oBilboFrodoCommonArea, oEastwingHallway, "eastwing");
            AddExit(oEastwingHallway, oBilboFrodoCommonArea, "common");
            westOfBreeMap.Rooms[oEastwingHallway] = new PointF(4, 2.5F);

            Room oBelladonnaTook = AddRoom("Kitchen", "Kitchen");
            AddPermanentMobs(oBelladonnaTook, MobTypeEnum.BelladonaTook);
            AddExit(oEastwingHallway, oBelladonnaTook, "doorway");
            AddExit(oBelladonnaTook, oEastwingHallway, "hallway");
            westOfBreeMap.Rooms[oBelladonnaTook] = new PointF(5, 2.5F);

            Room oSouthwingHallway = AddRoom("Southwing Hallway", "Southwing Hallway");
            AddExit(oEastwingHallway, oSouthwingHallway, "southwing");
            AddExit(oSouthwingHallway, oEastwingHallway, "eastwing");
            westOfBreeMap.Rooms[oSouthwingHallway] = new PointF(3, 2.75F);

            Room oBilboBaggins = AddRoom("Bilbo Baggins", "Bilbo's Living Quarters");
            AddPermanentMobs(oBilboBaggins, MobTypeEnum.BilboBaggins);
            AddBidirectionalSameNameMustOpenExit(oSouthwingHallway, oBilboBaggins, "oakdoor");
            westOfBreeMap.Rooms[oBilboBaggins] = new PointF(3, 3);

            Room oFrodoBaggins = AddRoom("Frodo Baggins", "Frodo's Living Quarters");
            AddPermanentMobs(oFrodoBaggins, MobTypeEnum.FrodoBaggins);
            AddBidirectionalSameNameExit(oSouthwingHallway, oFrodoBaggins, "curtain");
            westOfBreeMap.Rooms[oFrodoBaggins] = new PointF(4, 2.75F);

            Room oGreatHallOfHeroes = AddRoom("Great Hall of Heroes", "The Great Hall of Heroes.");
            AddPermanentMobs(oGreatHallOfHeroes, MobTypeEnum.DenethoreTheWise);
            AddBidirectionalExitsWithOut(oGrandIntersection, oGreatHallOfHeroes, "hall");
            westOfBreeMap.Rooms[oGreatHallOfHeroes] = new PointF(13, 0.5F);

            //something is hasted
            Room oSomething = AddRoom("Something", "Kitchen");
            //CSRTODO
            //oSomething.Mob1 = "Something";
            //oSomething.Experience1 = 140;
            e = AddExit(oGreatHallOfHeroes, oSomething, "curtain");
            e.MaximumLevel = 10;
            e.Hidden = true;
            AddExit(oSomething, oGreatHallOfHeroes, "curtain");
            westOfBreeMap.Rooms[oSomething] = new PointF(13, 1);

            Room oShepherd = AddRoom("Shepherd", "Pasture");
            AddPermanentMobs(oShepherd, MobTypeEnum.Shepherd);
            AddExit(oNorthFork1, oShepherd, "pasture");
            AddExit(oShepherd, oNorthFork1, "south");
            westOfBreeMap.Rooms[oShepherd] = new PointF(13, -2);
            breeSewersMap.Rooms[oShepherd] = new PointF(1, -2);

            AddExit(oSmoulderingVillage, oShepherd, "gate");
            e = AddExit(oShepherd, oSmoulderingVillage, "gate");
            e.KeyType = SupportedKeysFlags.GateKey;
            westOfBreeMap.Rooms[oSmoulderingVillage] = new PointF(13, -2.5F);
            AddMapBoundaryPoint(oShepherd, oSmoulderingVillage, MapType.WestOfBree, MapType.BreeSewers);
        }

        private void AddNarwe(Room oWesternRoadFromHobbiton)
        {
            RoomGraph narweGraph = _graphs[MapType.Narwe];
            RoomGraph westOfBreeGraph = _graphs[MapType.WestOfBree];

            narweGraph.Rooms[oWesternRoadFromHobbiton] = new PointF(9, 5);

            Room oDirtRoad1 = AddRoom("Dirt Road", "Dirt Road");
            AddBidirectionalExits(oDirtRoad1, oWesternRoadFromHobbiton, BidirectionalExitType.WestEast);
            westOfBreeGraph.Rooms[oDirtRoad1] = new PointF(-1, 0);
            narweGraph.Rooms[oDirtRoad1] = new PointF(8, 5);
            AddMapBoundaryPoint(oDirtRoad1, oWesternRoadFromHobbiton, MapType.Narwe, MapType.WestOfBree);

            Room oDirtRoad2 = AddRoom("Dirt Road", "Dirt Road");
            AddBidirectionalExits(oDirtRoad2, oDirtRoad1, BidirectionalExitType.WestEast);
            narweGraph.Rooms[oDirtRoad2] = new PointF(7, 5);

            Room oDirtRoad3 = AddRoom("Dirt Road", "Dirt Road");
            AddBidirectionalExits(oDirtRoad3, oDirtRoad2, BidirectionalExitType.WestEast);
            narweGraph.Rooms[oDirtRoad3] = new PointF(6, 5);

            Room oDirtRoad4 = AddRoom("Dirt Road", "Dirt Road");
            AddBidirectionalExits(oDirtRoad4, oDirtRoad3, BidirectionalExitType.WestEast);
            narweGraph.Rooms[oDirtRoad4] = new PointF(5, 5);

            Room oFallenCityOfNarweEntrance = AddRoom("Narwe Entrance", "The Fallen City of Narwe");
            Exit e = AddExit(oDirtRoad4, oFallenCityOfNarweEntrance, "south");
            e.Hidden = true;
            AddExit(oFallenCityOfNarweEntrance, oDirtRoad4, "north");
            narweGraph.Rooms[oFallenCityOfNarweEntrance] = new PointF(5, 6);

            Room oNarwe2 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddBidirectionalExits(oFallenCityOfNarweEntrance, oNarwe2, BidirectionalExitType.WestEast);
            AddExit(oNarwe2, oFallenCityOfNarweEntrance, "north");
            narweGraph.Rooms[oNarwe2] = new PointF(6, 6);

            Room oNarwe3 = AddRoom("Narwe", "The Fallen City of Narwe"); //can have north, south, east, west exits, with east/west/south not always present.
            AddBidirectionalExits(oFallenCityOfNarweEntrance, oNarwe3, BidirectionalExitType.NorthSouth);
            AddExit(oNarwe3, oDirtRoad4, "east"); //exit not always present
            AddExit(oFallenCityOfNarweEntrance, oNarwe3, "west");
            narweGraph.Rooms[oNarwe3] = new PointF(5, 7);

            Room oNarwe4 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddExit(oNarwe3, oNarwe4, "west"); //exit not always present
            AddExit(oNarwe4, oNarwe2, "east");
            narweGraph.Rooms[oNarwe4] = new PointF(4, 7);

            Room oNarwe5 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddBidirectionalExits(oNarwe4, oNarwe5, BidirectionalExitType.NorthSouth);
            AddExit(oNarwe5, oNarwe2, "east");
            narweGraph.Rooms[oNarwe5] = new PointF(4, 8);

            Room oNarwe6 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddExit(oNarwe4, oNarwe6, "north");
            narweGraph.Rooms[oNarwe6] = new PointF(4, 6);

            Room oNarwe7 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddExit(oNarwe3, oNarwe7, "south"); //exit not always present
            narweGraph.Rooms[oNarwe7] = new PointF(5, 8);
            //can have east, west, north, south exits

            Room oNarwe8 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddExit(oNarwe7, oNarwe8, "west");
            AddExit(oNarwe8, oNarwe7, "north");
            AddExit(oNarwe8, oNarwe3, "west");
            narweGraph.Rooms[oNarwe8] = new PointF(4, 8);

            Room oNarwe9 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddExit(oNarwe8, oNarwe9, "east");
            AddExit(oNarwe7, oNarwe9, "east"); //not always present
            AddExit(oNarwe9, oNarwe7, "west");
            AddExit(oNarwe9, oNarwe6, "south");
            narweGraph.Rooms[oNarwe9] = new PointF(5, 8);

            Room oNarwe10 = AddRoom("Narwe", "The Fallen City of Narwe");
            AddExit(oNarwe7, oNarwe10, "south");
            AddExit(oNarwe10, oNarwe9, "north");
            narweGraph.Rooms[oNarwe10] = new PointF(5, 9);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside, out Room oTharbadGateOutside, Room southernBrethilNWEdge, Room southernBrethilForestSW, Room southernBrethilForestSE, Room southernBrethilForestNE)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];
            RoomGraph imladrisToTharbadGraph = _graphs[MapType.ImladrisToTharbad];
            RoomGraph spindrilsCastleLevel1Graph = _graphs[MapType.SpindrilsCastleLevel1];
            RoomGraph tharbadGraph = _graphs[MapType.Tharbad];

            Room oMistyTrail1 = AddRoom("South Gate Outside", "Misty Trail");
            AddBidirectionalSameNameExit(oImladrisSouthGateInside, oMistyTrail1, "gate");
            imladrisGraph.Rooms[oMistyTrail1] = new PointF(5, 11);
            imladrisToTharbadGraph.Rooms[oMistyTrail1] = new PointF(5, 0);
            imladrisToTharbadGraph.Rooms[oImladrisSouthGateInside] = new PointF(5, -1);
            AddMapBoundaryPoint(oImladrisSouthGateInside, oMistyTrail1, MapType.Imladris, MapType.ImladrisToTharbad);

            Room oBrunskidTradersGuild1 = AddRoom("Brunskid Guild", "Brunskid Trader's Guild Store Front");
            AddBidirectionalExits(oBrunskidTradersGuild1, oMistyTrail1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oBrunskidTradersGuild1] = new PointF(4, 11);
            imladrisToTharbadGraph.Rooms[oBrunskidTradersGuild1] = new PointF(4, 0);
            AddRoomMapDisambiguation(oBrunskidTradersGuild1, MapType.ImladrisToTharbad); //on imladris graph for convenience

            Room oGuildmaster = AddRoom("Guildmaster", "Brunskid Trader's Guild Office");
            AddPermanentMobs(oGuildmaster, MobTypeEnum.Guildmaster);
            AddBidirectionalExits(oGuildmaster, oBrunskidTradersGuild1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oGuildmaster] = new PointF(3, 11);
            imladrisToTharbadGraph.Rooms[oGuildmaster] = new PointF(3, 0);
            AddRoomMapDisambiguation(oGuildmaster, MapType.ImladrisToTharbad); //on imladris graph for convenience

            Room oCutthroatAssassin = AddRoom("Hiester", "Brunskid Trader's Guild Acquisitions Room");
            AddBidirectionalExits(oCutthroatAssassin, oGuildmaster, BidirectionalExitType.WestEast);
            AddPermanentMobs(oCutthroatAssassin, MobTypeEnum.GregoryHiester, MobTypeEnum.MasterAssassin, MobTypeEnum.Cutthroat);
            imladrisGraph.Rooms[oCutthroatAssassin] = new PointF(2, 11);
            imladrisToTharbadGraph.Rooms[oCutthroatAssassin] = new PointF(2, 0);
            AddRoomMapDisambiguation(oCutthroatAssassin, MapType.ImladrisToTharbad); //on imladris graph for convenience

            Room oMistyTrail2 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail1, oMistyTrail2, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail2] = new PointF(5, 0.5F);

            Room oMistyTrail3 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail2, oMistyTrail3, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail3] = new PointF(5, 1);

            Room oMistyTrail4 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail3, oMistyTrail4, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail4] = new PointF(4, 1.5F);

            AddPotionFactory(oMistyTrail4);

            Room oMistyTrail5 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail4, oMistyTrail5, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail5] = new PointF(4, 4);

            Room oMistyTrail6 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail5, oMistyTrail6, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail6] = new PointF(4, 5);

            AddSouthernBrethilForest(oMistyTrail6, southernBrethilNWEdge, southernBrethilForestSW, southernBrethilForestSE, southernBrethilForestNE);

            Room oMistyTrail7 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail6, oMistyTrail7, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail7] = new PointF(4, 6);

            Room oMistyTrail8 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail7, oMistyTrail8, BidirectionalExitType.NorthSouth);
            AddShantyTown(oMistyTrail8);
            imladrisToTharbadGraph.Rooms[oMistyTrail8] = new PointF(4, 7);

            Room oMistyTrail9 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail8, oMistyTrail9, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail9] = new PointF(4, 8);

            Room oMistyTrail10 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail9, oMistyTrail10, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail10] = new PointF(3, 9);

            Room oMistyTrail11 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail10, oMistyTrail11, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail11] = new PointF(2, 10);

            Room oMistyTrail12 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail11, oMistyTrail12, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail12] = new PointF(2, 11);

            Room oMistyTrail13 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail12, oMistyTrail13, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail13] = new PointF(1, 12);

            Room oMistyTrail14 = AddRoom("Misty Trail", "Misty Trail");
            AddBidirectionalExits(oMistyTrail13, oMistyTrail14, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail14] = new PointF(0, 13);
            tharbadGraph.Rooms[oMistyTrail14] = new PointF(3, 0);

            Room oGrassyField = AddRoom("Grassy Field", "Grassy Field");
            AddNonPermanentMobs(oGrassyField, MobTypeEnum.Griffon);
            AddBidirectionalExits(oGrassyField, oMistyTrail14, BidirectionalExitType.SoutheastNorthwest);
            imladrisToTharbadGraph.Rooms[oGrassyField] = new PointF(-1, 12);
            spindrilsCastleLevel1Graph.Rooms[oGrassyField] = new PointF(11, 10.5F);

            Room spindrilsCastleOutside = AddRoom("Dark Clouds", "Dark Clouds");
            Exit e = AddExit(oGrassyField,spindrilsCastleOutside, "up");
            e.Hidden = true;
            e.PresenceType = ExitPresenceType.RequiresSearch;
            AddExit(spindrilsCastleOutside, oGrassyField, "down");
            imladrisToTharbadGraph.Rooms[spindrilsCastleOutside] = new PointF(-1, 11);
            AddMapBoundaryPoint(oGrassyField, spindrilsCastleOutside, MapType.ImladrisToTharbad, MapType.SpindrilsCastleLevel1);

            AddSpindrilsCastle(spindrilsCastleOutside);

            oTharbadGateOutside = AddRoom("North Gate", "North Gate of Tharbad");
            AddBidirectionalExits(oMistyTrail14, oTharbadGateOutside, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oTharbadGateOutside] = new PointF(0, 14);
            AddMapBoundaryPoint(oMistyTrail14, oTharbadGateOutside, MapType.ImladrisToTharbad, MapType.Tharbad);
        }

        private void AddPotionFactory(Room oMistyTrail4)
        {
            RoomGraph imladrisToTharbadGraph = _graphs[MapType.ImladrisToTharbad];

            Room oPotionFactoryReception = AddRoom("Reception Area", "Reception Area of Potion Factory");
            AddPermanentMobs(oPotionFactoryReception, MobTypeEnum.Guard);
            AddBidirectionalExits(oPotionFactoryReception, oMistyTrail4, BidirectionalExitType.WestEast);
            imladrisToTharbadGraph.Rooms[oPotionFactoryReception] = new PointF(3, 1.5F);

            Room oPotionFactoryAdministrativeOffices = AddRoom("Admin Offices", "Administrative Offices");
            AddBidirectionalExits(oPotionFactoryReception, oPotionFactoryAdministrativeOffices, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oPotionFactoryAdministrativeOffices] = new PointF(3, 2);

            Room oMarkFrey = AddRoom("Mark Frey", "Potent Potions, Inc.");
            AddPermanentMobs(oMarkFrey, MobTypeEnum.MarkFrey);
            AddBidirectionalExitsWithOut(oPotionFactoryAdministrativeOffices, oMarkFrey, "door");
            imladrisToTharbadGraph.Rooms[oMarkFrey] = new PointF(3, 2.5F);

            Room oAuctionRoom = AddRoom("Auction Room", "Auction Room");
            AddBidirectionalSameNameExit(oPotionFactoryReception, oAuctionRoom, "door");
            imladrisToTharbadGraph.Rooms[oAuctionRoom] = new PointF(3, 1);

            Room oPotionFactory = AddRoom("Potion Factory", "Potion Factory");
            Exit e = AddBidirectionalExitsWithOut(oPotionFactoryReception, oPotionFactory, "factory");
            e.KeyType = SupportedKeysFlags.FactoryKey;
            e.MustOpen = true;
            imladrisToTharbadGraph.Rooms[oPotionFactory] = new PointF(2, 1.5F);

            Room oPotionFactory2 = AddRoom("Potion Factory", "Potion Factory");
            AddBidirectionalExits(oPotionFactory2, oPotionFactory, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oPotionFactory2] = new PointF(2, 1);

            Room oResearchLaboratory = AddRoom("Research Laboratory", "Research Laboratory");
            AddBidirectionalExitsWithOut(oPotionFactory2, oResearchLaboratory, "laboratory");
            imladrisToTharbadGraph.Rooms[oResearchLaboratory] = new PointF(2, 0.5F);

            Room oReceivingDock = AddRoom("Receiving Dock", "Receiving Dock");
            AddExit(oPotionFactory, oReceivingDock, "receiving");
            AddExit(oReceivingDock, oPotionFactory, "warehouse");
            imladrisToTharbadGraph.Rooms[oReceivingDock] = new PointF(2, 2);
        }

        private void AddSouthernBrethilForest(Room oMistyTrail, Room southernBrethilNWEdge, Room southernBrethilForestSW, Room southernBrethilForestSE, Room southernBrethilForestNE)
        {
            RoomGraph imladrisToTharbadGraph = _graphs[MapType.ImladrisToTharbad];

            Room oMistyTrailForest = AddRoom("Forest", "Southern Brethil Forest");
            AddExit(oMistyTrail, oMistyTrailForest, "forest");
            AddExit(oMistyTrailForest, oMistyTrail, "southeast");
            AddExit(oMistyTrailForest, southernBrethilNWEdge, "west");
            AddExit(oMistyTrailForest, southernBrethilForestSW, "southwest");
            imladrisToTharbadGraph.Rooms[oMistyTrailForest] = new PointF(1, 4.5F);

            Room oAncientForest = AddRoom("Ancient Forest", "Ancient Brethil Forest");
            AddBidirectionalExits(oAncientForest, oMistyTrailForest, BidirectionalExitType.NorthSouth);
            AddExit(oAncientForest, southernBrethilNWEdge, "west");
            AddExit(oAncientForest, southernBrethilForestSW, "southeast");
            imladrisToTharbadGraph.Rooms[oAncientForest] = new PointF(1, 3.5F);

            Room oSouthernBrethilForest = AddRoom("Forest", "Southern Brethil Forest");
            AddExit(oAncientForest, oSouthernBrethilForest, "east");
            AddExit(oSouthernBrethilForest, southernBrethilForestSE, "southeast");
            AddExit(oSouthernBrethilForest, southernBrethilForestNE, "southwest");
            imladrisToTharbadGraph.Rooms[oSouthernBrethilForest] = new PointF(2, 3.5F);

            Room oAncientForest2 = AddRoom("Ancient Forest", "Ancient Forest");
            AddBidirectionalExits(oAncientForest2, oSouthernBrethilForest, BidirectionalExitType.NorthSouth);
            AddExit(oAncientForest2, southernBrethilForestSE, "west");
            AddExit(oAncientForest2, southernBrethilNWEdge, "east");
            AddExit(oAncientForest2, southernBrethilForestNE, "northeast");
            imladrisToTharbadGraph.Rooms[oAncientForest2] = new PointF(2, 2.5F);
        }

        private void AddNorthOfEsgaroth(Room esgarothNorthGateOutside)
        {
            RoomGraph northOfEsgarothGraph = _graphs[MapType.NorthOfEsgaroth];

            northOfEsgarothGraph.Rooms[esgarothNorthGateOutside] = new PointF(5, 10);

            Room mountainTrail1 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail1, esgarothNorthGateOutside, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail1] = new PointF(6, 9);

            Room mountainTrail2 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail2, mountainTrail1, BidirectionalExitType.NorthSouth);
            northOfEsgarothGraph.Rooms[mountainTrail2] = new PointF(6, 8);

            Room mountainTrail3 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail3, mountainTrail2, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail3] = new PointF(5, 7);

            Room mountainTrail4 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail4, mountainTrail3, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail4] = new PointF(6, 6);

            Room mountainTrail5 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail5, mountainTrail4, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail5] = new PointF(5, 5);

            Room mountainTrail6 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail6, mountainTrail5, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail6] = new PointF(6, 4);

            Room mountainTrail7 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail6, mountainTrail7, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrail7] = new PointF(7, 4);

            Room mountainTrail8 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail7, mountainTrail8, BidirectionalExitType.NorthSouth);
            northOfEsgarothGraph.Rooms[mountainTrail8] = new PointF(7, 5);

            Room mountainTrail9 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail8, mountainTrail9, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail9] = new PointF(10, 6);

            Room mountainTrail10 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail9, mountainTrail10, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrail10] = new PointF(9, 7);

            Room mountainTrail11 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail11, mountainTrail10, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrail11] = new PointF(8, 7);

            Room mountainTrail12 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail11, mountainTrail12, BidirectionalExitType.NorthSouth);
            northOfEsgarothGraph.Rooms[mountainTrail12] = new PointF(8, 9);

            Room mountainTrail13 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrail13, mountainTrail12, BidirectionalExitType.WestEast);
            AddBidirectionalExits(mountainTrail2, mountainTrail13, BidirectionalExitType.SoutheastNorthwest);
            northOfEsgarothGraph.Rooms[mountainTrail13] = new PointF(7, 9);

            Room mountainTrailWest1 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest1, mountainTrail3, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrailWest1] = new PointF(2, 7);

            Room mountainTrailWest2 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest2, mountainTrailWest1, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrailWest2] = new PointF(2.5F, 6.5F);

            Room mountainTrailWest3 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest3, mountainTrailWest2, BidirectionalExitType.SouthwestNortheast);
            northOfEsgarothGraph.Rooms[mountainTrailWest3] = new PointF(3, 6);

            Room mountainTrailWest4 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest3, mountainTrailWest4, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrailWest4] = new PointF(4, 6);

            Room mountainTrailWest5 = AddRoom("Trail", "Mountain Trail");
            AddBidirectionalExits(mountainTrailWest5, mountainTrailWest3, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(mountainTrailWest5, mountainTrail5, BidirectionalExitType.WestEast);
            northOfEsgarothGraph.Rooms[mountainTrailWest5] = new PointF(4, 5);

            Room ambush = AddRoom("Ambush", "Ambush!");
            AddPermanentMobs(ambush, MobTypeEnum.BarbarianGuard, MobTypeEnum.BarbarianGuard, MobTypeEnum.HillGiant, MobTypeEnum.HillGiant, MobTypeEnum.EvilSorcerer, MobTypeEnum.MercenaryCaptain);
            Exit e = AddBidirectionalExitsWithOut(mountainTrailWest4, ambush, "brush");
            e.Hidden = true;
            northOfEsgarothGraph.Rooms[ambush] = new PointF(4, 5.5F);

            Room disfiguredStatue = AddRoom("Disfigured Statue", "Disfigured Statue");
            AddBidirectionalExitsWithOut(mountainTrail3, disfiguredStatue, "disfigured"); //says disfigured statue but statue doesn't work
            northOfEsgarothGraph.Rooms[disfiguredStatue] = new PointF(6, 7);
        }

        private void AddEsgaroth(Room westGateOfEsgaroth, out Room northGateOutside)
        {
            RoomGraph esgarothGraph = _graphs[MapType.Esgaroth];
            RoomGraph eastOfImladrisGraph = _graphs[MapType.EastOfImladris];
            RoomGraph northOfEsgarothGraph = _graphs[MapType.NorthOfEsgaroth];

            esgarothGraph.Rooms[westGateOfEsgaroth] = new PointF(0, 7);

            Room plymouthIndigo = AddRoom("Plymouth/Indigo", "Plymouth Road/Indigo Avenue Intersection");
            AddBidirectionalSameNameExit(westGateOfEsgaroth, plymouthIndigo, "gate");
            esgarothGraph.Rooms[plymouthIndigo] = new PointF(1, 6);
            eastOfImladrisGraph.Rooms[plymouthIndigo] = new PointF(10, 4);
            AddMapBoundaryPoint(westGateOfEsgaroth, plymouthIndigo, MapType.EastOfImladris, MapType.Esgaroth);

            Room cathedralEntrance = AddRoom("Cathedral Entrance", "Cathedral of Worldly Bliss Court");
            AddExit(plymouthIndigo, cathedralEntrance, "cathedral");
            AddExit(cathedralEntrance, plymouthIndigo, "east");
            esgarothGraph.Rooms[cathedralEntrance] = new PointF(0, 6);

            Room cathedralNorthFoyer = AddRoom("Foyer", "Northern Entrance Foyer");
            AddBidirectionalExits(cathedralNorthFoyer, cathedralEntrance, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[cathedralNorthFoyer] = new PointF(0, 5.5F);

            Room cathedralSouthFoyer = AddRoom("Foyer", "Southern Entrance Foyer");
            AddBidirectionalExits(cathedralEntrance, cathedralSouthFoyer, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[cathedralSouthFoyer] = new PointF(0, 6.5F);

            Room insideCathedral = AddRoom("Cathedral", "Cathedral of Worldly Bliss");
            AddExit(cathedralNorthFoyer, insideCathedral, "door");
            AddExit(cathedralSouthFoyer, insideCathedral, "door");
            AddExit(insideCathedral, cathedralNorthFoyer, "north door");
            AddExit(insideCathedral, cathedralSouthFoyer, "south door");
            esgarothGraph.Rooms[insideCathedral] = new PointF(-2, 6);

            Room alliyana = AddRoom("Alliyana", "Matriarch Alliyana's Quarters");
            AddPermanentMobs(alliyana, MobTypeEnum.MatriarchAlliyanaOfIsengard);
            Exit e = AddExit(insideCathedral, alliyana, "east door");
            e.Hidden = true;
            e.MustOpen = true;
            e = AddExit(alliyana, insideCathedral, "out");
            e.MustOpen = true;
            esgarothGraph.Rooms[alliyana] = new PointF(-1, 6);

            Room holyBankOfEsgaroth = AddRoom("Holy Bank", "Holy Bank of Esgaroth");
            AddBidirectionalExits(plymouthIndigo, holyBankOfEsgaroth, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[holyBankOfEsgaroth] = new PointF(1, 7);

            Room plymouthMagenta = AddRoom("Plymouth/Magenta", "Plymouth Road/Magenta Avenue Intersection");
            AddBidirectionalExits(plymouthIndigo, plymouthMagenta, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthMagenta] = new PointF(2, 6);

            Room plymouthGardenCir = AddRoom("Plymouth/Garden", "Plymouth Road/Garden Circle West");
            AddBidirectionalExits(plymouthMagenta, plymouthGardenCir, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthGardenCir] = new PointF(3, 6);

            Room plymouthAquamarine = AddRoom("Plymouth/Aquamarine", "Plymouth Road/Aquamarine Way");
            AddBidirectionalExits(plymouthGardenCir, plymouthAquamarine, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthAquamarine] = new PointF(4, 6);

            Room plymouthFuchsia = AddRoom("Plymouth/Fuchsia", "Plymouth Road/Fuchsia Way");
            AddBidirectionalExits(plymouthAquamarine, plymouthFuchsia, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[plymouthFuchsia] = new PointF(8, 6);

            Room theaterGrassSeating = AddRoom("Theater Grass Seating", "Esgaroth Amphitheather Grass Seating");
            AddBidirectionalExits(plymouthFuchsia, theaterGrassSeating, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[theaterGrassSeating] = new PointF(9, 6);

            Room briarIndigo = AddRoom("Briar/Indigo", "Briar Lane/Indigo Avenue Intersection");
            AddBidirectionalExits(briarIndigo, plymouthIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarIndigo] = new PointF(1, 5);

            Room briarMagenta = AddRoom("Briar/Magenta", "Briar Lane/Magenta Avenue Intersection");
            AddBidirectionalExits(briarIndigo, briarMagenta, BidirectionalExitType.WestEast);
            AddBidirectionalExits(briarMagenta, plymouthMagenta, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarMagenta] = new PointF(2, 5);

            Room briarLane = AddRoom("Briar", "Briar Lane");
            AddBidirectionalExits(briarMagenta, briarLane, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[briarLane] = new PointF(3, 5);

            Room parthTowers = AddRoom("Parth Towers", "Entrance to the Parth Towers");
            AddPermanentItems(parthTowers, ItemTypeEnum.BlackRune, ItemTypeEnum.BlueRune, ItemTypeEnum.GreenRune, ItemTypeEnum.GreyRune);
            AddBidirectionalExits(parthTowers, briarLane, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[parthTowers] = new PointF(3, 4.5F);
            //CSRTODO: drawbridge

            Room briarAquamarine = AddRoom("Briar/Aquamarine", "Briar Lane/Aquamarine Way Intersection");
            AddBidirectionalExits(briarLane, briarAquamarine, BidirectionalExitType.WestEast);
            AddBidirectionalExits(briarAquamarine, plymouthAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarAquamarine] = new PointF(4, 5);

            Room briarLane2 = AddRoom("Briar", "Briar Lane");
            AddBidirectionalExits(briarAquamarine, briarLane2, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[briarLane2] = new PointF(6, 5);
            AddEsgarothMuseum(briarLane2, esgarothGraph);

            Room briarFuchsia = AddRoom("Briar/Fuchsia", "Briar Lane/Fuchsia Way Intersection");
            AddBidirectionalExits(briarLane2, briarFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(briarFuchsia, plymouthFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[briarFuchsia] = new PointF(8, 5);

            Room theaterMainSeating = AddRoom("Theater Main Seating", "Esgaroth Amphitheater Main Seating");
            AddPermanentMobs(theaterMainSeating, MobTypeEnum.MinstrelOfEsgaroth);
            AddBidirectionalExits(briarFuchsia, theaterMainSeating, BidirectionalExitType.WestEast);
            AddBidirectionalExits(theaterMainSeating, theaterGrassSeating, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[theaterMainSeating] = new PointF(9, 5);

            Room dragonpawFuchsia = AddRoom("Dragonpaw/Fuchsia", "Dragonpaw Lane/Fuchsia Way Intersection");
            AddBidirectionalExits(dragonpawFuchsia, briarFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[dragonpawFuchsia] = new PointF(8, 4.5F);

            Room dragonpaw = AddRoom("Dragonpaw", "Dragonpaw Lane");
            AddBidirectionalExits(dragonpawFuchsia, dragonpaw, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[dragonpaw] = new PointF(9, 4.5F);

            Room theaterStage = AddRoom("Theater Stage", "Esgaroth Amphitheater Stage");
            AddBidirectionalExits(dragonpaw, theaterStage, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(theaterStage, theaterMainSeating, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[theaterStage] = new PointF(9, 4.75F);

            Room sweetwaterIndigo = AddRoom("Sweetwater/Indigo", "Sweetwater Lane/Indigo Avenue Intersection");
            AddBidirectionalExits(sweetwaterIndigo, briarIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterIndigo] = new PointF(1, 4);

            Room sweetwaterMagenta = AddRoom("Sweetwater/Magenta", "Sweetwater Lane/Magenta Avenue Intersection");
            AddBidirectionalExits(sweetwaterIndigo, sweetwaterMagenta, BidirectionalExitType.WestEast);
            AddBidirectionalExits(sweetwaterMagenta, briarMagenta, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterMagenta] = new PointF(2, 4);

            Room sweetwaterAquamarine = AddRoom("Sweetwater/Aquamarine", "Sweetwater Lane/Aquamarine Way Intersection");
            AddBidirectionalExits(sweetwaterMagenta, sweetwaterAquamarine, BidirectionalExitType.WestEast);
            AddBidirectionalExits(sweetwaterAquamarine, briarAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterAquamarine] = new PointF(4, 4);

            Room sweetwaterLane = AddRoom("Sweetwater", "Sweetwater Lane");
            AddBidirectionalExits(sweetwaterAquamarine, sweetwaterLane, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[sweetwaterLane] = new PointF(6, 4);

            Room marketplaceLane = AddRoom("Marketplace", "Marketplace Lane");
            AddBidirectionalExits(sweetwaterLane, marketplaceLane, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(marketplaceLane, briarLane2, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[marketplaceLane] = new PointF(6, 4.5F);

            Room marketplaceAlley = AddRoom("Alley", "Marketplace Alley");
            AddBidirectionalExits(marketplaceAlley, marketplaceLane, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[marketplaceAlley] = new PointF(5, 4.5F);

            Room gypsyRow = AddRoom("Gypsy Row", "Gypsy Row");
            AddBidirectionalExits(marketplaceLane, gypsyRow, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[gypsyRow] = new PointF(7, 4.5F);

            Room sweetwaterFuchsia = AddRoom("Sweetwater/Fuchsia", "Sweetwater Lane/Fuchsia Way");
            AddBidirectionalExits(sweetwaterLane, sweetwaterFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(sweetwaterFuchsia, dragonpawFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[sweetwaterFuchsia] = new PointF(8, 4);

            Room oEsgarothPawnShop = AddPawnShoppeRoom("Pawnshop", "Esgaroth Pawnshop", PawnShoppe.Esgaroth);
            AddBidirectionalSameNameExit(sweetwaterFuchsia, oEsgarothPawnShop, "door");
            esgarothGraph.Rooms[oEsgarothPawnShop] = new PointF(7, 3.5F);

            Room parthRecreationField = AddRoom("Recreation Field", "Parth Recreation Field");
            AddBidirectionalExits(sweetwaterFuchsia, parthRecreationField, BidirectionalExitType.WestEast);
            AddBidirectionalExits(parthRecreationField, dragonpaw, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[parthRecreationField] = new PointF(9, 4);

            Room frostIndigo = AddRoom("Frost/Indigo", "Frost Lane/Indigo Avenue Intersection");
            AddBidirectionalExits(frostIndigo, sweetwaterIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostIndigo] = new PointF(1, 3);

            Room frostMagenta = AddRoom("Frost/Magenta", "Frost Lane/Magenta Avenue Intersection");
            AddBidirectionalExits(frostIndigo, frostMagenta, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostMagenta, sweetwaterMagenta, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostMagenta] = new PointF(2, 3);

            Room frostWestLake = AddRoom("Frost/WestLake", "Frost Lane/West Lake Circle");
            AddBidirectionalExits(frostMagenta, frostWestLake, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[frostWestLake] = new PointF(3, 3);

            Room frostAquamarine = AddRoom("Frost/Aquamarine", "Frost Lane/Aquamarine Way Intersection");
            AddBidirectionalExits(frostWestLake, frostAquamarine, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostAquamarine, sweetwaterAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostAquamarine] = new PointF(4, 3);

            Room frostFuchsia = AddRoom("Frost/Fuchsia", "Frost Lane/Fuchsia Way Intersection");
            AddBidirectionalExits(frostAquamarine, frostFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostFuchsia, sweetwaterFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostFuchsia] = new PointF(8, 3);

            Room frostEastLake = AddRoom("Frost/EastLake", "Frost Lane/East Lake Circle");
            AddBidirectionalExits(frostFuchsia, frostEastLake, BidirectionalExitType.WestEast);
            AddBidirectionalExits(frostEastLake, parthRecreationField, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[frostEastLake] = new PointF(9, 3);

            Room northEntranceInside = AddRoom("North Gate Inside", "North Entrance to Esgaroth");
            AddBidirectionalExits(northEntranceInside, frostIndigo, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[northEntranceInside] = new PointF(1, 2);
            northOfEsgarothGraph.Rooms[northEntranceInside] = new PointF(5, 11);

            northGateOutside = AddRoom("North Gate Outside", "North Entrance to Esgaroth");
            AddBidirectionalSameNameExit(northEntranceInside, northGateOutside, "gate");
            esgarothGraph.Rooms[northGateOutside] = new PointF(1, 1.5F);
            AddMapBoundaryPoint(northEntranceInside, northGateOutside, MapType.Esgaroth, MapType.NorthOfEsgaroth);

            Room stablesExerciseYard = AddRoom("Stables/Exercise", "Esgaroth Stables and Exercise Yard");
            AddBidirectionalExits(northEntranceInside, stablesExerciseYard, BidirectionalExitType.WestEast);
            AddExit(stablesExerciseYard, frostMagenta, "south");
            esgarothGraph.Rooms[stablesExerciseYard] = new PointF(2, 2);

            Room lakeCircle = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(stablesExerciseYard, lakeCircle, BidirectionalExitType.WestEast);
            AddBidirectionalExits(lakeCircle, frostWestLake, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[lakeCircle] = new PointF(3, 2);

            Room dignitaryStables = AddRoom("Dignitary Stables", "Dignitary Stables");
            AddBidirectionalExits(lakeCircle, dignitaryStables, BidirectionalExitType.WestEast);
            AddBidirectionalExits(dignitaryStables, frostAquamarine, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[dignitaryStables] = new PointF(4, 2);

            Room dairyProduction = AddRoom("Dairy Facility", "Dairy Production and Storage Facility");
            AddBidirectionalExits(dignitaryStables, dairyProduction, BidirectionalExitType.WestEast);
            AddBidirectionalExits(dairyProduction, frostFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[dairyProduction] = new PointF(8, 2);

            Room lakeCircle2 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(dairyProduction, lakeCircle2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(lakeCircle2, frostEastLake, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[lakeCircle2] = new PointF(9, 2);

            Room lakeParthShore = AddHealingRoom("Shore", "Lake Parth Shore", HealingRoom.Esgaroth);
            AddBidirectionalExits(lakeParthShore, lakeCircle2, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(lakeParthShore, dairyProduction, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(lakeParthShore, lakeCircle, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeParthShore] = new PointF(8, 1);

            Room lakeViewDrive1 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive1, lakeParthShore, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeViewDrive1] = new PointF(9, 0);

            Room lakeViewDrive2 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive2, lakeViewDrive1, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeViewDrive2] = new PointF(10, -1);

            Room lakeViewDrive3 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive3, lakeViewDrive2, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[lakeViewDrive3] = new PointF(10, -2);

            Room lakeViewDrive4 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive4, lakeViewDrive3, BidirectionalExitType.SoutheastNorthwest);
            esgarothGraph.Rooms[lakeViewDrive4] = new PointF(7, -3);

            Room lakeViewDrive5 = AddRoom("Lakeview", "Lakeview Drive");
            AddBidirectionalExits(lakeViewDrive5, lakeViewDrive4, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[lakeViewDrive5] = new PointF(6, -3);

            Room lakeCircle3 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeViewDrive5, lakeCircle3, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeCircle3] = new PointF(5, -2);

            Room lakeCircle4 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeCircle3, lakeCircle4, BidirectionalExitType.SouthwestNortheast);
            esgarothGraph.Rooms[lakeCircle4] = new PointF(1.5F, 0.5F);

            Room lakeCircle5 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeCircle4, lakeCircle5, BidirectionalExitType.SoutheastNorthwest);
            esgarothGraph.Rooms[lakeCircle5] = new PointF(2, 1);

            Room lakeCircle6 = AddRoom("Lake Circle", "Lake Circle Road");
            AddBidirectionalExits(lakeCircle5, lakeCircle6, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(lakeCircle6, lakeCircle, BidirectionalExitType.SoutheastNorthwest);
            esgarothGraph.Rooms[lakeCircle6] = new PointF(2.5F, 1.5F);

            Room prospectorInn = AddRoom("Prospector Inn", "Prospector Inn");
            AddBidirectionalSameNameExit(frostWestLake, prospectorInn, "door");
            esgarothGraph.Rooms[prospectorInn] = new PointF(3, 3.25F);

            Room redDragonTavern = AddRoom("Red Dragon Tavern", "The Red Dragon Tavern");
            AddBidirectionalSameNameExit(prospectorInn, redDragonTavern, "red"); //says red door but door doesn't work
            AddBidirectionalSameNameExit(redDragonTavern, sweetwaterAquamarine, "door");
            esgarothGraph.Rooms[redDragonTavern] = new PointF(3, 3.5F);

            Room jaysSmithShoppe = AddRoom("Smithy", "Jay's Smith Shoppe");
            AddPermanentMobs(jaysSmithShoppe, MobTypeEnum.Smithy);
            AddBidirectionalSameNameExit(frostMagenta, jaysSmithShoppe, "door");
            esgarothGraph.Rooms[jaysSmithShoppe] = new PointF(2.33F, 2.75F);

            Room foundry = AddRoom("Foundry", "Foundry");
            AddPermanentMobs(foundry, MobTypeEnum.SivalTheArtificer);
            AddBidirectionalExitsWithOut(jaysSmithShoppe, foundry, "foundry");
            esgarothGraph.Rooms[foundry] = new PointF(2.33F, 2.5F);

            Room archeryRange = AddRoom("Archery Range", "Archery Range");
            AddBidirectionalExits(plymouthMagenta, archeryRange, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[archeryRange] = new PointF(2, 7);

            Room gardenCircle = AddRoom("Garden Circle", "Garden Circle Road");
            AddBidirectionalExits(archeryRange, gardenCircle, BidirectionalExitType.WestEast);
            AddBidirectionalExits(plymouthGardenCir, gardenCircle, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[gardenCircle] = new PointF(3, 7);

            Room gardenCircle2 = AddRoom("Garden Circle", "Garden Circle Road");
            AddBidirectionalExits(gardenCircle, gardenCircle2, BidirectionalExitType.WestEast);
            esgarothGraph.Rooms[gardenCircle2] = new PointF(6, 7);

            Room gardenCirFuchsia = AddRoom("Garden/Fuchsia", "Garden Circle Road/Fuchsia Way");
            AddBidirectionalExits(gardenCircle2, gardenCirFuchsia, BidirectionalExitType.WestEast);
            AddBidirectionalExits(plymouthFuchsia, gardenCirFuchsia, BidirectionalExitType.NorthSouth);
            esgarothGraph.Rooms[gardenCirFuchsia] = new PointF(8, 7);

            Room southGate = AddRoom("South Gate", "South Gate of Esgaroth");
            AddBidirectionalSameNameExit(gardenCirFuchsia, southGate, "gate");
            esgarothGraph.Rooms[southGate] = new PointF(8, 8);

            Room archeryEmporium = AddRoom("Archery Emporium", "Elena's Archery Emporium");
            AddExit(archeryRange, archeryEmporium, "door");
            AddExit(archeryEmporium, archeryRange, "range");
            AddBidirectionalSameNameExit(archeryEmporium, gardenCircle, "door");
            esgarothGraph.Rooms[archeryEmporium] = new PointF(2, 8);

            Room libraryEntrance = AddRoom("Front Entry", "Front Entry of the Great Library");
            AddExit(briarMagenta, libraryEntrance, "library");
            AddExit(libraryEntrance, briarMagenta, "oak door");
            esgarothGraph.Rooms[libraryEntrance] = new PointF(0, 4);

            Room referenceDesk = AddRoom("Ranier", "Reference Desk");
            AddPermanentMobs(referenceDesk, MobTypeEnum.RanierTheLibrarian);
            AddExit(libraryEntrance, referenceDesk, "blue door");
            AddExit(referenceDesk, libraryEntrance, "door");
            esgarothGraph.Rooms[referenceDesk] = new PointF(0, 5);

            Room colloquiaRoom = AddRoom("Blind Crone", "Colloquia Room");
            AddPermanentMobs(colloquiaRoom, MobTypeEnum.BlindCrone);
            e = AddExit(libraryEntrance, colloquiaRoom, "golden door");
            e.MustOpen = true;
            AddExit(colloquiaRoom, libraryEntrance, "door");
            esgarothGraph.Rooms[colloquiaRoom] = new PointF(-1, 5);

            Room researchRoom = AddRoom("Research Room", "The Research Room");
            AddExit(libraryEntrance, researchRoom, "green door");
            AddExit(researchRoom, libraryEntrance, "door");
            esgarothGraph.Rooms[researchRoom] = new PointF(-1, 4);

            Room languageStudies = AddRoom("Language Studies", "Language studies room");
            AddExit(researchRoom, languageStudies, "small door");
            AddExit(languageStudies, researchRoom, "door");
            esgarothGraph.Rooms[languageStudies] = new PointF(-1, 3.5F);

            Room lounge = AddRoom("Lodz", "Lounge");
            AddPermanentMobs(lounge, MobTypeEnum.Lodz);
            e = AddBidirectionalExitsWithOut(languageStudies, lounge, "oak door");
            e.Hidden = true;
            esgarothGraph.Rooms[lounge] = new PointF(-1, 3);

            Room specialCollectionStacks = AddRoom("Galdor", "Special Collection Stacks");
            AddPermanentMobs(specialCollectionStacks, MobTypeEnum.Galdor);
            e = AddBidirectionalExitsWithOut(lounge, specialCollectionStacks, "sign");
            e.KeyType = SupportedKeysFlags.LibraryKey;
            e.Hidden = true;
            esgarothGraph.Rooms[specialCollectionStacks] = new PointF(-1, 2.5F);
        }

        private void AddEsgarothMuseum(Room briarLane2, RoomGraph esgarothGraph)
        {
            RoomGraph esgarothMuseumGraph = _graphs[MapType.EsgarothMuseum];

            esgarothMuseumGraph.Rooms[briarLane2] = new PointF(0, 0);

            Room giftShoppe = AddRoom("Gift Shoppe", "Museum Gift Shoppe");
            AddExit(briarLane2, giftShoppe, "museum");
            AddExit(giftShoppe, briarLane2, "door");
            esgarothGraph.Rooms[giftShoppe] = new PointF(6, 5.5F);
            esgarothMuseumGraph.Rooms[giftShoppe] = new PointF(0, 1);
            AddMapBoundaryPoint(briarLane2, giftShoppe, MapType.Esgaroth, MapType.EsgarothMuseum);

            Room foyer = AddRoom("Foyer", "Adrilite Museum Entrance Foyer");
            AddBidirectionalExits(giftShoppe, foyer, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[foyer] = new PointF(1, 1);

            Room childsExhibition = AddRoom("Child's Exhibition", "Adrilite Child's Exhibition");
            AddBidirectionalExits(childsExhibition, foyer, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[childsExhibition] = new PointF(1, 0);

            Room oForestExhibit = AddRoom("Forest", "Adrilite Museum Forest Exhibit");
            AddBidirectionalExits(foyer, oForestExhibit, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[oForestExhibit] = new PointF(2, 1);

            Room oDarkForestExhibit = AddRoom("Dark Forest", "Dark Forest Exhibit");
            AddBidirectionalExits(oForestExhibit, oDarkForestExhibit, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[oDarkForestExhibit] = new PointF(3, 1);

            Room hallOfDoomNorth = AddRoom("Hall of Doom", "Adrilite Museum Hall of Doom");
            AddBidirectionalExits(foyer, hallOfDoomNorth, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[hallOfDoomNorth] = new PointF(1, 2);

            Room hallOfDoomSouth = AddRoom("Hall of Doom", "Adrilite Museum Hall of Doom");
            AddBidirectionalExits(hallOfDoomNorth, hallOfDoomSouth, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[hallOfDoomSouth] = new PointF(1, 3);

            Room dragonsAndBrimstone = AddRoom("Dragons/Brimstone", "Dragons and Brimstone Exhibit");
            AddExit(hallOfDoomNorth, dragonsAndBrimstone, "west");
            AddExit(dragonsAndBrimstone, hallOfDoomNorth, "northeast");
            AddExit(hallOfDoomSouth, dragonsAndBrimstone, "west");
            AddExit(dragonsAndBrimstone, hallOfDoomSouth, "southeast");
            esgarothMuseumGraph.Rooms[dragonsAndBrimstone] = new PointF(0, 2.5F);

            Room theatreOfDoom = AddRoom("Theatre of Doom", "Adrilite Museum Theatre of Doom");
            AddBidirectionalExits(hallOfDoomNorth, theatreOfDoom, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[theatreOfDoom] = new PointF(2, 2);

            Room cavernsOfDoom = AddRoom("Caverns of Doom", "Adrilite Museum Caverns of Doom Exhibit");
            AddBidirectionalExits(hallOfDoomSouth, cavernsOfDoom, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[cavernsOfDoom] = new PointF(2, 3);

            Room kTralDesertExhibit = AddRoom("K'Tral Desert", "Adrilite K'Tral Desert Exhibit");
            AddBidirectionalExits(hallOfDoomSouth, kTralDesertExhibit, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[kTralDesertExhibit] = new PointF(1, 4);

            Room hallOfPrehistory = AddRoom("Hall of Prehistory", "Adrilite Museum Hall of Prehistory");
            AddBidirectionalSameNameExit(foyer, hallOfPrehistory, "stairs");
            esgarothMuseumGraph.Rooms[hallOfPrehistory] = new PointF(4, 2);

            Room hallOfPrehistory2 = AddRoom("Hall of Prehistory", "Adrilite Museum Hall of Prehistory");
            AddBidirectionalExits(hallOfPrehistory, hallOfPrehistory2, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[hallOfPrehistory2] = new PointF(4, 3);

            Room carnivoreExhibit = AddRoom("Carnivores", "Adrilite Museum Prehistoric Carnivore Exhibit");
            AddBidirectionalExits(carnivoreExhibit, hallOfPrehistory2, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[carnivoreExhibit] = new PointF(3, 3);

            Room herbivoresExhibit = AddRoom("Herbivores", "Adrilite Museum Prehistoric Herbivore Exhibit");
            AddBidirectionalExits(hallOfPrehistory2, herbivoresExhibit, BidirectionalExitType.WestEast);
            esgarothMuseumGraph.Rooms[herbivoresExhibit] = new PointF(5, 3);

            Room mammalsExhibit = AddRoom("Mammals", "Adrilite Museum Prehistoric Mammal Exhibit");
            AddBidirectionalExits(hallOfPrehistory2, mammalsExhibit, BidirectionalExitType.NorthSouth);
            esgarothMuseumGraph.Rooms[mammalsExhibit] = new PointF(4, 4);
        }

        private void AddSpindrilsCastle(Room spindrilsCastleOutside)
        {
            RoomGraph spindrilsCastleLevel1Graph = _graphs[MapType.SpindrilsCastleLevel1];

            spindrilsCastleLevel1Graph.Rooms[spindrilsCastleOutside] = new PointF(11, 10);

            Room spindrilsCastleInside = AddRoom("Dark/Heavy Clouds", "Dark and Heavy Clouds");
            Exit e = AddExit(spindrilsCastleOutside, spindrilsCastleInside, "up");
            e.FloatRequirement = FloatRequirement.Fly;
            e = AddExit(spindrilsCastleInside, spindrilsCastleOutside, "down");
            e.FloatRequirement = FloatRequirement.Fly;
            spindrilsCastleLevel1Graph.Rooms[spindrilsCastleInside] = new PointF(10, 9);

            Room oCloudEdge = AddHealingRoom("Cloud Edge", "Cloud Edge", HealingRoom.SpindrilsCastle);
            AddBidirectionalExits(spindrilsCastleInside, oCloudEdge, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCloudEdge] = new PointF(10, 10);

            Room oBrokenCastleWall = AddRoom("Broken Castle Wall", "Broken Castle Wall");
            AddBidirectionalExits(oBrokenCastleWall, spindrilsCastleInside, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oBrokenCastleWall] = new PointF(10, 8);
            //CSRTODO: rubble

            Room oEastCastleWall = AddRoom("East Castle Wall", "East Castle Wall");
            AddBidirectionalExits(oEastCastleWall, oBrokenCastleWall, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oEastCastleWall] = new PointF(10, 7);

            Room oEastCastleWall2 = AddRoom("East Castle Wall", "East Castle Wall");
            AddBidirectionalExits(oEastCastleWall2, oEastCastleWall, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oEastCastleWall2] = new PointF(10, 6);

            Room oSewageVault = AddRoom("Sewage Vault", "Sewage Vault");
            e = AddExit(oEastCastleWall2, oSewageVault, "grate");
            e.Hidden = true;
            AddExit(oSewageVault, oEastCastleWall2, "grate");
            spindrilsCastleLevel1Graph.Rooms[oSewageVault] = new PointF(10, 5);

            Room oSewageShaft1 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddExit(oSewageVault, oSewageShaft1, "shaft");
            AddExit(oSewageShaft1, oSewageVault, "east");
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft1] = new PointF(9, 5);

            Room oSewageShaft2 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddBidirectionalExits(oSewageShaft2, oSewageShaft1, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft2] = new PointF(8, 5);

            Room oSewageShaft3 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddBidirectionalExits(oSewageShaft3, oSewageShaft2, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft3] = new PointF(7, 5);

            Room oSewageShaft4 = AddRoom("Sewage Shaft", "Sewage Shaft");
            AddBidirectionalExits(oSewageShaft4, oSewageShaft3, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oSewageShaft4] = new PointF(6, 5);

            Room oKitchenCorridor = AddRoom("Kitchen Corridor", "Kitchen Corridor");
            AddBidirectionalSameNameExit(oSewageShaft4, oKitchenCorridor, "grate");
            spindrilsCastleLevel1Graph.Rooms[oKitchenCorridor] = new PointF(5, 5);

            Room oServiceCorridor = AddRoom("Service Corridor", "Service Corridor");
            AddBidirectionalExits(oKitchenCorridor, oServiceCorridor, BidirectionalExitType.SoutheastNorthwest);
            spindrilsCastleLevel1Graph.Rooms[oServiceCorridor] = new PointF(5.5F, 5.5F);

            Room oArieCorridor = AddRoom("Arie Corridor", "Arie Corridor");
            AddBidirectionalExits(oServiceCorridor, oArieCorridor, BidirectionalExitType.SoutheastNorthwest);
            spindrilsCastleLevel1Graph.Rooms[oArieCorridor] = new PointF(6, 6);
            //CSRTODO: ladder

            Room oSpindrilsAerie = AddRoom("Spindril's Aerie", "Spindril's Aerie");
            AddPermanentMobs(oSpindrilsAerie, MobTypeEnum.Roc);
            AddExit(oArieCorridor, oSpindrilsAerie, "aerie");
            AddExit(oSpindrilsAerie, oArieCorridor, "entry");
            spindrilsCastleLevel1Graph.Rooms[oSpindrilsAerie] = new PointF(6.5F, 5.5F);

            Room oTuraksAlcove = AddRoom("Turak's Alcove", "Turak's Alcove");
            AddBidirectionalExits(oTuraksAlcove, oKitchenCorridor, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oTuraksAlcove] = new PointF(5, 4.5F);
            //CSRTODO: up

            Room oKitchen = AddRoom("Kitchen", "Kitchen");
            AddBidirectionalExits(oKitchen, oKitchenCorridor, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oKitchen] = new PointF(4, 5);

            Room oCastleSpindrilCourtyardNE = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExitsWithOut(oCastleSpindrilCourtyardNE, oArieCorridor, "corridor");
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardNE] = new PointF(6, 7);

            Room oCastleSpindrilCourtyardE = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardNE, oCastleSpindrilCourtyardE, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardE] = new PointF(6, 8);

            Room oCastleSpindrilCourtyardSE = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardE, oCastleSpindrilCourtyardSE, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardSE] = new PointF(6, 9);

            Room oCastleSpindrilCourtyardN = AddRoom("Castle Courtyard", "North Court of Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardN, oCastleSpindrilCourtyardNE, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardN] = new PointF(5, 7);

            Room oCastleSpindrilCourtyardMiddle = AddRoom("Castle Courtyard", "Center Court for Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardN, oCastleSpindrilCourtyardMiddle, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCastleSpindrilCourtyardMiddle, oCastleSpindrilCourtyardE, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardMiddle] = new PointF(5, 8);

            Room oCastleSpindrilCourtyardS = AddRoom("Castle Courtyard", "South Court of Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardMiddle, oCastleSpindrilCourtyardS, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCastleSpindrilCourtyardS, oCastleSpindrilCourtyardSE, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardS] = new PointF(5, 9);

            Room oCastleSpindrilCourtyardSW = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardSW, oCastleSpindrilCourtyardS, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardSW] = new PointF(4, 9);

            Room oWestsideTunnelEntry = AddRoom("Westside Tunnel Entry", "Westside Tunnel Entry");
            AddBidirectionalExitsWithOut(oCastleSpindrilCourtyardSW, oWestsideTunnelEntry, "tunnel");
            spindrilsCastleLevel1Graph.Rooms[oWestsideTunnelEntry] = new PointF(3, 9);

            Room oWestsideHallway1 = AddRoom("Westside Hallway", "Westside Hallway");
            AddBidirectionalExits(oWestsideHallway1, oWestsideTunnelEntry, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oWestsideHallway1] = new PointF(3, 8);

            Room oWesternHallway2 = AddRoom("Western Hallway", "Western Hallway");
            AddBidirectionalExits(oWesternHallway2, oWestsideHallway1, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oWesternHallway2] = new PointF(3, 7);

            Room oBaseOfBroadStairs = AddRoom("Stairs Base", "Base of the Broad Stairs");
            AddBidirectionalExits(oBaseOfBroadStairs, oWesternHallway2, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oBaseOfBroadStairs] = new PointF(3, 6);

            Room oBroadStairs = AddRoom("Broad Stairs", "Broad Stairs");
            AddBidirectionalExits(oBroadStairs, oBaseOfBroadStairs, BidirectionalExitType.UpDown);
            spindrilsCastleLevel1Graph.Rooms[oBroadStairs] = new PointF(2, 5);
            //CSRTODO: up

            Room oRedVelvetRoom = AddRoom("Tellia", "Red Velvet Room");
            AddPermanentMobs(oRedVelvetRoom, MobTypeEnum.TelliaTheWitch);
            AddBidirectionalExitsWithOut(oBroadStairs, oRedVelvetRoom, "oak");
            spindrilsCastleLevel1Graph.Rooms[oRedVelvetRoom] = new PointF(1, 6);

            Room oBlueVelvetRoom = AddRoom("Blue Velvet", "Blue Velvet Room");
            AddBidirectionalExitsWithOut(oBroadStairs, oBlueVelvetRoom, "ash");
            spindrilsCastleLevel1Graph.Rooms[oBlueVelvetRoom] = new PointF(1, 5);

            Room oGreenVelvetRoom = AddRoom("Lord De'Arnse", "Green Velvet Room");
            AddPermanentMobs(oGreenVelvetRoom, MobTypeEnum.LordDeArnse);
            AddBidirectionalExitsWithOut(oBroadStairs, oGreenVelvetRoom, "hickory");
            spindrilsCastleLevel1Graph.Rooms[oGreenVelvetRoom] = new PointF(1, 4);

            Room oLowerWesternCorridor = AddRoom("Corridor", "Lower Western Corridor");
            AddBidirectionalSameNameExit(oBaseOfBroadStairs, oLowerWesternCorridor, "door");
            spindrilsCastleLevel1Graph.Rooms[oLowerWesternCorridor] = new PointF(3, 5);

            Room oLowerWesternCorridor2 = AddRoom("Corridor", "Lower Western Corridor");
            AddBidirectionalExits(oLowerWesternCorridor2, oLowerWesternCorridor, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oLowerWesternCorridor2] = new PointF(3, 4);

            Room oLowerWesternCorridor3 = AddRoom("Corridor", "Lower Western Corridor");
            AddBidirectionalExits(oLowerWesternCorridor3, oLowerWesternCorridor2, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oLowerWesternCorridor3] = new PointF(3, 3);

            Room oWesternCorridorStairs = AddRoom("Stairs", "Western Corridor Stairs");
            AddBidirectionalExits(oWesternCorridorStairs, oLowerWesternCorridor3, BidirectionalExitType.UpDown);
            spindrilsCastleLevel1Graph.Rooms[oWesternCorridorStairs] = new PointF(3, 2);

            Room oWesternCorridor = AddRoom("Western Corridor", "Western Corridor");
            AddExit(oWesternCorridorStairs, oWesternCorridor, "up");
            AddExit(oWesternCorridor, oWesternCorridorStairs, "stairs");
            spindrilsCastleLevel1Graph.Rooms[oWesternCorridor] = new PointF(3, 1);
            //CSRTODO: north, door

            Room oCastleSpindrilCourtyardW = AddRoom("Castle Courtyard", "Western Courtyard for Castle Spindril");
            AddBidirectionalExits(oCastleSpindrilCourtyardW, oCastleSpindrilCourtyardSW, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCastleSpindrilCourtyardW, oCastleSpindrilCourtyardMiddle, BidirectionalExitType.WestEast);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardW] = new PointF(4, 8);

            Room oCastleSpindrilCourtyardNW = AddRoom("Castle Courtyard", "Castle Spindril Courtyard");
            AddBidirectionalExits(oCastleSpindrilCourtyardNW, oCastleSpindrilCourtyardN, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCastleSpindrilCourtyardNW, oCastleSpindrilCourtyardW, BidirectionalExitType.NorthSouth);
            spindrilsCastleLevel1Graph.Rooms[oCastleSpindrilCourtyardNW] = new PointF(4, 7);
            //CSRTODO: steps

            Room oWeaponsmithShop = AddRoom("Weaponsmith's Shop", "Weaponsmith's Shop");
            AddPermanentMobs(oWeaponsmithShop, MobTypeEnum.Gnimbelle);
            AddBidirectionalExitsWithOut(oCastleSpindrilCourtyardS, oWeaponsmithShop, "door");
            spindrilsCastleLevel1Graph.Rooms[oWeaponsmithShop] = new PointF(5, 9.5F);

            Room oGnimbelleGninbalArmory = AddRoom("Gni Armory", "Gnimbelle and Gninbal's Armory");
            AddPermanentMobs(oGnimbelleGninbalArmory, MobTypeEnum.Gnibal);
            AddBidirectionalSameNameExit(oWeaponsmithShop, oGnimbelleGninbalArmory, "door");
            spindrilsCastleLevel1Graph.Rooms[oGnimbelleGninbalArmory] = new PointF(5, 10);

            Room oGniPawnShop = AddRoom("Gni Pawn Shop", "Gnimbelle and Gnarbolla's Pawn Shoppe");
            AddPermanentMobs(oGniPawnShop, MobTypeEnum.Gnarbolla);
            e = AddBidirectionalExitsWithOut(oGnimbelleGninbalArmory, oGniPawnShop, "passage");
            e.Hidden = true;
            spindrilsCastleLevel1Graph.Rooms[oGniPawnShop] = new PointF(5, 10.5F);

            Room oSouthernStairwellAlcove = AddRoom("South Stairwell Alcove", "Southern Tower's Stairwell Alcove");
            AddExit(oCastleSpindrilCourtyardSE, oSouthernStairwellAlcove, "alcove");
            AddExit(oSouthernStairwellAlcove, oCastleSpindrilCourtyardSE, "north");
            spindrilsCastleLevel1Graph.Rooms[oSouthernStairwellAlcove] = new PointF(7, 9);
            //CSRTODO: up

            Room oBarracksHallway = AddRoom("Barracks Hallway", "Barracks Hallway");
            AddBidirectionalExitsWithOut(oSouthernStairwellAlcove, oBarracksHallway, "door");
            spindrilsCastleLevel1Graph.Rooms[oBarracksHallway] = new PointF(8, 9);

            Room oCastleBarracks = AddRoom("Castle Barracks", "Castle Barracks");
            AddBidirectionalExitsWithOut(oBarracksHallway, oCastleBarracks, "barracks");
            spindrilsCastleLevel1Graph.Rooms[oCastleBarracks] = new PointF(8, 8);

            Room oCastleArmory = AddRoom("Castle Armory", "Castle Spindril Armory");
            AddBidirectionalExitsWithOut(oBarracksHallway, oCastleArmory, "armory");
            spindrilsCastleLevel1Graph.Rooms[oCastleArmory] = new PointF(8, 10);
        }

        private void AddShantyTown(Room oMistyTrail8)
        {
            RoomGraph imladrisToTharbadGraph = _graphs[MapType.ImladrisToTharbad];
            RoomGraph oShantyTownGraph = _graphs[MapType.ShantyTown];

            oShantyTownGraph.Rooms[oMistyTrail8] = new PointF(5, 0);

            Room oRuttedDirtRoad = AddRoom("Dirt Road", "Rutted Dirt Road");
            AddBidirectionalExits(oRuttedDirtRoad, oMistyTrail8, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oRuttedDirtRoad] = new PointF(4, 0);
            imladrisToTharbadGraph.Rooms[oRuttedDirtRoad] = new PointF(3, 7);
            AddMapBoundaryPoint(oMistyTrail8, oRuttedDirtRoad, MapType.ImladrisToTharbad, MapType.ShantyTown);

            Room oHouseOfPleasure = AddRoom("mistress", "House of Pleasure");
            AddPermanentMobs(oHouseOfPleasure, MobTypeEnum.Mistress);
            AddBidirectionalSameNameExit(oRuttedDirtRoad, oHouseOfPleasure, "door");
            oShantyTownGraph.Rooms[oHouseOfPleasure] = new PointF(4, -1);

            Room oMadameDespana = AddRoom("Madame Despana", "Private Bedchamber");
            AddPermanentMobs(oMadameDespana, MobTypeEnum.MadameDespana);
            AddBidirectionalSameNameMustOpenExit(oHouseOfPleasure, oMadameDespana, "crimson door");
            oShantyTownGraph.Rooms[oMadameDespana] = new PointF(3, -1);

            Room oNorthEdgeOfShantyTown = AddRoom("Shanty Town", "North Edge of Shanty Town");
            AddBidirectionalExits(oRuttedDirtRoad, oNorthEdgeOfShantyTown, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oNorthEdgeOfShantyTown] = new PointF(4, 1);

            Room oRedFoxLane = AddRoom("Red Fox", "Red Fox Lane");
            AddBidirectionalExits(oRedFoxLane, oNorthEdgeOfShantyTown, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oRedFoxLane] = new PointF(3, 1);

            Room oGypsyCamp = AddRoom("Gypsy Camp", "Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp, oRedFoxLane, BidirectionalExitType.SoutheastNorthwest);
            oShantyTownGraph.Rooms[oGypsyCamp] = new PointF(2, 0);

            Room oGypsyCamp2 = AddRoom("Gypsy Camp", "Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp2, oGypsyCamp, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oGypsyCamp2] = new PointF(1, 0);

            Room oMadameProkawskiPalmReadingService = AddRoom("Palm Reading Service", "Madame Prokawski's Palm Reading Service");
            AddBidirectionalExitsWithOut(oGypsyCamp2, oMadameProkawskiPalmReadingService, "wagon");
            oShantyTownGraph.Rooms[oMadameProkawskiPalmReadingService] = new PointF(1, -1);

            Room oGypsyAnimalKeep = AddRoom("Gypsy Animal Keep", "Gypsy Animal Keep");
            AddBidirectionalSameNameExit(oGypsyCamp2, oGypsyAnimalKeep, "gate");
            oShantyTownGraph.Rooms[oGypsyAnimalKeep] = new PointF(0, 0);

            Room oExoticAnimalKeep = AddRoom("Exotic Animal Wagon", "Exotic Animal Wagon");
            AddBidirectionalExitsWithOut(oGypsyAnimalKeep, oExoticAnimalKeep, "wagon");
            oShantyTownGraph.Rooms[oExoticAnimalKeep] = new PointF(-1, 0);

            Room oNorthShantyTown = AddRoom("Shanty Town", "North Shanty Town");
            AddBidirectionalExits(oRedFoxLane, oNorthShantyTown, BidirectionalExitType.SouthwestNortheast);
            oShantyTownGraph.Rooms[oNorthShantyTown] = new PointF(2, 2);

            Room oShantyTownDump = AddRoom("Town Dump", "Shanty Town Dump");
            AddBidirectionalExits(oNorthShantyTown, oShantyTownDump, BidirectionalExitType.SouthwestNortheast);
            oShantyTownGraph.Rooms[oShantyTownDump] = new PointF(1, 3);

            Room oGarbagePit = AddRoom("Garbage Pit", "Garbage Pit");
            AddBidirectionalExitsWithOut(oShantyTownDump, oGarbagePit, "garbage");
            oShantyTownGraph.Rooms[oGarbagePit] = new PointF(0, 3);

            Room oShantyTownWest = AddRoom("Shanty Town", "Shanty Town West");
            AddBidirectionalExits(oShantyTownDump, oShantyTownWest, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTownWest] = new PointF(1, 4);

            Room oCopseOfTrees = AddRoom("Copse of Trees", "Copse of Trees");
            AddBidirectionalExits(oShantyTownWest, oCopseOfTrees, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oCopseOfTrees] = new PointF(1, 5);

            Room oBluff = AddRoom("Bluff", "Shanty Town Bluff");
            AddBidirectionalExits(oCopseOfTrees, oBluff, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oBluff] = new PointF(1, 6);

            Room oShantyTown1 = AddRoom("Shanty Town", "Shanty Town");
            AddBidirectionalExits(oNorthEdgeOfShantyTown, oShantyTown1, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTown1] = new PointF(4, 2);

            Room oShantyTown2 = AddRoom("Shanty Town", "Shanty Town");
            AddBidirectionalExits(oShantyTown1, oShantyTown2, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTown2] = new PointF(4, 3);

            Room oShantyTown3 = AddRoom("Shanty Town", "Shanty Town");
            AddBidirectionalExits(oShantyTown2, oShantyTown3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBluff, oShantyTown3, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oShantyTown3] = new PointF(3, 6);

            Room oPentampurisPalace = AddRoom("Pentampuri's Palace", "Pentampuri's Palace");
            AddBidirectionalExitsWithOut(oShantyTown3, oPentampurisPalace, "shack");
            oShantyTownGraph.Rooms[oPentampurisPalace] = new PointF(4, 6);

            Room oPrinceBrunden = AddRoom("Prince Brunden", "King of the Gypsies Wagon");
            AddPermanentMobs(oPrinceBrunden, MobTypeEnum.PrinceBrunden);
            AddBidirectionalExitsWithOut(oGypsyCamp, oPrinceBrunden, "wagon");
            oShantyTownGraph.Rooms[oPrinceBrunden] = new PointF(2, -1);

            Room oNaugrim = AddRoom("Naugrim", "Naugrim's Wine Cask Home");
            AddPermanentMobs(oNaugrim, MobTypeEnum.Naugrim);
            AddBidirectionalExitsWithOut(oNorthShantyTown, oNaugrim, "cask");
            oShantyTownGraph.Rooms[oNaugrim] = new PointF(1, 1);

            Room oNaugrimsWineCellar = AddRoom("Wine Cellar", "Naugrim's Wine Cellar");
            AddBidirectionalExitsWithOut(oNaugrim, oNaugrimsWineCellar, "cellar");
            oShantyTownGraph.Rooms[oNaugrimsWineCellar] = new PointF(0, 1);

            Room oHogoth = AddRoom("Hogoth", "Hogoth's Home");
            AddPermanentMobs(oHogoth, MobTypeEnum.Hogoth);
            AddBidirectionalExitsWithOut(oShantyTownWest, oHogoth, "shack");
            oShantyTownGraph.Rooms[oHogoth] = new PointF(0, 4);

            Room oFaornil = AddRoom("Faornil", "Seer's Tent");
            AddPermanentMobs(oFaornil, MobTypeEnum.FaornilTheSeer);
            AddBidirectionalExitsWithOut(oShantyTown1, oFaornil, "tent");
            oShantyTownGraph.Rooms[oFaornil] = new PointF(5, 2);

            Room oGraddy = AddRoom("Graddy", "Graddy the Dwarf's Wagon");
            AddPermanentMobs(oGraddy, MobTypeEnum.Graddy);
            AddBidirectionalExitsWithOut(oShantyTown2, oGraddy, "wagon");
            oShantyTownGraph.Rooms[oGraddy] = new PointF(5, 3);

            Room oGraddyOgre = AddRoom("Ogre", "Graddy's Ogre Pen");
            AddPermanentMobs(oGraddyOgre, MobTypeEnum.Ogre);
            AddBidirectionalSameNameMustOpenExit(oGraddy, oGraddyOgre, "gate");
            oShantyTownGraph.Rooms[oGraddyOgre] = new PointF(5, 4);
        }

        private void AddIntangible(Room oBreeTownSquare, Room healingHand, Room nindamosVillageCenter, Room accursedGuildHall, Room crusaderGuildHall, Room thiefGuildHall)
        {
            RoomGraph intangibleGraph = _graphs[MapType.Intangible];
            RoomGraph nindamosGraph = _graphs[MapType.Nindamos];
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];
            RoomGraph breeStreetsGraph = _graphs[MapType.BreeStreets];

            intangibleGraph.Rooms[oBreeTownSquare] = new PointF(0, 0);
            intangibleGraph.Rooms[healingHand] = new PointF(1, 0);
            intangibleGraph.Rooms[nindamosVillageCenter] = new PointF(2, 0);

            Room treeOfLife = AddRoom("Tree of Life", "The Tree of Life");
            AddPermanentItems(treeOfLife, ItemTypeEnum.BookOfKnowledge);
            treeOfLife.Intangible = true;
            AddExit(treeOfLife, oBreeTownSquare, "down");
            intangibleGraph.Rooms[treeOfLife] = new PointF(0, 1);
            breeStreetsGraph.Rooms[treeOfLife] = new PointF(5.5F, 3.3F);
            AddMapBoundaryPoint(treeOfLife, oBreeTownSquare, MapType.Intangible, MapType.BreeStreets);

            Room branch = AddRoom("Branch", Room.UNKNOWN_ROOM);
            AddExit(treeOfLife, branch, "branch");
            intangibleGraph.Rooms[branch] = new PointF(-1, 1);

            Room oLimbo = AddRoom("Limbo", "Limbo");
            oLimbo.Intangible = true;
            Exit e = AddExit(oLimbo, treeOfLife, "green door");
            e.MustOpen = true;
            intangibleGraph.Rooms[oLimbo] = new PointF(1, 2);

            Room oDarkTunnel = AddRoom("Dark Tunnel", "Dark Tunnel");
            oDarkTunnel.Intangible = true;
            e = AddExit(oLimbo, oDarkTunnel, "blue door");
            e.MustOpen = true;
            e.MinimumLevel = 4;
            AddExit(oDarkTunnel, healingHand, "light");
            intangibleGraph.Rooms[oDarkTunnel] = new PointF(1, 1);
            imladrisGraph.Rooms[oDarkTunnel] = new PointF(5, 4);
            AddMapBoundaryPoint(oDarkTunnel, healingHand, MapType.Intangible, MapType.Imladris);

            Room oFluffyCloudsAboveNindamos = AddRoom("Fluffy Clouds", "Fluffy clouds above Nindamos");
            oFluffyCloudsAboveNindamos.Intangible = false;
            e = AddExit(oLimbo, oFluffyCloudsAboveNindamos, "white door");
            e.MustOpen = true;
            AddExit(oFluffyCloudsAboveNindamos, nindamosVillageCenter, "green");
            intangibleGraph.Rooms[oFluffyCloudsAboveNindamos] = new PointF(2, 1);
            nindamosGraph.Rooms[oFluffyCloudsAboveNindamos] = new PointF(7, 3);
            AddMapBoundaryPoint(oFluffyCloudsAboveNindamos, nindamosVillageCenter, MapType.Intangible, MapType.Nindamos);

            Room oGuildedTunnel = AddRoom("Guilded Tunnel", "Guilded Tunnel");
            e = AddExit(treeOfLife, oGuildedTunnel, "guild halls");
            e.MustOpen = true;
            intangibleGraph.Rooms[oGuildedTunnel] = new PointF(-1, 0);
            intangibleGraph.Rooms[accursedGuildHall] = new PointF(-2, 0);
            breeStreetsGraph.Rooms[oGuildedTunnel] = new PointF(6, -2);
            AddMapBoundaryPoint(oGuildedTunnel, accursedGuildHall, MapType.Intangible, MapType.BreeStreets);
            AddExit(oGuildedTunnel, accursedGuildHall, "accursed guild");

            Room oGuildedTunnel2 = AddRoom("Guilded Tunnel", "Guilded Tunnel");
            AddBidirectionalExits(oGuildedTunnel2, oGuildedTunnel, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildedTunnel2] = new PointF(-1, -1);

            Room oGuildedTunnel3 = AddRoom("Guilded Tunnel", "Guilded Tunnel");
            AddBidirectionalExits(oGuildedTunnel3, oGuildedTunnel2, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildedTunnel3] = new PointF(-1, -2);
            intangibleGraph.Rooms[crusaderGuildHall] = new PointF(-2, -2);
            breeStreetsGraph.Rooms[oGuildedTunnel3] = new PointF(8, -2);
            AddMapBoundaryPoint(oGuildedTunnel3, crusaderGuildHall, MapType.Intangible, MapType.BreeStreets);
            AddExit(oGuildedTunnel3, crusaderGuildHall, "crusader guild");

            Room oGuildStreet1 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet1, oGuildedTunnel3, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet1] = new PointF(-1, -3);

            Room oGuildStreet2 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet2, oGuildStreet1, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet2] = new PointF(-1, -4);

            Room oGuildStreet3 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet3, oGuildStreet2, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet3] = new PointF(-1, -5);

            Room oGuildStreet4 = AddRoom("Guild Street", "Guild Street");
            AddBidirectionalExits(oGuildStreet4, oGuildStreet3, BidirectionalExitType.NorthSouth);
            intangibleGraph.Rooms[oGuildStreet4] = new PointF(-1, -6);
            intangibleGraph.Rooms[thiefGuildHall] = new PointF(-2, -6);
            breeStreetsGraph.Rooms[oGuildStreet4] = new PointF(9, -2);
            AddMapBoundaryPoint(oGuildStreet4, thiefGuildHall, MapType.Intangible, MapType.BreeStreets);
            AddExit(oGuildStreet4, thiefGuildHall, "thieves guild");

            Room oNorthGuildStreet = AddRoom("North Guild Street", "North Guild Street");
            AddBidirectionalExits(oNorthGuildStreet, oGuildStreet4, BidirectionalExitType.NorthSouth);
            AddExit(oNorthGuildStreet, oBreeTownSquare, "out");
            intangibleGraph.Rooms[oNorthGuildStreet] = new PointF(-1, -7);
        }

        private void AddNindamos(out Room oArmenelosGatesOutside, out Room oSouthernJunction, out Room oPathThroughTheValleyHiddenPath, out Room nindamosDocks, out RoomGraph nindamosGraph, out Room nindamosVillageCenter)
        {
            nindamosGraph = _graphs[MapType.Nindamos];

            nindamosVillageCenter = AddRoom("Village Center", "Nindamos Village Center");
            AddPermanentMobs(nindamosVillageCenter, MobTypeEnum.MaxTheVegetableVendor);
            AddPermanentItems(nindamosVillageCenter, ItemTypeEnum.BoxOfStrawberries, ItemTypeEnum.BundleOfWheat, ItemTypeEnum.SackOfPotatoes);
            nindamosGraph.Rooms[nindamosVillageCenter] = new PointF(8, 4);

            Room oSandstoneNorth1 = AddRoom("Sandstone", "Sandstone Road North");
            AddBidirectionalExits(oSandstoneNorth1, nindamosVillageCenter, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneNorth1] = new PointF(8, 3);

            Room oSandstoneNorth2 = AddRoom("Sandstone", "Sandstone Road North");
            AddBidirectionalExits(oSandstoneNorth2, oSandstoneNorth1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneNorth2] = new PointF(8, 2);

            Room oSandyPath1 = AddRoom("Sandy Path", "Sandy Path");
            AddBidirectionalExits(oSandstoneNorth2, oSandyPath1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyPath1] = new PointF(8.75F, 2);

            Room oSandyPath2 = AddRoom("Sandy Path", "Sandy Path");
            AddBidirectionalExits(oSandyPath1, oSandyPath2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyPath2] = new PointF(9.5F, 2);

            Room oSandyPath3 = AddRoom("Sandy Path", "Sandy Path");
            AddBidirectionalExits(oSandyPath2, oSandyPath3, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyPath3] = new PointF(9.5F, 2.3F);

            Room oMarketplace = AddRoom("Marketplace", "The Marketplace");
            Exit e = AddExit(oSandyPath3, oMarketplace, "door");
            e.MustOpen = true;
            e.RequiresDay = true;
            AddExit(oMarketplace, oSandyPath3, "door");
            nindamosGraph.Rooms[oMarketplace] = new PointF(9.5F, 2.6F);

            Room oSmithy = AddRoom("Smithy", "The Marketplace");
            AddPermanentMobs(oSmithy, MobTypeEnum.Smithy);
            AddBidirectionalExits(oSmithy, oMarketplace, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSmithy] = new PointF(8.5F, 2.6F);

            Room oSandstoneDrivel = AddRoom("Drivel/Sandstone", "Sandstone Road / Drivel Avenue");
            AddBidirectionalExits(oSandstoneDrivel, oSandstoneNorth2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneDrivel] = new PointF(8, 1);

            Room oSandstoneSouth1 = AddRoom("Sandstone", "Sandstone Road South");
            AddBidirectionalExits(nindamosVillageCenter, oSandstoneSouth1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneSouth1] = new PointF(8, 5);

            Room oSandstoneSouth2 = AddRoom("Sandstone", "Sandstone Road South");
            AddBidirectionalExits(oSandstoneSouth1, oSandstoneSouth2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandstoneSouth2] = new PointF(8, 6);

            Room oKKsIronWorksKosta = AddRoom("Kosta", "KK's Ironworks");
            AddPermanentMobs(oKKsIronWorksKosta, MobTypeEnum.Kosta);
            e = AddBidirectionalExitsWithOut(oSandstoneSouth2, oKKsIronWorksKosta, "path");
            e.RequiresDay = true;
            nindamosGraph.Rooms[oKKsIronWorksKosta] = new PointF(7, 6);

            Room oKauka = AddRoom("Kauka", "Kauka's Living Room");
            AddPermanentMobs(oKauka, MobTypeEnum.Kauka);
            AddBidirectionalExitsWithOut(oKKsIronWorksKosta, oKauka, "doorway");
            nindamosGraph.Rooms[oKauka] = new PointF(7, 7);

            Room oLimestoneSandstone = AddRoom("Limestone/Sandstone", "Sandstone Road / Limestone Street");
            AddBidirectionalExits(oSandstoneSouth2, oLimestoneSandstone, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oLimestoneSandstone] = new PointF(8, 7);

            Room oDrivel1 = AddRoom("Drivel", "Drivel Avenue");
            AddBidirectionalExits(oSandstoneDrivel, oDrivel1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oDrivel1] = new PointF(9, 1);

            Room oDrivel2 = AddRoom("Drivel", "Drivel Avenue");
            AddBidirectionalExits(oDrivel1, oDrivel2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oDrivel2] = new PointF(10, 1);

            Room oDrivelElysia = AddRoom("Drivel/Elysia", "Elysia Street / Drivel Avenue");
            AddBidirectionalExits(oDrivel2, oDrivelElysia, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oDrivelElysia] = new PointF(11, 1);

            Room oSandyBeach1 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oDrivelElysia, oSandyBeach1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyBeach1] = new PointF(12, 1);

            Room oPaledasenta1 = AddRoom("Paledasenta", "Paledasenta Street");
            AddBidirectionalExits(nindamosVillageCenter, oPaledasenta1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oPaledasenta1] = new PointF(9, 4);

            Room oNindamosPostOffice = AddRoom("Post Office", "Nindamos Post Office");
            AddBidirectionalExitsWithOut(oPaledasenta1, oNindamosPostOffice, "south");
            nindamosGraph.Rooms[oNindamosPostOffice] = new PointF(9, 5);

            Room oPaledasenta2 = AddRoom("Paledasenta", "Paledasenta Street");
            AddBidirectionalExits(oPaledasenta1, oPaledasenta2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oPaledasenta2] = new PointF(10, 4);

            Room oHealthCenter = AddHealingRoom("Health Center", "Nindamos Health Center", HealingRoom.Nindamos);
            AddBidirectionalExits(oHealthCenter, oPaledasenta2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oHealthCenter] = new PointF(10, 3.5F);

            Room oPaledasentaElysia = AddRoom("Paledasenta/Elysia", "Elysia Street / Paledasenta Street");
            AddBidirectionalExits(oPaledasenta2, oPaledasentaElysia, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oPaledasentaElysia] = new PointF(11, 4);

            Room oSandyBeach4 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oPaledasentaElysia, oSandyBeach4, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyBeach4] = new PointF(12, 4);

            Room oLimestone1 = AddRoom("Limestone", "Limestone Street");
            AddBidirectionalExits(oLimestoneSandstone, oLimestone1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oLimestone1] = new PointF(9, 7);

            Room oSandPlaygroundSW = AddRoom("Malika", "The Sand Playground");
            AddPermanentMobs(oSandPlaygroundSW, MobTypeEnum.Malika);
            AddBidirectionalExits(oSandPlaygroundSW, oLimestone1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandPlaygroundSW] = new PointF(9, 6.5F);

            Room oSandPlaygroundNW = AddRoom("Sand Playground", "Sand Playground");
            AddBidirectionalExits(oSandPlaygroundNW, oSandPlaygroundSW, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandPlaygroundNW] = new PointF(9, 6);

            Room oSandPlaygroundNE = AddRoom("Sand Playground", "The Sand Playground");
            AddBidirectionalExits(oSandPlaygroundNW, oSandPlaygroundNE, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandPlaygroundNE] = new PointF(10, 6);

            Room oSandPlaygroundSE = AddRoom("Sand Playground", "The Sand Playground");
            AddBidirectionalExits(oSandPlaygroundNE, oSandPlaygroundSE, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandPlaygroundSW, oSandPlaygroundSE, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandPlaygroundSE] = new PointF(10, 6.5F);

            Room oSandcastle = AddRoom("sobbing girl", "Inside the Sandcastle");
            AddPermanentMobs(oSandcastle, MobTypeEnum.SobbingGirl);
            AddBidirectionalExitsWithOut(oSandPlaygroundNE, oSandcastle, "sandcastle");
            nindamosGraph.Rooms[oSandcastle] = new PointF(10, 5.5F);

            Room oLimestone2 = AddRoom("Limestone", "Limestone Street");
            AddBidirectionalExits(oLimestone1, oLimestone2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oLimestone2] = new PointF(10, 7);

            Room oLimestoneElysia = AddRoom("Numenorean Warder", "Elysia Street / Limestone Street");
            AddPermanentMobs(oLimestoneElysia, MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oLimestone2, oLimestoneElysia, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oLimestoneElysia] = new PointF(11, 7);

            Room oSandyBeach7 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oLimestoneElysia, oSandyBeach7, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oSandyBeach7] = new PointF(12, 7);

            Room oSandyBeach2 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach1, oSandyBeach2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach2] = new PointF(12, 2);

            Room oSandyBeach3 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach2, oSandyBeach3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeach3, oSandyBeach4, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach3] = new PointF(12, 3);

            Room oSandyBeach5 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach4, oSandyBeach5, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach5] = new PointF(12, 5);

            Room oSandyBeach6 = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach5, oSandyBeach6, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeach6, oSandyBeach7, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeach6] = new PointF(12, 6);

            Room oSandyBeachNorth = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeachNorth, oSandyBeach1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeachNorth] = new PointF(12, 0);

            Room oSandyBeachSouth = AddRoom("Sandy Beach", "Sandy Beach");
            AddBidirectionalExits(oSandyBeach7, oSandyBeachSouth, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oSandyBeachSouth] = new PointF(12, 8);

            Room oShoreline1 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oSandyBeachNorth, oShoreline1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oShoreline1] = new PointF(13, 0);

            Room oShoreline2 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oSandyBeach1, oShoreline2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oShoreline1, oShoreline2, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline2] = new PointF(13, 1);

            Room oShoreline3 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline2, oShoreline3, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline3] = new PointF(13, 2);

            Room oShoreline4 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline3, oShoreline4, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline4] = new PointF(13, 3);

            Room oShoreline5 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline4, oShoreline5, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeach4, oShoreline5, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oShoreline5] = new PointF(13, 4);

            Room oShoreline6 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline5, oShoreline6, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline6] = new PointF(13, 5);

            Room oShoreline7 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline6, oShoreline7, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline7] = new PointF(13, 6);

            Room oShoreline8 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline7, oShoreline8, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oShoreline8] = new PointF(13, 7);

            Room oSmallDock = AddRoom("Small Dock", "A Small Dock");
            e = AddExit(oShoreline8, oSmallDock, "east");
            e.Hidden = true;
            AddExit(oSmallDock, oShoreline8, "west");
            nindamosGraph.Rooms[oSmallDock] = new PointF(14, 7);

            nindamosDocks = AddRoom("Small Dock", "A Small Dock");
            nindamosDocks.BoatLocationType = BoatEmbarkOrDisembark.BullroarerNindamos;
            AddBidirectionalExits(oSmallDock, nindamosDocks, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[nindamosDocks] = new PointF(15, 7);

            AddGreatWesternOcean(nindamosDocks);

            Room oShoreline9 = AddRoom("Shoreline", "Shoreline");
            AddBidirectionalExits(oShoreline8, oShoreline9, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSandyBeachSouth, oShoreline9, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oShoreline9] = new PointF(13, 8);

            Room oElysia1 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oDrivelElysia, oElysia1, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia1] = new PointF(11, 2);

            Room oElysia2 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oElysia1, oElysia2, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElysia2, oPaledasentaElysia, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia2] = new PointF(11, 3);

            Room oHestasMarket = AddRoom("Hesta's Market", "Hesta's Market");
            AddPermanentMobs(oHestasMarket, MobTypeEnum.Hesta);
            e = AddBidirectionalExitsWithOut(oElysia2, oHestasMarket, "market");
            e.RequiresDay = true;
            nindamosGraph.Rooms[oHestasMarket] = new PointF(10, 3);

            Room oElysia3 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oPaledasentaElysia, oElysia3, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia3] = new PointF(11, 5);

            Room oElysia4 = AddRoom("Elysia", "Elysia Street");
            AddBidirectionalExits(oElysia3, oElysia4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElysia4, oLimestoneElysia, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oElysia4] = new PointF(11, 6);

            Room oGranitePath1 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath1, nindamosVillageCenter, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath1] = new PointF(7, 4);

            Room oGranitePath2 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath2, oGranitePath1, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath2] = new PointF(6, 4);

            Room oAlasse = AddRoom("Alasse's Pub", "Alasse's Pub");
            AddPermanentMobs(oAlasse, MobTypeEnum.Alasse);
            e = AddBidirectionalExitsWithOut(oGranitePath2, oAlasse, "south");
            e.RequiresDay = true;
            nindamosGraph.Rooms[oAlasse] = new PointF(6, 5);

            Room oGranitePath3 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath3, oGranitePath2, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath3] = new PointF(5, 4);

            Room oGranitePath4 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath4, oGranitePath3, BidirectionalExitType.SoutheastNorthwest);
            nindamosGraph.Rooms[oGranitePath4] = new PointF(4, 3);

            Room oGranitePath5 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath5, oGranitePath4, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath5] = new PointF(3, 3);

            Room oGranitePath6 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath6, oGranitePath5, BidirectionalExitType.WestEast);
            nindamosGraph.Rooms[oGranitePath6] = new PointF(2, 3);

            Room oGranitePath7 = AddRoom("Granite Path", "Granite Path");
            AddBidirectionalExits(oGranitePath7, oGranitePath6, BidirectionalExitType.SoutheastNorthwest);
            nindamosGraph.Rooms[oGranitePath7] = new PointF(1, 2);

            oSouthernJunction = AddRoom("Southern Junction", "Southern Junction");
            AddPermanentMobs(oSouthernJunction, MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oSouthernJunction, oGranitePath7, BidirectionalExitType.SoutheastNorthwest);
            nindamosGraph.Rooms[oSouthernJunction] = new PointF(0, 1);

            Room oPathToArmenelos1 = AddRoom("Valley Path", "Path Through the Valley");
            AddBidirectionalExits(oPathToArmenelos1, oSouthernJunction, BidirectionalExitType.SouthwestNortheast);
            nindamosGraph.Rooms[oPathToArmenelos1] = new PointF(1, 0);

            oPathThroughTheValleyHiddenPath = AddRoom("Valley Path", "Path Through the Valley");
            AddBidirectionalExits(oPathThroughTheValleyHiddenPath, oPathToArmenelos1, BidirectionalExitType.SouthwestNortheast);
            nindamosGraph.Rooms[oPathThroughTheValleyHiddenPath] = new PointF(2, -1);

            oArmenelosGatesOutside = AddRoom("Gate Outside", "Gates of Armenelos");
            AddPermanentMobs(oArmenelosGatesOutside, MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oArmenelosGatesOutside, oPathThroughTheValleyHiddenPath, BidirectionalExitType.NorthSouth);
            nindamosGraph.Rooms[oArmenelosGatesOutside] = new PointF(2, -2);
        }

        private void AddGreatWesternOcean(Room nindamosDocks)
        {
            RoomGraph nindamosGraph = _graphs[MapType.Nindamos];
            RoomGraph greatWesternOceanGraph = _graphs[MapType.GreatWesternOcean];

            greatWesternOceanGraph.Rooms[nindamosDocks] = new PointF(8, 2);

            Room oGreatWesternOcean1 = AddRoom("Ocean", "Great Western Ocean");
            AddPermanentMobs(oGreatWesternOcean1, MobTypeEnum.GiantStingray);
            Exit e = AddExit(nindamosDocks, oGreatWesternOcean1, "down");
            e.Hidden = true;
            e = AddExit(oGreatWesternOcean1, nindamosDocks, "dock");
            e.Hidden = true;
            nindamosGraph.Rooms[oGreatWesternOcean1] = new PointF(15, 8);
            greatWesternOceanGraph.Rooms[oGreatWesternOcean1] = new PointF(8, 3);

            AddMapBoundaryPoint(nindamosDocks, oGreatWesternOcean1, MapType.Nindamos, MapType.GreatWesternOcean);

            Room oGreatWesternOcean2 = AddRoom("Ocean", "Great Western Ocean");
            AddBidirectionalExits(oGreatWesternOcean2, oGreatWesternOcean1, BidirectionalExitType.WestEast);
            greatWesternOceanGraph.Rooms[oGreatWesternOcean2] = new PointF(7, 3);
            //This room shifts. It may or may not have a south exit.

            Room oGreatWesternOcean3 = AddRoom("Ocean", "Great Western Ocean");
            AddBidirectionalExits(oGreatWesternOcean3, oGreatWesternOcean2, BidirectionalExitType.WestEast);
            greatWesternOceanGraph.Rooms[oGreatWesternOcean3] = new PointF(6, 3);

            Room oIslandOfGiants = AddRoom("Giant Island", "Island of Giants");
            AddExit(oGreatWesternOcean3, oIslandOfGiants, "shoal");
            AddExit(oIslandOfGiants, oGreatWesternOcean3, "ocean");
            greatWesternOceanGraph.Rooms[oIslandOfGiants] = new PointF(5, 3);
        }

        private void AddArmenelos(Room oArmenelosGatesOutside)
        {
            RoomGraph armenelosGraph = _graphs[MapType.Armenelos];
            RoomGraph nindamosGraph = _graphs[MapType.Nindamos];

            Room oAdrahilHirgon = AddRoom("Adrahil/Hirgon", "Hirgon Way/ Adrahil Road");
            armenelosGraph.Rooms[oAdrahilHirgon] = new PointF(0, 0);

            Room oAdrahil1 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahilHirgon, oAdrahil1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil1] = new PointF(1, 0);

            Room oAdrahilRivel = AddRoom("Adrahil/Rivel", "Adrahil Road/Rivel Way");
            AddBidirectionalExits(oAdrahil1, oAdrahilHirgon, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilRivel] = new PointF(2, 0);

            Room oAdrahil2 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahilRivel, oAdrahil2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil2] = new PointF(3, 0);

            Room oAdrahil3 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahil2, oAdrahil3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil3] = new PointF(4, 0);

            Room oAdrahil4 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahil3, oAdrahil4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil4] = new PointF(5, 0);

            Room oCityDump = AddRoom("City Dump", "Armenelos City Dump");
            AddBidirectionalExitsWithOut(oAdrahil4, oCityDump, "gate");
            armenelosGraph.Rooms[oCityDump] = new PointF(5, 1);

            Room oDori = AddRoom("Dori", "Dori's Dump Shack");
            AddPermanentMobs(oDori, MobTypeEnum.Dori);
            AddBidirectionalExitsWithOut(oCityDump, oDori, "dump");
            armenelosGraph.Rooms[oDori] = new PointF(4, 1);

            Room oAdrahilFolca = AddRoom("Adrahil/Folca", "Adrahil Road/Folca Street");
            AddBidirectionalExits(oAdrahil4, oAdrahilFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilFolca] = new PointF(6, 0);

            Room oAdrahil5 = AddRoom("Adrahil", "Adrahil Road");
            AddBidirectionalExits(oAdrahilFolca, oAdrahil5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahil5] = new PointF(7, 0);

            Room oAdrahilWindfola = AddRoom("Adrahil/Windfola", "Adrahil Road/Windfola Avenue");
            AddBidirectionalExits(oAdrahil5, oAdrahilWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAdrahilWindfola] = new PointF(8, 0);

            Room oHirgon1 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oAdrahilHirgon, oHirgon1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon1] = new PointF(0, 1);

            Room oDoctorFaramir = AddRoom("Dr Faramir", "Dr. Faramir's Medical Supplies");
            AddPermanentMobs(oDoctorFaramir, MobTypeEnum.DrFaramir);
            AddBidirectionalExitsWithOut(oHirgon1, oDoctorFaramir, "door");
            armenelosGraph.Rooms[oDoctorFaramir] = new PointF(1, 1);

            Room oRivel1 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oAdrahilRivel, oRivel1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel1] = new PointF(2, 1);

            Room oFolca1 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oAdrahilFolca, oFolca1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca1] = new PointF(6, 1);

            Room oWindfola1 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oAdrahilWindfola, oWindfola1, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola1] = new PointF(8, 1);

            Room oDorlasHirgon = AddRoom("Dorlas/Hirgon", "Hirgon Way/Dorlas Street");
            AddBidirectionalExits(oHirgon1, oDorlasHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oDorlasHirgon] = new PointF(0, 2);

            Room oDorlas1 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlasHirgon, oDorlas1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas1] = new PointF(1, 2);

            Room oDorlasRivel = AddRoom("Dorlas/Rivel", "Dorlas Street/Rivel Way");
            AddBidirectionalExits(oRivel1, oDorlasRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas1, oDorlasRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasRivel] = new PointF(2, 2);

            Room oDorlas2 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlasRivel, oDorlas2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas2] = new PointF(3, 2);

            Room oDorlas3 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlas2, oDorlas3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas3] = new PointF(4, 2);

            Room oDorlas4 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlas3, oDorlas4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas4] = new PointF(5, 2);

            Room oDorlasFolca = AddRoom("Dorlas/Folca", "Dorlas Street/Folca Avenue");
            AddBidirectionalExits(oFolca1, oDorlasFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas4, oDorlasFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasFolca] = new PointF(6, 2);

            Room oDorlas5 = AddRoom("Dorlas", "Dorlas Street");
            AddBidirectionalExits(oDorlasFolca, oDorlas5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlas5] = new PointF(7, 2);

            Room oTamar = AddRoom("Tamar", "Tamar of Armenelos");
            AddPermanentMobs(oTamar, MobTypeEnum.Tamar);
            AddBidirectionalExits(oDorlas5, oTamar, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oTamar] = new PointF(7, 2.5F);

            Room oDorlasWindfola = AddRoom("Dorlas/Windfola", "Windfola Avenue/Dorlas Street");
            AddBidirectionalExits(oWindfola1, oDorlasWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorlas5, oDorlasWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oDorlasWindfola] = new PointF(8, 2);

            Room oHirgon2 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oDorlasHirgon, oHirgon2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon2] = new PointF(0, 3);

            Room oRivel2 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oDorlasRivel, oRivel2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel2] = new PointF(2, 3);

            Room oFolca2 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oDorlasFolca, oFolca2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca2] = new PointF(6, 3);

            Room oWindfola2 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oDorlasWindfola, oWindfola2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola2] = new PointF(8, 3);

            Room oAzgara = AddRoom("Azgara", "Azgara's Metalworking");
            AddPermanentMobs(oAzgara, MobTypeEnum.Azgara);
            AddBidirectionalExits(oAzgara, oWindfola2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oAzgara] = new PointF(7, 3);

            Room oOnlyArmor = AddRoom("Kali", "Only Armor");
            AddPermanentMobs(oOnlyArmor, MobTypeEnum.Kali);
            AddBidirectionalExitsWithOut(oAzgara, oOnlyArmor, "door");
            armenelosGraph.Rooms[oOnlyArmor] = new PointF(6.5F, 3.5F);

            Room oSpecialtyShoppe = AddRoom("Specialty", "Azgara's Specialty Shoppe");
            AddBidirectionalExitsWithOut(oAzgara, oSpecialtyShoppe, "curtain");
            armenelosGraph.Rooms[oSpecialtyShoppe] = new PointF(7.5F, 3.5F);

            Room oThalosHirgon = AddRoom("Hirgon/Thalos", "Hirgon Way/West Thalos Road");
            AddBidirectionalExits(oHirgon2, oThalosHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oThalosHirgon] = new PointF(0, 4);

            Room oThalos1 = AddRoom("Thalos", "West Thalos Road");
            AddBidirectionalExits(oThalosHirgon, oThalos1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos1] = new PointF(1, 4);

            Room oThalosRivel = AddRoom("Thalos/Rivel", "West Thalos Road/Rivel Way");
            AddBidirectionalExits(oRivel2, oThalosRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos1, oThalosRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosRivel] = new PointF(2, 4);

            Room oThalos2 = AddRoom("Thalos", "West Thalos Road");
            AddBidirectionalExits(oThalosRivel, oThalos2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos2] = new PointF(3, 4);

            Room oThalos3 = AddRoom("Thalos", "Thalos Road");
            AddBidirectionalExits(oThalos2, oThalos3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos3] = new PointF(4, 4);

            Room oThalos4 = AddRoom("Thalos", "East Thalos Road");
            AddBidirectionalExits(oThalos3, oThalos4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos4] = new PointF(5, 4);

            Room oThalosFolca = AddRoom("Thalos/Folca", "East Thalos Road/Folca Avenue");
            AddBidirectionalExits(oFolca2, oThalosFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos4, oThalosFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosFolca] = new PointF(6, 4);

            Room oThalos5 = AddRoom("Thalos", "East Thalos Road");
            AddBidirectionalExits(oThalosFolca, oThalos5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalos5] = new PointF(7, 4);

            Room oThalosWindfola = AddRoom("Thalos/Windfola", "Windfola Avenue/ East Thalos Road");
            AddBidirectionalExits(oWindfola2, oThalosWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oThalos5, oThalosWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oThalosWindfola] = new PointF(8, 4);

            Room oHirgon3 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oThalosHirgon, oHirgon3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon3] = new PointF(0, 5);

            Room oRivel3 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oThalosRivel, oRivel3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel3] = new PointF(2, 5);
            //CSRTODO: south (blocked)

            Room oFolca3 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oThalosFolca, oFolca3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca3] = new PointF(6, 5);
            //CSRTODO: south (blocked)

            Room oWindfola3 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oThalosWindfola, oWindfola3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola3] = new PointF(8, 5);

            Room oEllessarHirgon = AddRoom("Ellessar/Hirgon", "Hirgon Way/Ellessar Street");
            AddBidirectionalExits(oHirgon3, oEllessarHirgon, BidirectionalExitType.NorthSouth);
            //CSRTODO: east (blocked)
            armenelosGraph.Rooms[oEllessarHirgon] = new PointF(0, 6);

            Room oEllessarWindfola = AddRoom("Ellessar/Windfola", "Windfola Avenue/Ellessar Street");
            AddBidirectionalExits(oWindfola3, oEllessarWindfola, BidirectionalExitType.NorthSouth);
            //CSRTODO: west (blocked)
            armenelosGraph.Rooms[oEllessarWindfola] = new PointF(8, 6);

            Room oHirgon4 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oEllessarHirgon, oHirgon4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon4] = new PointF(0, 7);

            Room oOutdoorMarket = AddRoom("OutdoorMarket", "Outdoor Market");
            AddBidirectionalExits(oHirgon4, oOutdoorMarket, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOutdoorMarket] = new PointF(1, 7);

            Room oRivel4 = AddRoom("Rivel", "Rivel Way");
            armenelosGraph.Rooms[oRivel4] = new PointF(2, 7);
            //CSRTODO: north (blocked)

            Room oFolca4 = AddRoom("Folca", "Folca Avenue");
            armenelosGraph.Rooms[oFolca4] = new PointF(6, 7);
            //CSRTODO: north (blocked)

            Room oWindfola4 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oEllessarWindfola, oWindfola4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola4] = new PointF(8, 7);

            Room oOrithilHirgon = AddRoom("Orithil/Hirgon", "Hirgon Way/Orithil Drive");
            AddBidirectionalExits(oHirgon4, oOrithilHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oOrithilHirgon] = new PointF(0, 8);

            Room oOrithil1 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOutdoorMarket, oOrithil1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithilHirgon, oOrithil1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil1] = new PointF(1, 8);

            Room oOrithilRivel = AddRoom("Orithil/Rivel", "Orithil Drive/Rivel Way");
            AddBidirectionalExits(oRivel4, oOrithilRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil1, oOrithilRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilRivel] = new PointF(2, 8);

            Room oOrithil2 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithilRivel, oOrithil2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil2] = new PointF(3, 8);

            Room oOrithil3 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithil2, oOrithil3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil3] = new PointF(4, 8);

            Room oOrithil4 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithil3, oOrithil4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil4] = new PointF(5, 8);

            Room oYurahtamJewelers = AddRoom("Yurahtam Jewlers", "Yurahtam Jewlers");
            AddBidirectionalExitsWithOut(oOrithil4, oYurahtamJewelers, "south");
            armenelosGraph.Rooms[oYurahtamJewelers] = new PointF(5, 8.5F);

            Room oOrithilFolca = AddRoom("Orithil/Folca", "Orithil Drive/Folca Street");
            AddBidirectionalExits(oFolca4, oOrithilFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil4, oOrithilFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilFolca] = new PointF(6, 8);

            Room oOrithil5 = AddRoom("Orithil", "Orithil Drive");
            AddBidirectionalExits(oOrithilFolca, oOrithil5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithil5] = new PointF(7, 8);
            //CSRTODO: archway (blocked)

            Room oOrithilWindfola = AddRoom("Orithil/Windfola", "Windfola Avenue/Orithil Drive");
            AddBidirectionalExits(oWindfola4, oOrithilWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oOrithil5, oOrithilWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oOrithilWindfola] = new PointF(8, 8);

            Room oHirgon5 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oOrithilHirgon, oHirgon5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon5] = new PointF(0, 9);

            Room oStairwayLanding = AddRoom("Stairway Landing", "Stairway Landing");
            AddExit(oHirgon5, oStairwayLanding, "stairway");
            AddExit(oStairwayLanding, oHirgon5, "down");
            armenelosGraph.Rooms[oStairwayLanding] = new PointF(1, 9);

            Room oAmme = AddRoom("Amme", "Commoner's Home");
            AddPermanentMobs(oAmme, MobTypeEnum.Amme);
            AddBidirectionalExitsWithOut(oStairwayLanding, oAmme, "doorway");
            armenelosGraph.Rooms[oAmme] = new PointF(1, 8.5F);

            Room oRivel5 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oOrithilRivel, oRivel5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel5] = new PointF(2, 9);

            Room oFolca5 = AddRoom("Folca", "Folca Avenue");
            AddBidirectionalExits(oOrithilFolca, oFolca5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca5] = new PointF(6, 9);

            Room oWindfola5 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oOrithilWindfola, oWindfola5, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola5] = new PointF(8, 9);

            Room oBalanHirgon = AddRoom("Balan/Hirgon", "Hirgon Way/Balan Avenue");
            AddBidirectionalExits(oHirgon5, oBalanHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oBalanHirgon] = new PointF(0, 10);

            Room oBalan1 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalanHirgon, oBalan1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan1] = new PointF(1, 10);

            Room oBalanRivel = AddRoom("Balan/Rivel", "Balan Avenue/Rivel Way");
            AddBidirectionalExits(oRivel5, oBalanRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan1, oBalanRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanRivel] = new PointF(2, 10);

            Room oBalan2 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalanRivel, oBalan2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan2] = new PointF(3, 10);

            Room oBalan3 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalan2, oBalan3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan3] = new PointF(4, 10);

            Room oBalan4 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalan3, oBalan4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan4] = new PointF(5, 10);

            Room oMerchantsMarket1 = AddRoom("Merchant Market", "Merchant's Market");
            AddBidirectionalExits(oMerchantsMarket1, oBalan2, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket1] = new PointF(3, 9.5F);

            Room oCultOfAzogHQ = AddRoom("Cult of Azoq HQ", "Cult of Azog Headquarters");
            AddPermanentMobs(oCultOfAzogHQ, MobTypeEnum.Voteli);
            Exit e = AddBidirectionalExitsWithOut(oMerchantsMarket1, oCultOfAzogHQ, "tent");
            e.Hidden = true;
            armenelosGraph.Rooms[oCultOfAzogHQ] = new PointF(3, 9);

            Room oMerchantsMarket2 = AddRoom("Merchant Market", "Merchant's Market");
            AddBidirectionalExits(oMerchantsMarket1, oMerchantsMarket2, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMerchantsMarket2, oBalan3, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket2] = new PointF(4, 9.5F);

            Room oMerchantsMarket3 = AddRoom("Merchant Market", "Merchant's Market");
            AddBidirectionalExits(oMerchantsMarket2, oMerchantsMarket3, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMerchantsMarket3, oBalan4, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oMerchantsMarket3] = new PointF(5, 9.5F);

            Room oBalanFolca = AddRoom("Balan/Folca", "Folca Street/Balan Avenue");
            AddBidirectionalExits(oFolca5, oBalanFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan4, oBalanFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanFolca] = new PointF(6, 10);

            Room oBalan5 = AddRoom("Balan", "Balan Avenue");
            AddBidirectionalExits(oBalanFolca, oBalan5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalan5] = new PointF(7, 10);

            Room oBalanWindfola = AddRoom("Balan/Windfola", "Windfola Avenue/Balan Avenue");
            AddBidirectionalExits(oWindfola5, oBalanWindfola, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBalan5, oBalanWindfola, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oBalanWindfola] = new PointF(8, 10);

            Room oHirgon6 = AddRoom("Hirgon", "Hirgon Way");
            AddBidirectionalExits(oBalanHirgon, oHirgon6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oHirgon6] = new PointF(0, 11);

            Room oRivel6 = AddRoom("Rivel", "Rivel Way");
            AddBidirectionalExits(oBalanRivel, oRivel6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oRivel6] = new PointF(2, 11);

            Room oFolca6 = AddRoom("Folca", "Folca Street");
            AddBidirectionalExits(oBalanFolca, oFolca6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oFolca6] = new PointF(6, 11);

            Room oWindfola6 = AddRoom("Windfola", "Windfola Avenue");
            AddBidirectionalExits(oBalanWindfola, oWindfola6, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oWindfola6] = new PointF(8, 11);

            Room oGoldberryHirgon = AddRoom("Goldberry/Hirgon", "Goldberry Road/Hirgon Way");
            AddBidirectionalExits(oHirgon6, oGoldberryHirgon, BidirectionalExitType.NorthSouth);
            armenelosGraph.Rooms[oGoldberryHirgon] = new PointF(0, 12);

            Room oGoldberry1 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberryHirgon, oGoldberry1, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry1] = new PointF(1, 12);

            Room oImrahil = AddRoom("Imrahil", "Imrahil's Pub");
            AddPermanentMobs(oImrahil, MobTypeEnum.Imrahil);
            AddBidirectionalSameNameExit(oGoldberry1, oImrahil, "swinging");
            armenelosGraph.Rooms[oImrahil] = new PointF(1, 11);

            Room oGoldberryRivel = AddRoom("Goldberry/Rivel", "Goldberry Road/Rivil Way");
            AddBidirectionalExits(oRivel6, oGoldberryRivel, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry1, oGoldberryRivel, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberryRivel] = new PointF(2, 12);

            Room oGoldberry2 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberryRivel, oGoldberry2, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry2] = new PointF(3, 12);

            Room oGoldberry3 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberry2, oGoldberry3, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry3] = new PointF(4, 12);

            Room oHummley = AddRoom("Hummley", "Hummley's Shop o' Fun");
            AddPermanentMobs(oHummley, MobTypeEnum.Hummley);
            AddBidirectionalExitsWithOut(oGoldberry3, oHummley, "doorway");
            armenelosGraph.Rooms[oHummley] = new PointF(4, 11);

            Room oGoldberry4 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberry3, oGoldberry4, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry4] = new PointF(5, 12);

            Room oGoldberryFolca = AddRoom("Goldberry/Folca", "Goldberry Road/Folca Street");
            AddBidirectionalExits(oFolca6, oGoldberryFolca, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry4, oGoldberryFolca, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberryFolca] = new PointF(6, 12);

            Room oGoldberry5 = AddRoom("Goldberry", "Goldberry Road");
            AddBidirectionalExits(oGoldberryFolca, oGoldberry5, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGoldberry5] = new PointF(7, 12);

            Room oZain = AddRoom("Zain", "Tourist Information");
            AddPermanentMobs(oZain, MobTypeEnum.Zain);
            AddBidirectionalExitsWithOut(oGoldberry5, oZain, "north");
            armenelosGraph.Rooms[oZain] = new PointF(7, 11);

            Room oGateInside = AddRoom("Gate Inside", "Entrance to Armenelos");
            AddBidirectionalExits(oWindfola6, oGateInside, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oGoldberry5, oGateInside, BidirectionalExitType.WestEast);
            armenelosGraph.Rooms[oGateInside] = new PointF(8, 12);
            nindamosGraph.Rooms[oGateInside] = new PointF(2, -3);

            AddExit(oGateInside, oArmenelosGatesOutside, "gate");
            e = AddExit(oArmenelosGatesOutside, oGateInside, "gate");
            e.RequiresDay = true;
            armenelosGraph.Rooms[oArmenelosGatesOutside] = new PointF(8, 13);
            AddMapBoundaryPoint(oArmenelosGatesOutside, oGateInside, MapType.Nindamos, MapType.Armenelos);
        }

        private void AddWestOfNindamosAndArmenelos(Room oSouthernJunction, Room oPathThroughTheValley, out Room oEldemondeEastGateOutside, RoomGraph nindamosGraph)
        {
            RoomGraph nindamosEldemondeGraph = _graphs[MapType.NindamosToEldemonde];
            RoomGraph deathValleyGraph = _graphs[MapType.DeathValley];

            Room r;
            Room previousRoom = oSouthernJunction;
            nindamosEldemondeGraph.Rooms[oSouthernJunction] = new PointF(26, 18);
            for (int i = 0; i < 7; i++)
            {
                r = AddRoom("Laiquendi", "Laiquendi");
                AddBidirectionalExits(r, previousRoom, BidirectionalExitType.WestEast);
                nindamosEldemondeGraph.Rooms[r] = new PointF(25 - i, 18);
                if (i == 0)
                {
                    PointF pSJ = nindamosGraph.Rooms[oSouthernJunction];
                    nindamosGraph.Rooms[r] = new PointF(pSJ.X - 1, pSJ.Y);
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
                nindamosEldemondeGraph.Rooms[r] = new PointF(19, 17-i);
                previousRoom = r;
            }
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[r] = new PointF(18, 8);
            previousRoom = r;
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[r] = new PointF(17, 7);
            previousRoom = r;
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[r] = new PointF(17, 6);
            previousRoom = r;
            r = AddRoom("Liara", "Liara");
            AddBidirectionalExits(r, previousRoom, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[r] = new PointF(17, 4);
            previousRoom = r;
            Room oLastLiara = r;

            Room oBaseOfMenelTarma = AddRoom("Base of Menel tarma", "Base of Menel tarma");
            AddPermanentMobs(oBaseOfMenelTarma, MobTypeEnum.NumenoreanWarder);
            AddBidirectionalExits(oBaseOfMenelTarma, previousRoom, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oBaseOfMenelTarma] = new PointF(15, 4);
            AddMenelTarma(oBaseOfMenelTarma, nindamosEldemondeGraph);

            Room oHiddenPath1 = AddRoom("Streambed", "Streambed");
            AddBidirectionalExits(hiddenPathRoom, oHiddenPath1, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath1] = new PointF(20, 13.5F);
            Room oHiddenPath2 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath1, oHiddenPath2, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath2] = new PointF(21, 14);
            Room oHiddenPath3 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath2, oHiddenPath3, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath3] = new PointF(21, 14.5F);
            Room oHiddenPath4 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath3, oHiddenPath4, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath4] = new PointF(21, 15);
            Room oHiddenPath5 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath4, oHiddenPath5, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath5] = new PointF(21, 15.5F);
            Room oHiddenPath6 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath5, oHiddenPath6, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath6] = new PointF(21, 16);
            Room oHiddenPath7 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath6, oHiddenPath7, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath7] = new PointF(21, 16.5F);
            Room oHiddenPath8 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath7, oHiddenPath8, BidirectionalExitType.NorthSouth, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath8] = new PointF(21, 17);
            Room oHiddenPath9 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath8, oHiddenPath9, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath9] = new PointF(22, 17.5F);
            Room oHiddenPath10 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath9, oHiddenPath10, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath10] = new PointF(23, 17.5F);
            Room oHiddenPath11 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath10, oHiddenPath11, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath11] = new PointF(24, 17.5F);
            Room oHiddenPath12 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath12, oHiddenPath11, BidirectionalExitType.SouthwestNortheast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath12] = new PointF(25, 17);
            Room oHiddenPath13 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath12, oHiddenPath13, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath13] = new PointF(26, 17);
            Room oHiddenPath14 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath13, oHiddenPath14, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath14] = new PointF(27, 17);
            Room oHiddenPath15 = AddRoom("Hidden Path", "Hidden Path");
            AddBidirectionalExits(oHiddenPath14, oHiddenPath15, BidirectionalExitType.WestEast, true);
            nindamosEldemondeGraph.Rooms[oHiddenPath15] = new PointF(28, 17);
            AddBidirectionalExits(oHiddenPath15, oPathThroughTheValley, BidirectionalExitType.SoutheastNorthwest, true);
            nindamosEldemondeGraph.Rooms[oPathThroughTheValley] = new PointF(29, 17.5F);
            PointF p = nindamosGraph.Rooms[oPathThroughTheValley];
            nindamosGraph.Rooms[oHiddenPath15] = new PointF(p.X - 1, p.Y - 1);
            AddMapBoundaryPoint(oPathThroughTheValley, oHiddenPath15, MapType.Nindamos, MapType.NindamosToEldemonde);

            Room oGrasslands1 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oSouthernJunction, oGrasslands1, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands1] = new PointF(25, 19);

            Room oGrasslands2 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands2, oGrasslands1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands2] = new PointF(24, 19);

            Room oHostaEncampment = AddRoom("Hosta Encampment", "Hosta Encampment");
            AddPermanentMobs(oHostaEncampment, MobTypeEnum.HostaWarrior);
            AddBidirectionalExits(oHostaEncampment, oGrasslands2, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oHostaEncampment] = new PointF(23, 18.5F);

            Room oGrasslands3 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands2, oGrasslands3, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands3] = new PointF(23, 20);

            Room oGrasslands4 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands4, oGrasslands3, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands4] = new PointF(22, 20);

            Room oGrasslands5 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands5, oGrasslands4, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands5] = new PointF(22, 19);

            Room oGrasslands6 = AddRoom("Grasslands", "Grasslands of Mittalamar");
            AddBidirectionalExits(oGrasslands5, oGrasslands6, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands6] = new PointF(23, 19);

            Room oGrasslands7 = AddRoom("Grasslands", "Grasslands of Mittalamar");
            AddBidirectionalExits(oGrasslands6, oGrasslands7, BidirectionalExitType.NorthSouth);
            AddExit(oGrasslands7, oGrasslands3, "south");
            nindamosEldemondeGraph.Rooms[oGrasslands7] = new PointF(23, 19.5F);

            Room oGrasslands8 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands4, oGrasslands8, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands8] = new PointF(21, 21);

            Room oGrasslands9 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands9, oGrasslands4, BidirectionalExitType.WestEast);
            AddExit(oGrasslands8, oGrasslands9, "north");
            AddExit(oGrasslands9, oGrasslands8, "north");
            nindamosEldemondeGraph.Rooms[oGrasslands9] = new PointF(21, 20);

            Room oGrasslands10 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands4, oGrasslands10, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands10] = new PointF(22, 21);

            Room oGrasslands11 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands10, oGrasslands11, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrasslands11] = new PointF(21, 22);

            Room oGrasslands12 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands12, oGrasslands11, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrasslands12] = new PointF(20, 22);

            Room oGrasslands13 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands13, oGrasslands12, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands13] = new PointF(20, 21);

            Room oGrasslands14 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands14, oGrasslands13, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oGrasslands14] = new PointF(20, 20);

            Room oGrasslands15 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands15, oGrasslands14, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oGrasslands15, oGrasslands5, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oGrasslands15] = new PointF(21, 18.5F);

            Room oGrasslands16 = AddRoom("Grasslands", "Grasslands of Mittalmar");
            AddBidirectionalExits(oGrasslands16, oGrasslands13, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oGrasslands16] = new PointF(19, 20);
            deathValleyGraph.Rooms[oGrasslands16] = new PointF(6, 9.5F);

            Room oDeathValleyEntrance = AddRoom("Death Valley Entrance", "Entrance to the Valley of the Dead");
            AddBidirectionalExits(oGrasslands16, oDeathValleyEntrance, BidirectionalExitType.NorthSouth);
            nindamosEldemondeGraph.Rooms[oDeathValleyEntrance] = new PointF(19, 21);
            AddMapBoundaryPoint(oGrasslands16, oDeathValleyEntrance, MapType.NindamosToEldemonde, MapType.DeathValley);

            Room oGrassCoveredField1 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oLastLiara, oGrassCoveredField1, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField1] = new PointF(16, 6);

            Room oGrassCoveredField2 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField2, oGrassCoveredField1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField2] = new PointF(15, 6);

            Room oGrassCoveredField3 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField3, oGrassCoveredField2, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField3] = new PointF(14, 6);

            Room oGrassCoveredField4 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField4, oGrassCoveredField3, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField4] = new PointF(13, 5);

            Room oRiverPath1 = AddRoom("River Path", "River Path");
            AddBidirectionalExits(oRiverPath1, oGrassCoveredField4, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oRiverPath1] = new PointF(12, 5);

            Room oRiverPath2 = AddRoom("River Path", "River Path");
            AddBidirectionalExits(oRiverPath2, oRiverPath1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oRiverPath2] = new PointF(11, 5);

            Room oRiverBank = AddRoom("River Bank", "River Bank");
            AddBidirectionalExits(oRiverBank, oRiverPath2, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oRiverBank] = new PointF(10, 4);

            Room oGrassCoveredField5 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField3, oGrassCoveredField5, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField5] = new PointF(13, 7);

            Room oGrassCoveredField6 = AddRoom("Grass Field", "Grass Covered Field");
            AddBidirectionalExits(oGrassCoveredField6, oGrassCoveredField5, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oGrassCoveredField6] = new PointF(12, 7);

            Room oEdgeOfNisimaldar = AddRoom("Nisimaldar Edge", "Edge of Nisimaldar");
            AddBidirectionalExits(oGrassCoveredField6, oEdgeOfNisimaldar, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oEdgeOfNisimaldar] = new PointF(11, 8);

            Room oNisimaldar1 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar1, oEdgeOfNisimaldar, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar1] = new PointF(10, 9);

            Room oNisimaldar2 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar2, oNisimaldar1, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar2] = new PointF(9, 9);

            Room oNisimaldar3 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar3, oNisimaldar2, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oNisimaldar3] = new PointF(8, 8);

            Room oNisimaldar4 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar4, oNisimaldar3, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar4] = new PointF(7, 8);

            Room oNisimaldar5 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar4, oNisimaldar5, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oNisimaldar5] = new PointF(6, 9);

            Room oNisimaldar6 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar6, oNisimaldar5, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar6] = new PointF(5, 9);

            Room oNisimaldar7 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar6, oNisimaldar7, BidirectionalExitType.SouthwestNortheast);
            nindamosEldemondeGraph.Rooms[oNisimaldar7] = new PointF(4, 10);

            Room oNisimaldar8 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar8, oNisimaldar7, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar8] = new PointF(3, 10);

            Room oNisimaldar9 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar9, oNisimaldar8, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oNisimaldar9] = new PointF(2, 9);

            Room oNisimaldar10 = AddRoom("Nisimaldar", "Nisimaldar");
            AddBidirectionalExits(oNisimaldar10, oNisimaldar9, BidirectionalExitType.WestEast);
            nindamosEldemondeGraph.Rooms[oNisimaldar10] = new PointF(1, 9);

            oEldemondeEastGateOutside = AddRoom("East Gate Outside", "East Gate of Eldalonde");
            AddBidirectionalExits(oEldemondeEastGateOutside, oNisimaldar10, BidirectionalExitType.SoutheastNorthwest);
            nindamosEldemondeGraph.Rooms[oEldemondeEastGateOutside] = new PointF(0, 8);

            AddDeathValley(oDeathValleyEntrance);
        }

        private void AddMenelTarma(Room baseOfMenelTarma, RoomGraph eldemondeToNindamosGraph)
        {
            Room oRoad1 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(baseOfMenelTarma, oRoad1, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad1] = new PointF(15, 4.5F);

            Room oRoad2 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad2, oRoad1, BidirectionalExitType.WestEast);
            eldemondeToNindamosGraph.Rooms[oRoad2] = new PointF(14, 4.5F);

            Room oRoad3 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad2, oRoad3, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad3] = new PointF(14, 5);

            Room oRoad4 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad3, oRoad4, BidirectionalExitType.WestEast);
            eldemondeToNindamosGraph.Rooms[oRoad4] = new PointF(15, 5);

            Room oRoad5 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad4, oRoad5, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad5] = new PointF(15, 5.5F);

            Room oRoad6 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad5, oRoad6, BidirectionalExitType.WestEast);
            eldemondeToNindamosGraph.Rooms[oRoad6] = new PointF(16, 5.5F);

            Room oRoad7 = AddRoom("Road", "Road up Menel tarma");
            AddBidirectionalExits(oRoad7, oRoad6, BidirectionalExitType.NorthSouth);
            eldemondeToNindamosGraph.Rooms[oRoad7] = new PointF(16, 5.1F);

            Room oPath1 = AddRoom("Path", "Path to the summit of Menel tarma");
            AddBidirectionalExits(oPath1, oRoad7, BidirectionalExitType.UpDown);
            eldemondeToNindamosGraph.Rooms[oPath1] = new PointF(16, 4.7F);

            Room oSummit = AddRoom("Summit", "Summit of Menel Tarma");
            AddPermanentMobs(oSummit, MobTypeEnum.GoldenEagle, MobTypeEnum.GoldenEagle, MobTypeEnum.GoldenEagle);
            AddExit(oPath1, oSummit, "up");
            AddExit(oSummit, oPath1, "slope");
            eldemondeToNindamosGraph.Rooms[oSummit] = new PointF(16, 4.3F);
        }

        private void AddDeathValley(Room oDeathValleyEntrance)
        {
            RoomGraph deathValleyGraph = _graphs[MapType.DeathValley];

            deathValleyGraph.Rooms[oDeathValleyEntrance] = new PointF(6, 10);

            Room oDeathValleyWest1 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest1, oDeathValleyEntrance, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyWest1] = new PointF(5, 10);

            Room oDeathValleyWest2 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest2, oDeathValleyWest1, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyWest2] = new PointF(5, 9);

            Room oAmlug = AddRoom("Amlug", "Tomb of Amlug");
            AddPermanentMobs(oAmlug, MobTypeEnum.Amlug);
            AddBidirectionalExitsWithOut(oDeathValleyWest2, oAmlug, "tomb");
            deathValleyGraph.Rooms[oAmlug] = new PointF(5, 8);

            Room oDeathValleyWest3 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest3, oDeathValleyWest2, BidirectionalExitType.SouthwestNortheast);
            deathValleyGraph.Rooms[oDeathValleyWest3] = new PointF(6, 8);

            Room oDeathValleyWest4 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest4, oDeathValleyWest3, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyWest4] = new PointF(6, 7);

            Room oKallo = AddRoom("Kallo", "Kallo's Final Resting Place");
            AddPermanentMobs(oKallo, MobTypeEnum.Kallo);
            AddBidirectionalExitsWithOut(oDeathValleyWest4, oKallo, "tomb");
            deathValleyGraph.Rooms[oKallo] = new PointF(5, 7);

            Room oDeathValleyWest5 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest5, oDeathValleyWest4, BidirectionalExitType.SoutheastNorthwest);
            deathValleyGraph.Rooms[oDeathValleyWest5] = new PointF(5, 6);

            Room oDeathValleyWest6 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyWest6, oDeathValleyWest5, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyWest6] = new PointF(5, 5);

            Room oWizard = AddRoom("Wizard of the First Order", "Wizard's Resting Place");
            AddPermanentMobs(oWizard, MobTypeEnum.WizardOfTheFirstOrder);
            AddBidirectionalExitsWithOut(oDeathValleyWest6, oWizard, "vault");
            deathValleyGraph.Rooms[oWizard] = new PointF(6, 5);

            Room oDeathValleyWest7 = AddRoom("Death Valley", "Valley of the Dead.");
            AddBidirectionalExits(oDeathValleyWest7, oDeathValleyWest6, BidirectionalExitType.SouthwestNortheast);
            //CSRTODO: doorway
            deathValleyGraph.Rooms[oDeathValleyWest7] = new PointF(6, 4);

            Room oDeathValleyEast1 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEntrance, oDeathValleyEast1, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyEast1] = new PointF(7, 10);

            Room oDeathValleyEast2 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast2, oDeathValleyEast1, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyEast2] = new PointF(7, 9.5F);

            Room oDeathValleyEast3 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast3, oDeathValleyEast2, BidirectionalExitType.NorthSouth);
            deathValleyGraph.Rooms[oDeathValleyEast3] = new PointF(7, 9);

            Room oTranquilParkKaivo = AddHealingRoom("Kaivo", "Tranquil Park", HealingRoom.DeathValley);
            AddPermanentMobs(oTranquilParkKaivo, MobTypeEnum.Kaivo);
            AddBidirectionalExits(oTranquilParkKaivo, oDeathValleyEast3, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oTranquilParkKaivo] = new PointF(6, 9);

            Room oDeathValleyEast4 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast4, oDeathValleyEast3, BidirectionalExitType.SouthwestNortheast);
            deathValleyGraph.Rooms[oDeathValleyEast4] = new PointF(8, 7);

            Room oDeathValleyEast5 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast4, oDeathValleyEast5, BidirectionalExitType.SoutheastNorthwest);
            deathValleyGraph.Rooms[oDeathValleyEast5] = new PointF(9, 8);

            Room oDeathValleyEast6 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast6, oDeathValleyEast5, BidirectionalExitType.SouthwestNortheast);
            deathValleyGraph.Rooms[oDeathValleyEast6] = new PointF(11, 7);

            Room oDeathValleyEast7 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast7, oDeathValleyEast6, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyEast7] = new PointF(10, 7);

            Room oDeathValleyEast8 = AddRoom("Death Valley", "Valley of the Dead");
            AddBidirectionalExits(oDeathValleyEast8, oDeathValleyEast7, BidirectionalExitType.WestEast);
            deathValleyGraph.Rooms[oDeathValleyEast8] = new PointF(9, 7);

            Room oStorageRoom = AddRoom("Storage Room", "Storage Room");
            Exit e = AddBidirectionalExitsWithOut(oDeathValleyEast8, oStorageRoom, "rocky");
            e.Hidden = true;
            deathValleyGraph.Rooms[oStorageRoom] = new PointF(9, 6);
        }

        private void AddEldemondeCity(Room oEldemondeEastGateOutside)
        {
            RoomGraph eldemondeGraph = _graphs[MapType.Eldemonde];
            RoomGraph nindamosToEldemondeGraph = _graphs[MapType.NindamosToEldemonde];

            eldemondeGraph.Rooms[oEldemondeEastGateOutside] = new PointF(10, 7);

            Room oEldemondeEastGateInside = AddRoom("East Gate Inside", "Eldalondë East Gate");
            AddPermanentMobs(oEldemondeEastGateInside, MobTypeEnum.GateGuard, MobTypeEnum.GateGuard);
            AddBidirectionalSameNameExit(oEldemondeEastGateOutside, oEldemondeEastGateInside, "gate");
            eldemondeGraph.Rooms[oEldemondeEastGateInside] = new PointF(9, 7);
            nindamosToEldemondeGraph.Rooms[oEldemondeEastGateInside] = new PointF(-1, 8);
            AddMapBoundaryPoint(oEldemondeEastGateOutside, oEldemondeEastGateInside, MapType.NindamosToEldemonde, MapType.Eldemonde);

            Room oCebe1 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe1, oEldemondeEastGateInside, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCebe1] = new PointF(9, 6);

            Room oDorie1 = AddRoom("Dorie", "Dorië Avenue");
            AddBidirectionalExits(oEldemondeEastGateInside, oDorie1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oDorie1] = new PointF(9, 8);

            Room oCebe2 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe2, oCebe1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe2] = new PointF(8, 6);

            Room oElros2 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros2, oEldemondeEastGateInside, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros2] = new PointF(8, 7);

            Room oDorie2 = AddRoom("Dorie", "Dorië Avenue - The Guardstation");
            AddBidirectionalExits(oDorie2, oDorie1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie2] = new PointF(8, 8);

            Room oGuardHall = AddRoom("Guard Hall", "Guard Station Main Hall");
            AddBidirectionalExits(oGuardHall, oDorie2, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oGuardHall] = new PointF(8, 7.7F);

            Room oBarracks = AddRoom("Barracks", "Barracks");
            AddBidirectionalExits(oBarracks, oGuardHall, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oBarracks] = new PointF(7.6F, 7.4F);

            Room oGuardHQ = AddRoom("Guard HQ", "Guard Station Headquarters");
            AddBidirectionalExits(oGuardHall, oGuardHQ, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oGuardHQ] = new PointF(8.4F, 7.4F);

            Room oCebe3 = AddRoom("Cebe", "Tower of Morgatha");
            AddBidirectionalExits(oCebe3, oCebe2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe3] = new PointF(7, 6);

            Room oTower = AddRoom("Tower", "Base of Morgathas Tower");
            AddBidirectionalSameNameExit(oCebe3, oTower, "door");
            eldemondeGraph.Rooms[oTower] = new PointF(7, 5.5F);

            Room oElementsChamber = AddRoom("Elements Chamber", "Chamber of Elements");
            AddBidirectionalExits(oElementsChamber, oTower, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oElementsChamber] = new PointF(8, 5.5F);

            Room oGolemsChamber = AddRoom("Golems Chamber", "Chamber of Golems");
            AddBidirectionalExits(oGolemsChamber, oElementsChamber, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oGolemsChamber] = new PointF(9, 5.5F);

            Room oMorgatha = AddRoom("Morgatha", "Morgatha's Chamber");
            AddPermanentMobs(oMorgatha, MobTypeEnum.MorgathaTheEnchantress);
            AddBidirectionalExits(oMorgatha, oGolemsChamber, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oMorgatha] = new PointF(10, 5.5F);

            Room oElros3 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros3, oElros2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros3] = new PointF(7, 7);

            Room oDorie3 = AddRoom("Dorie", "Dorië Avenue");
            AddBidirectionalExits(oDorie3, oDorie2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie3] = new PointF(7, 8);

            Room oPostOffice = AddRoom("Post Office", "The Eldalonde Post Office");
            AddBidirectionalExits(oPostOffice, oDorie3, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oPostOffice] = new PointF(7, 7.5F);

            Room oIothCandol = AddRoom("Ioth/Candol", "Ioth Road / Candol Street");
            eldemondeGraph.Rooms[oIothCandol] = new PointF(6, 4);

            Room oCandol1 = AddRoom("Candol", "Candol Street");
            AddBidirectionalExits(oIothCandol, oCandol1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCandol1] = new PointF(6, 5);

            Room oUniversityHall = AddRoom("University Hall", "Main Hall");
            AddBidirectionalExits(oCandol1, oUniversityHall, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oUniversityHall] = new PointF(7, 4);

            Room oUniversityHallSouth = AddRoom("University Hall", "South Hall");
            AddBidirectionalExits(oUniversityHall, oUniversityHallSouth, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oUniversityHallSouth] = new PointF(7, 4.5F);

            Room oUniversityHallSE = AddRoom("University Hall", "South Hall");
            AddBidirectionalExits(oUniversityHallSouth, oUniversityHallSE, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oUniversityHallSE] = new PointF(8, 4.5F);

            Room oAlchemy = AddRoom("Alchemy", "Alchemy");
            AddBidirectionalSameNameExit(oUniversityHallSE, oAlchemy, "door");
            eldemondeGraph.Rooms[oAlchemy] = new PointF(8, 5);

            Room oAurelius = AddRoom("Aurelius", "Mysticism");
            AddPermanentMobs(oAurelius, MobTypeEnum.AureliusTheScholar);
            AddBidirectionalSameNameExit(oUniversityHallSouth, oAurelius, "door");
            eldemondeGraph.Rooms[oAurelius] = new PointF(7, 5);

            Room oUniversityHallNorth = AddRoom("University Hall", "North Hall");
            AddBidirectionalExits(oUniversityHallNorth, oUniversityHall, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oUniversityHallNorth] = new PointF(7, 4);

            Room oMathemathics = AddRoom("Mathemathics", "Mathemathics");
            AddBidirectionalExitsWithOut(oUniversityHallNorth, oMathemathics, "door");
            eldemondeGraph.Rooms[oMathemathics] = new PointF(8, 4);

            Room oCebeCandol = AddRoom("Cebe/Candol", "Cebe Avenue / Candol Street");
            AddBidirectionalExits(oCebeCandol, oCebe3, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oCandol1, oCebeCandol, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oCebeCandol] = new PointF(6, 6);

            Room oElrosCandol = AddRoom("Elros/Candol", "Elros Boulevard / Candol Street");
            AddBidirectionalExits(oCebeCandol, oElrosCandol, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElrosCandol, oElros3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElrosCandol] = new PointF(6, 7);

            Room oDorieCandol = AddRoom("Dorie/Candol", "Dorië Avenue / Candol Street");
            AddBidirectionalExits(oElrosCandol, oDorieCandol, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDorieCandol, oDorie3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorieCandol] = new PointF(6, 8);

            Room oIoth1 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth1, oIothCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth1] = new PointF(5, 4);

            Room oCebe4 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe4, oCebeCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe4] = new PointF(5, 6);

            Room oIsildur = AddRoom("Isildur", "Isildur's Bows");
            AddPermanentMobs(oIsildur, MobTypeEnum.Isildur);
            AddBidirectionalExitsWithOut(oCebe4, oIsildur, "shop");
            eldemondeGraph.Rooms[oIsildur] = new PointF(5, 5.5F);

            Room oElros4 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros4, oElrosCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros4] = new PointF(5, 7);

            Room oGate = AddRoom("Palace Gate", "The Palace of Eldalondë");
            AddBidirectionalSameNameExit(oElros4, oGate, "gate");
            eldemondeGraph.Rooms[oGate] = new PointF(4.6F, 6.5F);

            Room oPalaceSouth = AddRoom("Palace", "The Palace of Eldalondë");
            AddBidirectionalSameNameExit(oGate, oPalaceSouth, "stairway");
            eldemondeGraph.Rooms[oPalaceSouth] = new PointF(4.6F, 3F);

            Room oPalaceSouthwest = AddRoom("Palace", "West Wing");
            AddBidirectionalExits(oPalaceSouthwest, oPalaceSouth, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oPalaceSouthwest] = new PointF(3.6F, 3);

            Room oFaeldor = AddRoom("Faeldor", "Elven Embassy");
            AddPermanentMobs(oFaeldor, MobTypeEnum.Faeldor);
            AddBidirectionalSameNameExit(oPalaceSouthwest, oFaeldor, "door");
            eldemondeGraph.Rooms[oFaeldor] = new PointF(2.6F, 3);

            Room oPalaceSoutheast = AddRoom("Palace", "East Wing");
            AddBidirectionalExits(oPalaceSouth, oPalaceSoutheast, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oPalaceSoutheast] = new PointF(5.6F, 3);

            Room oGrimaxeGrimson = AddRoom("Grimaxe Grimson", "Dwarven Embassy");
            AddPermanentMobs(oGrimaxeGrimson, MobTypeEnum.GrimaxeGrimson);
            AddBidirectionalSameNameExit(oPalaceSoutheast, oGrimaxeGrimson, "door");
            eldemondeGraph.Rooms[oGrimaxeGrimson] = new PointF(6.6F, 3);

            Room oMirrorHallEast = AddRoom("Mirror Hall", "Mirror Hall");
            AddBidirectionalExits(oMirrorHallEast, oPalaceSoutheast, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallEast] = new PointF(5.6F, 2);

            Room oMirrorHallCenter = AddRoom("Mirror Hall", "Mirror Hall");
            AddBidirectionalExits(oMirrorHallCenter, oMirrorHallEast, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMirrorHallCenter, oPalaceSouth, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallCenter] = new PointF(4.6F, 2);

            Room oMirrorHallWest = AddRoom("Mirror Hall", "Mirror Hall");
            AddBidirectionalExits(oMirrorHallWest, oMirrorHallCenter, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oMirrorHallWest, oPalaceSouthwest, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oMirrorHallWest] = new PointF(3.6F, 2);

            Room oThroneHallWest = AddRoom("Throne Hall", "Throne hall");
            AddExit(oMirrorHallWest, oThroneHallWest, "hall");
            AddExit(oThroneHallWest, oMirrorHallWest, "south");
            eldemondeGraph.Rooms[oThroneHallWest] = new PointF(3.6F, 1);

            Room oThroneHallCenter = AddRoom("Throne Hall", "Throne Hall");
            AddBidirectionalExits(oThroneHallWest, oThroneHallCenter, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oThroneHallCenter] = new PointF(4.6F, 1);

            Room oThroneHallEast = AddRoom("Throne Hall", "Throne Hall");
            AddBidirectionalExits(oThroneHallCenter, oThroneHallEast, BidirectionalExitType.WestEast);
            AddExit(oMirrorHallEast, oThroneHallEast, "hall");
            AddExit(oThroneHallEast, oMirrorHallEast, "south");
            eldemondeGraph.Rooms[oThroneHallEast] = new PointF(5.6F, 1);

            Room oThroneHall = AddRoom("Throne Hall", "Throne Hall");
            AddBidirectionalSameNameExit(oThroneHallCenter, oThroneHall, "stairs");
            eldemondeGraph.Rooms[oThroneHall] = new PointF(4.6F, 0);

            Room oDorie4 = AddRoom("Dori", "Tower of Yotha");
            AddBidirectionalExits(oDorie4, oDorieCandol, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie4] = new PointF(5, 8);

            Room oYothasTower = AddRoom("Yotha Tower", "Yotha's Tower");
            AddBidirectionalSameNameExit(oDorie4, oYothasTower, "door");
            eldemondeGraph.Rooms[oYothasTower] = new PointF(5, 7.8F);

            Room oChamberOfEnergy = AddRoom("Chamber of Energy", "Chamber of Energy");
            AddBidirectionalExits(oChamberOfEnergy, oYothasTower, BidirectionalExitType.UpDown);
            eldemondeGraph.Rooms[oChamberOfEnergy] = new PointF(5, 7.6F);

            Room oChamberOfIllusions = AddRoom("Chamber of Illusions", "Chamber of Illusions");
            AddBidirectionalExits(oChamberOfIllusions, oChamberOfEnergy, BidirectionalExitType.UpDown);
            AddExit(oChamberOfIllusions, oChamberOfEnergy, "up");
            eldemondeGraph.Rooms[oChamberOfIllusions] = new PointF(5, 7.4F);

            Room oYothasChamber = AddRoom("Yotha's Chamber", "Yotha's Chamber");
            Exit e = AddExit(oChamberOfIllusions, oYothasChamber, "wall");
            e.Hidden = true;
            AddExit(oYothasChamber, oChamberOfIllusions, "wall");
            eldemondeGraph.Rooms[oYothasChamber] = new PointF(5, 7.2F);

            Room oIoth2 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth2, oIoth1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth2] = new PointF(4, 4);

            Room oCebe5 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe5, oCebe4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe5] = new PointF(4, 6);

            Room oSssreth = AddRoom("Sssreth", "Sssreth's Potion Shop");
            AddPermanentMobs(oSssreth, MobTypeEnum.SssrethTheLizardman);
            AddBidirectionalExitsWithOut(oCebe5, oSssreth, "south");
            eldemondeGraph.Rooms[oSssreth] = new PointF(4, 6.5F);

            Room oElros5 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros5, oElros4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros5] = new PointF(4, 7);

            Room oDorie5 = AddRoom("Elros Statue", "Statue of Elros");
            AddBidirectionalExits(oDorie5, oDorie4, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDorie5] = new PointF(4, 8);

            Room oIothNundine = AddRoom("North Gate Inside", "Eldalondë, North Gate");
            AddBidirectionalExits(oIothNundine, oIoth2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIothNundine] = new PointF(3, 4);

            Room oNundine1 = AddRoom("Nundine", "Nundine Street");
            AddBidirectionalExits(oIothNundine, oNundine1, BidirectionalExitType.NorthSouth);
            eldemondeGraph.Rooms[oNundine1] = new PointF(3, 5);

            Room oKegTavern = AddRoom("Keg Tavern", "The Keg Tavern");
            AddBidirectionalExitsWithOut(oNundine1, oKegTavern, "west");
            eldemondeGraph.Rooms[oKegTavern] = new PointF(2, 5.3F);

            Room oTavernKitchen = AddRoom("Tavern Kitchen", "Tavern Kitchen");
            e = AddExit(oKegTavern, oTavernKitchen, "door");
            e.Hidden = true;
            AddExit(oTavernKitchen, oKegTavern, "door");
            eldemondeGraph.Rooms[oTavernKitchen] = new PointF(2, 4.7F);

            Room oCebeNundine = AddRoom("Cebe/Nundine", "Cebe Avenue / Nundine Street");
            AddBidirectionalExits(oNundine1, oCebeNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCebeNundine, oCebe5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebeNundine] = new PointF(3, 6);

            Room oElrosNundine = AddRoom("Elros/Nundine", "Elros Boulevard / Nundine Street");
            AddBidirectionalExits(oCebeNundine, oElrosNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oElrosNundine, oElros5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElrosNundine] = new PointF(3, 7);

            Room oDoriNundine = AddRoom("Dori/Nundine", "Dorië Avenue / Nundine Street");
            AddBidirectionalExits(oElrosNundine, oDoriNundine, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oDoriNundine, oDorie5, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDoriNundine] = new PointF(3, 8);

            Room oIoth3 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth3, oIothNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth3] = new PointF(2, 4);

            Room oCebe6 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oCebe6, oCebeNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe6] = new PointF(2, 6);

            Room oElros6 = AddRoom("Wish Fountain", "The Fountain of Wishes");
            AddBidirectionalExits(oElros6, oElrosNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros6] = new PointF(2, 7);

            Room oDori6 = AddRoom("Dark Lord Taunting", "The Taunting of The Dark Lord");
            AddBidirectionalExits(oDori6, oDoriNundine, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori6] = new PointF(2, 8);

            Room oIoth4 = AddRoom("Ioth", "Ioth Road");
            AddBidirectionalExits(oIoth4, oIoth3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oIoth4] = new PointF(1, 4);

            Room oSmallPath = AddRoom("Small Path", "Small path");
            e = AddExit(oIoth4, oSmallPath, "south");
            e.Hidden = true;
            e = AddExit(oSmallPath, oTavernKitchen, "backdoor");
            e.Hidden = true;
            AddExit(oSmallPath, oIoth4, "north");
            eldemondeGraph.Rooms[oSmallPath] = new PointF(1, 5);

            Room oCebe7 = AddRoom("Cebe", "Cebe Avenue");
            AddBidirectionalExits(oSmallPath, oCebe7, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oCebe7, oCebe6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCebe7] = new PointF(1, 6);

            Room oBezanthi = AddRoom("Bezanthi", "Bezanthi's Trading Post");
            AddPermanentMobs(oBezanthi, MobTypeEnum.Bezanthi);
            AddBidirectionalExitsWithOut(oCebe7, oBezanthi, "shop");
            eldemondeGraph.Rooms[oBezanthi] = new PointF(0, 6);

            Room oElros7 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oElros7, oElros6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oElros7] = new PointF(1, 7);

            Room oDori7 = AddRoom("Dori", "Dorië Avenue");
            AddBidirectionalExits(oDori7, oDori6, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oDori7] = new PointF(1, 8);

            Room oElros8 = AddRoom("Elros", "Elros Boulevard");
            AddBidirectionalExits(oCebe7, oElros8, BidirectionalExitType.SouthwestNortheast);
            AddBidirectionalExits(oElros8, oElros7, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oElros8, oDori7, BidirectionalExitType.SoutheastNorthwest);
            eldemondeGraph.Rooms[oElros8] = new PointF(0, 7);

            Room oCityWalkway1 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oDorie1, oCityWalkway1, BidirectionalExitType.SouthwestNortheast);
            eldemondeGraph.Rooms[oCityWalkway1] = new PointF(8, 9);

            Room oCityWalkway2 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oCityWalkway2, oCityWalkway1, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway2] = new PointF(6, 9);

            Room oCityWalkway3 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oCityWalkway3, oCityWalkway2, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway3] = new PointF(4, 9);

            Room oCityWalkway4 = AddRoom("City Walkway", "City Walkway");
            AddBidirectionalExits(oDori7, oCityWalkway4, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oCityWalkway4, oCityWalkway3, BidirectionalExitType.WestEast);
            eldemondeGraph.Rooms[oCityWalkway4] = new PointF(2, 9);
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

        public void AddPermanentMobs(Room r, params MobTypeEnum[] mobs)
        {
            if (r.PermanentMobs == null) r.PermanentMobs = new List<MobTypeEnum>();
            HashSet<MobTypeEnum> processedMobs = new HashSet<MobTypeEnum>();
            foreach (MobTypeEnum nextMob in mobs)
            {
                r.PermanentMobs.Add(nextMob);
                if (!processedMobs.Contains(nextMob))
                {
                    bool exists = MobRooms.TryGetValue(nextMob, out _);
                    MobRooms[nextMob] = exists ? null : r;
                    processedMobs.Add(nextMob);
                }
            }
        }

        public void AddNonPermanentMobs(Room r, params MobTypeEnum[] mobs)
        {
            if (r.NonPermanentMobs == null) r.NonPermanentMobs = new List<MobTypeEnum>();
            foreach (MobTypeEnum nextMob in mobs)
            {
                r.NonPermanentMobs.Add(nextMob);
            }
        }

        public void AddPermanentItems(Room r, params ItemTypeEnum[] items)
        {
            if (r.PermanentItems == null) r.PermanentItems = new List<ItemTypeEnum>();
            foreach (ItemTypeEnum nextItem in items)
            {
                r.PermanentItems.Add(nextItem);
            }
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
            AddBidirectionalSameNameExit(aRoom, bRoom, exitText, null);
        }

        private void AddBidirectionalSameNameMustOpenExit(Room aRoom, Room bRoom, string exitText)
        {
            AddBidirectionalSameNameExit(aRoom, bRoom, exitText, (e) => { e.MustOpen = true; });
        }

        private void AddBidirectionalSameNameHiddenExit(Room aRoom, Room bRoom, string exitText)
        {
            AddBidirectionalSameNameExit(aRoom, bRoom, exitText, (e) => { e.Hidden = true; });
        }

        private void AddBidirectionalSameNameExit(Room aRoom, Room bRoom, string exitText, Action<Exit> actionToApplyToBoth)
        {
            Exit e1 = new Exit(aRoom, bRoom, exitText);
            Exit e2 = new Exit(bRoom, aRoom, exitText);
            if (actionToApplyToBoth != null)
            {
                actionToApplyToBoth(e1);
                actionToApplyToBoth(e2);
            }
            AddExit(e1);
            AddExit(e2);
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

    public static class MapComputation
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

    public class GraphInputs
    {
        public GraphInputs(ClassType Class, int Level, bool IsDay, bool Flying, bool Levitating, SupportedKeysFlags Keys)
        {
            this.Class = Class;
            this.Level = Level;
            this.IsDay = IsDay;
            this.Flying = Flying;
            this.Levitating = Levitating;
            this.Keys = Keys;
        }
        public bool Flying { get; set; }
        public bool Levitating { get; set; }
        public bool IsDay { get; set; }
        public int Level { get; set; }
        public ClassType Class { get; set; }
        public SupportedKeysFlags Keys { get; set; }
    }
}
