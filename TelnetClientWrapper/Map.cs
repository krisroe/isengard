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

        private RoomGraph _breeStreetsGraph;

        private Room _breeEastGateInside = null;
        private Room _breeEastGateOutside = null;
        private Room _imladrisWestGateInside = null;
        private Room _imladrisWestGateOutside = null;
        private Room _breeDocks = null;
        private Room _boatswain = null;
        private Room _treeOfLife = null;
        private Area _aBreePerms;
        private Area _aImladrisTharbadPerms;
        private Area _aShips;
        private Area _aMisc;
        private Area _aInaccessible;

        private List<Exit> _nightEdges = new List<Exit>();
        private List<Exit> _celduinExpressEdges = new List<Exit>();

        private const string AREA_BREE_PERMS = "Bree Perms";
        private const string AREA_IMLADRIS_THARBAD_PERMS = "Imladris/Tharbad Perms";
        private const string AREA_MISC = "Misc";
        private const string AREA_SHIPS = "Ships";
        private const string AREA_INTANGIBLE = "Intangible";
        private const string AREA_INACCESSIBLE = "Inaccessible";

        public IsengardMap(AlignmentType preferredAlignment, int level)
        {
            _graphs = new Dictionary<MapType, RoomGraph>();
            _map = new AdjacencyGraph<Room, Exit>();
            _areas = new List<Area>();
            _areasByName = new Dictionary<string, Area>();

            _aBreePerms = AddArea(AREA_BREE_PERMS);
            _aImladrisTharbadPerms = AddArea(AREA_IMLADRIS_THARBAD_PERMS);
            _aMisc = AddArea(AREA_MISC);
            _aShips = AddArea(AREA_SHIPS);
            AddArea(AREA_INTANGIBLE);
            _aInaccessible = AddArea(AREA_INACCESSIBLE);

            foreach (Area a in _areas)
            {
                a.Locations.Clear();
            }

            RoomGraph graphMillwoodMansion = new RoomGraph("Millwood Mansion");
            graphMillwoodMansion.ScalingFactor = 100;
            _graphs[MapType.MillwoodMansion] = graphMillwoodMansion;

            AddBreeCity(out Room oIxell, out Room oBreeTownSquare, out Room oBreeWestGateInside, out Room oSmoulderingVillage, graphMillwoodMansion, preferredAlignment, level, out Room oDroolie, out Room oSewerPipeExit);
            AddMayorMillwoodMansion(oIxell);
            AddBreeToHobbiton(oBreeWestGateInside, oSmoulderingVillage, level);
            AddBreeToImladris(level, out Room oOuthouse);
            AddUnderBree(oDroolie, oOuthouse, oSewerPipeExit);
            AddImladrisCity(out Room oImladrisSouthGateInside);
            AddImladrisToTharbad(oImladrisSouthGateInside, out Room oTharbadGateOutside);
            AddTharbadCity(oTharbadGateOutside);
            AddIntangible(oBreeTownSquare);

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
        
        public Room TreeOfLifeRoom
        {
            get
            {
                return _treeOfLife;
            }
        }

        public Dictionary<MapType, RoomGraph> Graphs
        {
            get
            {
                return _graphs;
            }
        }

        private void AddTharbadCity(Room oTharbadGateOutside)
        {
            RoomGraph tharbadGraph = new RoomGraph("Tharbad");
            tharbadGraph.ScalingFactor = 100;
            _graphs[MapType.Tharbad] = tharbadGraph;

            tharbadGraph.Rooms[oTharbadGateOutside] = new System.Windows.Point(3, 0);

            Room balleNightingale = AddRoom("Balle/Nightingale");
            Room balle1 = AddRoom("Balle");
            AddBidirectionalExits(balleNightingale, balle1, BidirectionalExitType.WestEast);
            AddBidirectionalSameNameExit(oTharbadGateOutside, balleNightingale, "gate");
            tharbadGraph.Rooms[balleNightingale] = new System.Windows.Point(3, 1);
            tharbadGraph.Rooms[balle1] = new System.Windows.Point(4, 1);

            Room balleIllusion = AddRoom("Balle/Illusion");
            AddBidirectionalExits(balle1, balleIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balleIllusion] = new System.Windows.Point(5, 1);

            Room balle2 = AddRoom("Balle");
            AddBidirectionalExits(balleIllusion, balle2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balle2] = new System.Windows.Point(8, 1);

            Room balleEvard = AddRoom("Balle/Evard");
            AddBidirectionalExits(balle2, balleEvard, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[balleEvard] = new System.Windows.Point(10, 1);

            Room evardMemorialTree = AddRoom("Evard Memorial Tree");
            Exit e = AddExit(balleEvard, evardMemorialTree, "tree");
            e.Hidden = true;
            AddExit(evardMemorialTree, balleEvard, "down");
            tharbadGraph.Rooms[evardMemorialTree] = new System.Windows.Point(10, 0);

            Room evard1 = AddRoom("Evard");
            AddBidirectionalExits(balleEvard, evard1, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[evard1] = new System.Windows.Point(10, 3);

            Room evard2 = AddRoom("Evard");
            AddBidirectionalExits(evard1, evard2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[evard2] = new System.Windows.Point(10, 5);

            Room sabreEvard = AddRoom("Sabre/Evard");
            AddBidirectionalExits(evard2, sabreEvard, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[sabreEvard] = new System.Windows.Point(10, 8);

            Room sabre1 = AddRoom("Sabre");
            AddBidirectionalExits(sabre1, sabreEvard, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre1] = new System.Windows.Point(8, 8);

            Room sabreIllusion = AddRoom("Sabre/Illusion");
            AddBidirectionalExits(sabreIllusion, sabre1, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabreIllusion] = new System.Windows.Point(5, 8);

            Room sabre2 = AddRoom("Sabre");
            AddBidirectionalExits(sabre2, sabreIllusion, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre2] = new System.Windows.Point(4, 8);

            Room sabreNightingale = AddRoom("Sabre/Nightingale");
            AddBidirectionalExits(sabreNightingale, sabre2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabreNightingale] = new System.Windows.Point(3, 8);

            Room nightingale1 = AddRoom("Nightingale");
            AddBidirectionalExits(nightingale1, sabreNightingale, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale1] = new System.Windows.Point(3, 5);

            Room efrimsJuiceCafe = AddRoom("Efrim's Juice Cafe");
            AddBidirectionalSameNameExit(nightingale1, efrimsJuiceCafe, "door");
            tharbadGraph.Rooms[efrimsJuiceCafe] = new System.Windows.Point(4, 5);

            Room nightingale2 = AddRoom("Nightingale");
            AddBidirectionalExits(nightingale2, nightingale1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(balleNightingale, nightingale2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale2] = new System.Windows.Point(3, 3);

            Room jewelryShop = AddRoom("Jewelry Shop");
            AddBidirectionalExits(nightingale2, jewelryShop, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[jewelryShop] = new System.Windows.Point(4, 3);

            Room nightingale3 = AddRoom("Nightingale");
            AddBidirectionalExits(sabreNightingale, nightingale3, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[nightingale3] = new System.Windows.Point(3, 9);

            Room illusion2 = AddRoom("Illusion");
            AddBidirectionalExits(balleIllusion, illusion2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[illusion2] = new System.Windows.Point(5, 3);

            Room marketBeast = AddRoom("Market Beast");
            AddBidirectionalExits(illusion2, marketBeast, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketBeast] = new System.Windows.Point(5, 5);

            Room bardicGuildhall = AddRoom("Bardic Guildhall");
            bardicGuildhall.IsHealingRoom = true;
            AddBidirectionalExits(bardicGuildhall, nightingale3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[bardicGuildhall] = new System.Windows.Point(2, 9);

            Room oGuildmasterAnsette = AddRoom("Ansette");
            oGuildmasterAnsette.Mob1 = "Ansette";
            oGuildmasterAnsette.Experience1 = 1200;
            e = AddExit(bardicGuildhall, oGuildmasterAnsette, "door");
            e.Hidden = true;
            AddExit(oGuildmasterAnsette, bardicGuildhall, "out");
            tharbadGraph.Rooms[oGuildmasterAnsette] = new System.Windows.Point(2, 8.5);

            Room sabre3 = AddRoom("Sabre");
            AddBidirectionalExits(sabre3, sabreNightingale, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre3] = new System.Windows.Point(2, 8);

            Room sabre4 = AddRoom("Sabre");
            AddBidirectionalExits(sabre4, sabre3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[sabre4] = new System.Windows.Point(1, 8);

            Room tourismAndCustoms = AddRoom("Tourism and Customs");
            AddBidirectionalExits(tourismAndCustoms, sabre4, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[tourismAndCustoms] = new System.Windows.Point(1, 7);

            Room tharbadWestGateInside = AddRoom("West Gate Inside");
            AddBidirectionalExits(tharbadWestGateInside, sabre4, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[tharbadWestGateInside] = new System.Windows.Point(0, 8);

            Room tharbadWestGateOutside = AddRoom("West Gate Outside");
            AddBidirectionalSameNameExit(tharbadWestGateInside, tharbadWestGateOutside, "gate");
            tharbadGraph.Rooms[tharbadWestGateOutside] = new System.Windows.Point(-1, 8);

            Room tharbadDocks = AddRoom("Docks");
            AddBidirectionalExits(tharbadWestGateOutside, tharbadDocks, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[tharbadDocks] = new System.Windows.Point(-1, 9);

            Room illusion1 = AddRoom("Illusion");
            AddBidirectionalExits(illusion1, sabreIllusion, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[illusion1] = new System.Windows.Point(5, 7);

            Room marketDistrictClothiers = AddRoom("Market Clothiers");
            AddBidirectionalExits(marketDistrictClothiers, illusion1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(marketBeast, marketDistrictClothiers, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketDistrictClothiers] = new System.Windows.Point(5, 6);

            Room oMasterJeweler = AddRoom("Jeweler");
            oMasterJeweler.Mob1 = "Jeweler";
            oMasterJeweler.Experience1 = 170;
            oMasterJeweler.Alignment = AlignmentType.Red;
            AddBidirectionalExits(marketDistrictClothiers, oMasterJeweler, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oMasterJeweler] = new System.Windows.Point(6, 6);

            Room marketTrinkets = AddRoom("Market Trinkets");
            AddBidirectionalExits(marketBeast, marketTrinkets, BidirectionalExitType.WestEast);
            AddBidirectionalExits(marketTrinkets, oMasterJeweler, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[marketTrinkets] = new System.Windows.Point(6, 5);

            Room oEntranceToGypsyEncampment = AddRoom("Gypsy Encampment");
            AddExit(oMasterJeweler, oEntranceToGypsyEncampment, "row");
            AddExit(oEntranceToGypsyEncampment, oMasterJeweler, "market");
            tharbadGraph.Rooms[oEntranceToGypsyEncampment] = new System.Windows.Point(7, 6);

            Room oGypsyRow1 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oEntranceToGypsyEncampment, oGypsyRow1, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oGypsyRow1] = new System.Windows.Point(8, 6);

            Room oGypsyRow2 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oGypsyRow1, oGypsyRow2, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oGypsyRow2] = new System.Windows.Point(9, 6);

            Room oGypsyRow3 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oGypsyRow3, oGypsyRow2, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oGypsyRow3] = new System.Windows.Point(9, 5);

            Room oGypsyRow4 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oGypsyRow4, oGypsyRow3, BidirectionalExitType.WestEast);
            tharbadGraph.Rooms[oGypsyRow4] = new System.Windows.Point(8, 5);

            Room oKingBrundensWagon = AddRoom("King Wagon");
            AddExit(oGypsyRow4, oKingBrundensWagon, "wagon");
            AddExit(oKingBrundensWagon, oGypsyRow4, "out");
            tharbadGraph.Rooms[oKingBrundensWagon] = new System.Windows.Point(8, 4);

            Room oKingBrunden = AddRoom("King Brunden");
            oKingBrunden.Mob1 = "king";
            oKingBrunden.Experience1 = 300;
            AddExit(oKingBrundensWagon, oKingBrunden, "back");
            AddExit(oKingBrunden, oKingBrundensWagon, "out");
            tharbadGraph.Rooms[oKingBrunden] = new System.Windows.Point(8, 3);

            Room oGypsyBlademaster = AddRoom("Blademaster");
            oGypsyBlademaster.Mob1 = "Blademaster";
            oGypsyBlademaster.Experience1 = 160;
            oGypsyBlademaster.Alignment = AlignmentType.Blue;
            AddExit(oGypsyRow3, oGypsyBlademaster, "tent");
            AddExit(oGypsyBlademaster, oGypsyRow3, "out");
            tharbadGraph.Rooms[oGypsyBlademaster] = new System.Windows.Point(9, 4);

            Room oKingsMoneychanger = AddRoom("Moneychanger");
            oKingsMoneychanger.Mob1 = "Moneychanger";
            oKingsMoneychanger.Experience1 = 150;
            oKingsMoneychanger.Alignment = AlignmentType.Red;
            AddExit(oGypsyRow2, oKingsMoneychanger, "tent");
            AddExit(oKingsMoneychanger, oGypsyRow2, "out");
            tharbadGraph.Rooms[oKingsMoneychanger] = new System.Windows.Point(9, 7);

            Room oMadameNicolov = AddRoom("Nicolov");
            oMadameNicolov.Mob1 = "Madame";
            oMadameNicolov.Experience1 = 180;
            oMadameNicolov.Alignment = AlignmentType.Blue;
            AddExit(oGypsyRow1, oMadameNicolov, "wagon");
            AddExit(oMadameNicolov, oGypsyRow1, "out");
            tharbadGraph.Rooms[oMadameNicolov] = new System.Windows.Point(8, 7);

            Room gildedApple = AddRoom("Gilded Apple");
            AddBidirectionalSameNameExit(sabre3, gildedApple, "door");
            tharbadGraph.Rooms[gildedApple] = new System.Windows.Point(2, 7.5);

            Room zathriel = AddRoom("Zathriel the Minstrel");
            zathriel.Mob1 = "Minstrel";
            zathriel.Experience1 = 220;
            zathriel.Alignment = AlignmentType.Blue;
            e = AddExit(gildedApple, zathriel, "stage");
            e.Hidden = true;
            AddExit(zathriel, gildedApple, "down");
            tharbadGraph.Rooms[zathriel] = new System.Windows.Point(2, 7);

            Room oOliphauntsTattoos = AddRoom("Oliphaunt's Tattoos");
            AddBidirectionalExits(balle2, oOliphauntsTattoos, BidirectionalExitType.NorthSouth);
            tharbadGraph.Rooms[oOliphauntsTattoos] = new System.Windows.Point(8, 1.5);

            Room oOliphaunt = AddRoom("Oliphaunt");
            oOliphaunt.Mob1 = "Oliphaunt";
            oOliphaunt.Experience1 = 310;
            oOliphaunt.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oOliphauntsTattoos, oOliphaunt, "curtain");
            tharbadGraph.Rooms[oOliphaunt] = new System.Windows.Point(8, 2);

            AddLocation(_aImladrisTharbadPerms, bardicGuildhall);
            AddLocation(_aImladrisTharbadPerms, oGuildmasterAnsette);
            AddLocation(_aImladrisTharbadPerms, zathriel);
            AddLocation(_aImladrisTharbadPerms, oOliphaunt);
            AddLocation(_aImladrisTharbadPerms, oMasterJeweler);
            AddLocation(_aImladrisTharbadPerms, oMadameNicolov);
            AddLocation(_aImladrisTharbadPerms, oKingsMoneychanger);
            AddLocation(_aImladrisTharbadPerms, oGypsyBlademaster);
            AddLocation(_aImladrisTharbadPerms, oKingBrunden);
        }

        private void AddBreeCity(out Room oIxell, out Room oBreeTownSquare, out Room oWestGateInside, out Room oSmoulderingVillage, RoomGraph graphMillwoodMansion, AlignmentType preferredAlignment, int level, out Room oDroolie, out Room oSewerPipeExit)
        {
            _breeStreetsGraph = new RoomGraph("Bree Streets");
            _breeStreetsGraph.ScalingFactor = 100;
            _graphs[MapType.BreeStreets] = _breeStreetsGraph;

            //Bree's road structure is a 15x11 grid
            Room[,] breeStreets = new Room[16, 11];
            Room[,] breeSewers = new Room[16, 11];
            breeStreets[0, 0] = AddRoom("Thalion/Wain"); //1x1
            breeStreets[1, 0] = AddRoom("Thalion"); //2x1
            breeStreets[2, 0] = AddRoom("Thalion"); //3x1
            breeStreets[3, 0] = AddRoom("Thalion/High"); //4x1
            breeStreets[4, 0] = AddRoom("Thalion"); //5x1
            breeStreets[5, 0] = AddRoom("Thalion"); //6x1
            breeStreets[6, 0] = AddRoom("Thalion"); //7x1
            breeStreets[7, 0] = AddRoom("Thalion/Main"); //8x1
            _breeDocks = breeStreets[9, 0] = AddRoom("Docks"); //10x1
            oSewerPipeExit = breeStreets[10, 0] = AddRoom("Thalion/Crissaegrim"); //11x1
            breeStreets[11, 0] = AddRoom("Thalion"); //12x1
            breeStreets[12, 0] = AddRoom("Thalion"); //13x1
            breeStreets[13, 0] = AddRoom("Thalion"); //14x1
            breeStreets[14, 0] = AddRoom("Thalion/Brownhaven"); //15x1
            breeStreets[0, 1] = AddRoom("Wain"); //1x2
            Room oToCampusFreeClinic = breeStreets[3, 1] = AddRoom("High"); //4x2
            breeStreets[7, 1] = AddRoom("Main"); //8x2
            breeStreets[10, 1] = AddRoom("Crissaegrim"); //11x2
            breeStreets[14, 1] = AddRoom("Brownhaven"); //15x2
            breeStreets[0, 2] = AddRoom("Wain"); //1x3
            Room oToPawnShopWest = breeStreets[3, 2] = AddRoom("High"); //4x3
            Room oToBarracks = breeStreets[7, 2] = AddRoom("Main"); //8x3
            breeStreets[10, 2] = AddRoom("Crissaegrim"); //11x3
            breeStreets[14, 2] = AddRoom("Brownhaven"); //15x3
            breeStreets[0, 3] = AddRoom("Periwinkle/Wain"); //1x4
            breeSewers[0, 3] = AddRoom("Sewers Periwinkle/Wain"); //1x4
            AddExit(breeSewers[0, 3], breeStreets[0, 3], "up");
            breeStreets[1, 3] = AddRoom("Periwinkle"); //2x4
            breeSewers[1, 3] = AddRoom("Sewers Periwinkle"); //2x4
            breeStreets[2, 3] = AddRoom("Periwinkle"); //3x4
            breeSewers[2, 3] = AddRoom("Sewers Periwinkle"); //3x4
            breeStreets[3, 3] = AddRoom("Periwinkle/High"); //4x4
            breeSewers[3, 3] = AddRoom("Sewers Periwinkle/High"); //4x4
            AddExit(breeSewers[3, 3], breeStreets[3, 3], "up");
            breeStreets[4, 3] = AddRoom("Periwinkle"); //5x4
            breeSewers[4, 3] = AddRoom("Sewers Periwinkle"); //5x4
            breeStreets[5, 3] = AddRoom("Periwinkle"); //6x4
            breeSewers[5, 3] = AddRoom("Sewers Periwinkle"); //6x4
            breeStreets[6, 3] = AddRoom("Periwinkle"); //7x4
            breeSewers[6, 3] = AddRoom("Sewers Periwinkle"); //7x4
            breeStreets[7, 3] = AddRoom("Periwinkle/Main"); //8x4
            breeSewers[7, 3] = AddRoom("Shirriffs"); //Bree Sewers Periwinkle/Main 8x4
            AddExit(breeSewers[7, 3], breeStreets[7, 3], "up");
            breeStreets[8, 3] = AddRoom("Periwinkle"); //9x4
            breeStreets[9, 3] = AddRoom("South Bridge"); //10x4
            breeStreets[10, 3] = AddRoom("Periwinkle/Crissaegrim"); //11x4
            breeStreets[11, 3] = AddRoom("Periwinkle"); //12x4
            Room oPeriwinklePoorAlley = breeStreets[12, 3] = AddRoom("Periwinkle/PoorAlley"); //13x4
            breeStreets[13, 3] = AddRoom("Periwinkle"); //14x4
            breeStreets[14, 3] = AddRoom("Periwinkle/Brownhaven"); //15x4
            breeStreets[0, 4] = AddRoom("Wain"); //1x5
            breeSewers[0, 4] = AddRoom("Sewers Wain"); //1x5
            Room oToBlindPigPubAndUniversity = breeStreets[3, 4] = AddRoom("High"); //4x5
            breeStreets[7, 4] = AddRoom("Main"); //8x5
            Room oToSnarSlystoneShoppe = breeStreets[10, 4] = AddRoom("Crissaegrim"); //11x5
            breeStreets[14, 4] = AddRoom("Brownhaven"); //15x5
            breeStreets[0, 5] = AddRoom("Wain"); //1x6
            breeSewers[0, 5] = AddRoom("Sewers Wain"); //1x6
            breeStreets[3, 5] = AddRoom("High"); //4x6
            breeStreets[7, 5] = AddRoom("Main"); //8x6
            Room oBigPapa = breeStreets[8, 5] = AddRoom("Big Papa"); //9x6
            breeStreets[10, 5] = AddRoom("Crissaegrim"); //11x6
            breeStreets[14, 5] = AddRoom("Brownhaven"); //15x6
            breeStreets[0, 6] = AddRoom("Wain"); //1x7
            breeSewers[0, 6] = AddRoom("Sewers Wain"); //1x7
            breeStreets[3, 6] = AddRoom("High"); //4x7
            breeStreets[7, 6] = AddRoom("Main"); //8x7
            breeStreets[10, 6] = AddRoom("Crissaegrim"); //11x7
            breeStreets[14, 6] = AddRoom("Brownhaven"); //15x7
            oWestGateInside = breeStreets[0, 7] = AddRoom("West Gate Inside"); //1x8
            breeSewers[0, 7] = AddRoom("Sewers West Gate"); //1x8
            AddExit(breeSewers[0, 7], oWestGateInside, "up");
            breeStreets[1, 7] = AddRoom("Leviathan"); //2x8
            breeStreets[2, 7] = AddRoom("Leviathan"); //3x8
            breeStreets[3, 7] = AddRoom("Leviathan/High"); //4x8
            breeStreets[4, 7] = AddRoom("Leviathan"); //5x8
            oBreeTownSquare = breeStreets[5, 7] = AddRoom("Town Square"); //6x8
            breeStreets[6, 7] = AddRoom("Leviathan"); //7x8
            breeStreets[7, 7] = AddRoom("Leviathan/Main"); //8x8
            breeStreets[8, 7] = AddRoom("Leviathan"); //9x8
            Room oNorthBridge = breeStreets[9, 7] = AddRoom("North Bridge"); //10x8
            breeStreets[10, 7] = AddRoom("Leviathan/Crissaegrim"); //11x8
            breeStreets[11, 7] = AddRoom("Leviathan"); //12x8
            Room oLeviathanPoorAlley = breeStreets[12, 7] = AddRoom("Leviathan"); //13x8
            Room oToGrantsStables = breeStreets[13, 7] = AddRoom("Leviathan"); //14x8
            _breeEastGateInside = breeStreets[14, 7] = AddRoom("East Gate Inside"); //15x8
            breeStreets[0, 8] = AddRoom("Wain"); //1x9
            breeSewers[0, 8] = AddRoom("Sewers Wain"); //1x9
            breeStreets[3, 8] = AddRoom("High"); //4x9
            breeStreets[7, 8] = AddRoom("Main"); //8x9
            breeStreets[10, 8] = AddRoom("Crissaegrim"); //11x9
            breeStreets[14, 8] = AddRoom("Brownhaven"); //15x9
            Room oOrderOfLove = breeStreets[15, 8] = AddRoom("Order of Love"); //16x9
            oOrderOfLove.IsHealingRoom = true;
            switch (preferredAlignment)
            {
                case AlignmentType.Blue:
                    oOrderOfLove.Mob1 = "Drunk";
                    break;
                case AlignmentType.Red:
                    oOrderOfLove.Mob1 = "Doctor";
                    break;
            }
            breeStreets[0, 9] = AddRoom("Wain"); //1x10
            breeSewers[0, 9] = AddRoom("Sewers Wain"); //1x10
            breeStreets[3, 9] = AddRoom("High"); //4x10
            breeStreets[7, 9] = AddRoom("Main"); //8x10
            Room oToLeonardosFoundry = breeStreets[10, 9] = AddRoom("Crissaegrim"); //11x10
            Room oToGamblingPit = breeStreets[14, 9] = AddRoom("Brownhaven"); //15x10
            breeStreets[0, 10] = AddRoom("Ormenel/Wain"); //1x11
            breeSewers[0, 10] = AddRoom("Sewers Ormenel/Wain"); //1x11
            Exit e = AddExit(breeStreets[0, 10], breeSewers[0, 10], "sewer");
            e.PreCommand = "open sewer";
            breeStreets[1, 10] = AddRoom("Ormenel"); //2x11
            Room oToZoo = breeStreets[2, 10] = AddRoom("Ormenel"); //3x11
            breeStreets[3, 10] = AddRoom("Ormenel/High"); //4x11
            Room oToCasino = breeStreets[4, 10] = AddRoom("Ormenel"); //5x11
            breeStreets[5, 10] = AddRoom("Ormenel"); //6x11
            breeStreets[6, 10] = AddRoom("Ormenel"); //7x11
            breeStreets[7, 10] = AddRoom("Ormenel/Main"); //8x11
            breeStreets[10, 10] = AddRoom("Ormenel"); //11x11
            Room oToRealEstateOffice = breeStreets[11, 10] = AddRoom("Ormenel"); //12x11
            graphMillwoodMansion.Rooms[oToRealEstateOffice] = new System.Windows.Point(3, 0);
            breeStreets[12, 10] = AddRoom("Ormenel"); //13x11
            Room oStreetToFallon = breeStreets[13, 10] = AddRoom("Ormenel"); //14x11
            breeStreets[14, 10] = AddRoom("Brownhaven/Ormenel"); //15x11

            AddBreeSewers(breeStreets, breeSewers, out oSmoulderingVillage);

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    AddGridBidirectionalExits(breeStreets, x, y);
                }
            }

            Room oPoorAlley1 = AddRoom("Poor Alley");
            AddExit(oLeviathanPoorAlley, oPoorAlley1, "alley");
            AddExit(oPoorAlley1, oLeviathanPoorAlley, "north");
            _breeStreetsGraph.Rooms[oPoorAlley1] = new System.Windows.Point(12, 4);

            Room oPoorAlley2 = AddRoom("Poor Alley");
            AddBidirectionalExits(oPoorAlley1, oPoorAlley2, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oPoorAlley2] = new System.Windows.Point(12, 5);

            Room oPoorAlley3 = AddRoom("Poor Alley");
            AddBidirectionalExits(oPoorAlley2, oPoorAlley3, BidirectionalExitType.NorthSouth);
            AddExit(oPeriwinklePoorAlley, oPoorAlley3, "alley");
            AddExit(oPoorAlley3, oPeriwinklePoorAlley, "south");
            _breeStreetsGraph.Rooms[oPoorAlley3] = new System.Windows.Point(12, 6);

            Room oCampusFreeClinic = AddRoom("Bree Campus Free Clinic");
            oCampusFreeClinic.Mob1 = "Student";
            oCampusFreeClinic.IsHealingRoom = true;
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");
            _breeStreetsGraph.Rooms[oCampusFreeClinic] = new System.Windows.Point(4, 9);

            Room oBreeRealEstateOffice = AddRoom("Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oBreeRealEstateOffice] = new System.Windows.Point(11, -0.5);
            graphMillwoodMansion.Rooms[oBreeRealEstateOffice] = new System.Windows.Point(3, 1);

            oIxell = AddRoom("Ixell 70 Bl");
            oIxell.Mob1 = "Ixell";
            AddExit(oBreeRealEstateOffice, oIxell, "door");
            AddExit(oIxell, oBreeRealEstateOffice, "out");
            _breeStreetsGraph.Rooms[oIxell] = new System.Windows.Point(11, -1);
            graphMillwoodMansion.Rooms[oIxell] = new System.Windows.Point(2, 1);

            Room oKistaHillsHousing = AddRoom("Kista Hills Housing");
            AddBidirectionalExits(oStreetToFallon, oKistaHillsHousing, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oKistaHillsHousing] = new System.Windows.Point(13, -0.5);

            Room oChurchsEnglishGarden = AddRoom("Chuch's English Garden");
            AddBidirectionalSameNameExit(oKistaHillsHousing, oChurchsEnglishGarden, "gate");
            Room oFallon = AddRoom("Fallon");
            oFallon.Experience1 = 350;
            oFallon.Alignment = AlignmentType.Blue;
            AddExit(oChurchsEnglishGarden, oFallon, "door");
            AddExit(oFallon, oChurchsEnglishGarden, "out");
            oChurchsEnglishGarden.Mob1 = oFallon.Mob1 = "Fallon";
            _breeStreetsGraph.Rooms[oChurchsEnglishGarden] = new System.Windows.Point(13, -1);
            _breeStreetsGraph.Rooms[oFallon] = new System.Windows.Point(13, -1.5);

            Room oGrantsStables = AddRoom("Grant's stables");
            if (level < 11)
            {
                AddExit(oToGrantsStables, oGrantsStables, "stable");
            }
            AddExit(oGrantsStables, oToGrantsStables, "south");

            Room oGrant = AddRoom("Grant");
            oGrant.Mob1 = "Grant";
            oGrant.Experience1 = 170;
            Exit oToGrant = AddExit(oGrantsStables, oGrant, "gate");
            oToGrant.PreCommand = "open gate";
            AddExit(oGrant, oGrantsStables, "out");

            Room oPansy = AddRoom("Pansy Smallburrows");
            oPansy.Mob1 = "Pansy";
            oPansy.Experience1 = 95;
            oPansy.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oPansy] = new System.Windows.Point(13, 1);

            oDroolie = AddRoom("Droolie");
            oDroolie.Mob1 = "Droolie";
            oDroolie.Experience1 = 100;
            oDroolie.Alignment = AlignmentType.Red;
            e = AddExit(oNorthBridge, oDroolie, "rope");
            e.Hidden = true;
            AddExit(oDroolie, oNorthBridge, "up");
            _breeStreetsGraph.Rooms[oDroolie] = new System.Windows.Point(9, 3.5);

            Room oIgor = AddRoom("Igor");
            oIgor.Mob1 = "Igor";
            oIgor.Experience1 = 130;
            oIgor.Alignment = AlignmentType.Grey;
            AddExit(oIgor, oToBlindPigPubAndUniversity, "east");
            AddExit(oToBlindPigPubAndUniversity, oIgor, "pub");
            _breeStreetsGraph.Rooms[oIgor] = new System.Windows.Point(2, 6);

            Room oSnarlingMutt = AddRoom("Snarling Mutt");
            oSnarlingMutt.Mob1 = "Mutt";
            oSnarlingMutt.Experience1 = 50;
            oSnarlingMutt.Alignment = AlignmentType.Red;
            AddExit(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            AddExit(oSnarlingMutt, oToSnarSlystoneShoppe, "out");
            _breeStreetsGraph.Rooms[oSnarlingMutt] = new System.Windows.Point(9, 6);

            Room oGuido = AddRoom("Guido");
            oGuido.Mob1 = "Guido";
            oGuido.Experience1 = 350;
            oGuido.Alignment = AlignmentType.Red;
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");
            _breeStreetsGraph.Rooms[oGuido] = new System.Windows.Point(4, -0.5);

            Room oGodfather = AddRoom("Godfather");
            oGodfather.Mob1 = "Godfather";
            oGodfather.Experience1 = 1200;
            e = AddExit(oGuido, oGodfather, "door");
            e.Hidden = true;
            e.PreCommand = "open door";
            e = AddExit(oGodfather, oGuido, "door");
            e.PreCommand = "open door";
            _breeStreetsGraph.Rooms[oGodfather] = new System.Windows.Point(4, -1);

            Room oSergeantGrimdall = AddRoom("Sergeant Grimdall");
            oSergeantGrimdall.Mob1 = "Sergeant";
            oSergeantGrimdall.Experience1 = 350;
            oSergeantGrimdall.Alignment = AlignmentType.Blue;
            AddExit(oToBarracks, oSergeantGrimdall, "barracks");
            AddExit(oSergeantGrimdall, oToBarracks, "east");
            _breeStreetsGraph.Rooms[oSergeantGrimdall] = new System.Windows.Point(6, 8);

            Room oGuardsRecRoom = AddRoom("Guard's Rec Room");
            AddBidirectionalExits(oSergeantGrimdall, oGuardsRecRoom, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oGuardsRecRoom] = new System.Windows.Point(6, 8.5);

            oBigPapa.Mob1 = "papa";
            oBigPapa.Experience1 = 350;
            oBigPapa.Alignment = AlignmentType.Blue;

            Room oBreePawnShopWest = AddRoom("Ixell's Antique Shop");
            AddBidirectionalExits(oBreePawnShopWest, oToPawnShopWest, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBreePawnShopWest] = new System.Windows.Point(2, 8);

            Room oBreePawnShopEast = AddRoom("Pawn Shop");
            AddBidirectionalExits(oPoorAlley1, oBreePawnShopEast, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBreePawnShopEast] = new System.Windows.Point(13, 4);

            Room oLeonardosFoundry = AddRoom("Leonardo's Foundry");
            AddExit(oToLeonardosFoundry, oLeonardosFoundry, "foundry");
            AddExit(oLeonardosFoundry, oToLeonardosFoundry, "east");
            _breeStreetsGraph.Rooms[oLeonardosFoundry] = new System.Windows.Point(9, 1);

            Room oLeonardosSwords = AddRoom("Leonardo's Swords");
            AddBidirectionalExits(oLeonardosSwords, oLeonardosFoundry, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oLeonardosSwords] = new System.Windows.Point(9, 0.5);

            Room oZooEntrance = AddRoom("Scranlin's Zoological Wonders");
            AddExit(oToZoo, oZooEntrance, "zoo");
            AddExit(oZooEntrance, oToZoo, "exit");
            _breeStreetsGraph.Rooms[oZooEntrance] = new System.Windows.Point(2, -0.5);

            Room oPathThroughScranlinsZoo = AddRoom("Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo, oZooEntrance, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oPathThroughScranlinsZoo] = new System.Windows.Point(2, -1);

            Room oScranlinsPettingZoo = AddRoom("Scranlin's Petting Zoo");
            e = AddExit(oPathThroughScranlinsZoo, oScranlinsPettingZoo, "north");
            e.OmitGo = true;
            AddExit(oScranlinsPettingZoo, oPathThroughScranlinsZoo, "south");
            _breeStreetsGraph.Rooms[oScranlinsPettingZoo] = new System.Windows.Point(2, -1.5);

            Room oScranlinThreshold = AddRoom("Scranlin's Training Area");
            e = AddExit(oScranlinsPettingZoo, oScranlinThreshold, "clearing");
            e.Hidden = true;
            AddExit(oScranlinThreshold, oScranlinsPettingZoo, "gate");
            _breeStreetsGraph.Rooms[oScranlinThreshold] = new System.Windows.Point(2, -2);

            Room oScranlin = AddRoom("Scranlin");
            oScranlin.Mob1 = oScranlinThreshold.Mob1 = "Scranlin";
            oScranlin.Experience1 = 500;
            oScranlin.Alignment = AlignmentType.Red;
            e = AddExit(oScranlinThreshold, oScranlin, "outhouse");
            e.Hidden = true;
            AddExit(oScranlin, oScranlinThreshold, "out");
            _breeStreetsGraph.Rooms[oScranlin] = new System.Windows.Point(2, -2.5);

            _boatswain = AddRoom("Boatswain");
            _boatswain.Mob1 = "Boatswain";
            _boatswain.Experience1 = 350;
            AddLocation(_aShips, _boatswain);

            Room oPearlAlley = AddRoom("Pearl Alley");
            AddExit(oBreeTownSquare, oPearlAlley, "alley");
            AddExit(oPearlAlley, oBreeTownSquare, "north");
            _breeStreetsGraph.Rooms[oPearlAlley] = new System.Windows.Point(5, 3.5);

            Room oBartenderWaitress = AddRoom("Prancing Pony Bar/Wait");
            oBartenderWaitress.Mob1 = "Bartender";
            oBartenderWaitress.Mob2 = "Waitress";
            oBartenderWaitress.Experience1 = 15;
            oBartenderWaitress.Experience2 = 7;
            AddBidirectionalExits(oPearlAlley, oBartenderWaitress, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBartenderWaitress] = new System.Windows.Point(6, 3.5);

            AddLocation(_aBreePerms, oOrderOfLove);
            AddLocation(_aInaccessible, oGrant);
            AddLocation(_aBreePerms, oGuido);
            AddLocation(_aBreePerms, oGodfather);
            AddLocation(_aBreePerms, oFallon);
            AddLocation(_aBreePerms, oBigPapa);
            AddLocation(_aBreePerms, oScranlin);
        }

        private void AddUnderBree(Room droolie, Room oOuthouse, Room oSewerPipeExit)
        {
            RoomGraph underBreeGraph = new RoomGraph("Under Bree");
            underBreeGraph.ScalingFactor = 100;
            _graphs[MapType.UnderBree] = underBreeGraph;

            underBreeGraph.Rooms[droolie] = new System.Windows.Point(0, 0);
            underBreeGraph.Rooms[oOuthouse] = new System.Windows.Point(8, 12);
            underBreeGraph.Rooms[oSewerPipeExit] = new System.Windows.Point(7, 2);

            Room oCatchBasin = AddRoom("Catch Basin");
            AddExit(oOuthouse, oCatchBasin, "hole");
            AddExit(oCatchBasin, oOuthouse, "out");
            underBreeGraph.Rooms[oCatchBasin] = new System.Windows.Point(8, 11);

            Room oSepticTank = AddRoom("Septic Tank");
            AddBidirectionalSameNameExit(oCatchBasin, oSepticTank, "grate");
            underBreeGraph.Rooms[oSepticTank] = new System.Windows.Point(8, 10);

            Room oDrainPipe1 = AddRoom("Drain Pipe");
            AddBidirectionalSameNameExit(oSepticTank, oDrainPipe1, "pipe");
            underBreeGraph.Rooms[oDrainPipe1] = new System.Windows.Point(8, 9);

            Room oDrainPipe2 = AddRoom("Drain Pipe");
            AddBidirectionalSameNameExit(oDrainPipe1, oDrainPipe2, "culvert");
            underBreeGraph.Rooms[oDrainPipe2] = new System.Windows.Point(8, 8);

            Room oBrandywineRiverShore = AddRoom("Brandywine Shore");
            AddExit(oDrainPipe2, oBrandywineRiverShore, "out");
            AddExit(oBrandywineRiverShore, oDrainPipe2, "grate");
            underBreeGraph.Rooms[oBrandywineRiverShore] = new System.Windows.Point(8, 7);

            Room oSewerDitch = AddRoom("Sewer Ditch");
            AddExit(oBrandywineRiverShore, oSewerDitch, "ditch");
            AddExit(oSewerDitch, oBrandywineRiverShore, "out");
            underBreeGraph.Rooms[oSewerDitch] = new System.Windows.Point(8, 6);

            Room oSewerTunnel1 = AddRoom("Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel1, oSewerDitch, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTunnel1] = new System.Windows.Point(8, 5);

            Room oSewerTConnection = AddRoom("Sewer T-Connection");
            AddBidirectionalExits(oSewerTConnection, oSewerTunnel1, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[oSewerTConnection] = new System.Windows.Point(8, 4);

            Room oSewerTunnel2 = AddRoom("Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel2, oSewerTConnection, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oSewerTunnel2] = new System.Windows.Point(7, 4);

            Room oSewerPipe = AddRoom("Sewer Pipe");
            AddExit(oSewerTunnel2, oSewerPipe, "pipe");
            AddExit(oSewerPipe, oSewerTunnel2, "down");
            AddExit(oSewerPipe, oSewerPipeExit, "up");
            underBreeGraph.Rooms[oSewerPipe] = new System.Windows.Point(7, 3);

            Room oSalamander = AddRoom("Salamander");
            oSalamander.Mob1 = "Salamander";
            oSalamander.Experience1 = 100;
            oSalamander.Alignment = AlignmentType.Red;
            AddExit(oBrandywineRiverShore, oSalamander, "reeds");
            AddExit(oSalamander, oBrandywineRiverShore, "shore");
            underBreeGraph.Rooms[oSalamander] = new System.Windows.Point(9, 7);

            Room oBrandywineRiver1 = AddRoom("Brandywine River");
            AddExit(droolie, oBrandywineRiver1, "down");
            //AddExit(oBrandywineRiver1, droolie, "rope"); //requires fly
            underBreeGraph.Rooms[oBrandywineRiver1] = new System.Windows.Point(0, 1);

            Room oBrandywineRiver2 = AddRoom("Brandywine River");
            AddBidirectionalExits(oBrandywineRiver1, oBrandywineRiver2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBrandywineRiver2] = new System.Windows.Point(1, 1);

            Room oOohlgrist = AddRoom("Oohlgrist");
            oOohlgrist.Mob1 = "Oohlgrist";
            oOohlgrist.Experience1 = 110;
            AddExit(oBrandywineRiver2, oOohlgrist, "boat");
            AddExit(oOohlgrist, oBrandywineRiver2, "river");
            underBreeGraph.Rooms[oOohlgrist] = new System.Windows.Point(2, 1);

            Room oBrandywineRiverBoathouse = AddRoom("Brandywine Boathouse");
            AddExit(oOohlgrist, oBrandywineRiverBoathouse, "shore");
            AddExit(oBrandywineRiverBoathouse, oOohlgrist, "boat");
            underBreeGraph.Rooms[oBrandywineRiverBoathouse] = new System.Windows.Point(3, 1);

            Room oRockyBeach1 = AddRoom("Rocky Beach");
            AddBidirectionalExits(oBrandywineRiverBoathouse, oRockyBeach1, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oRockyBeach1] = new System.Windows.Point(4, 1);

            Room oRockyBeach2 = AddRoom("Rocky Beach");
            AddBidirectionalExits(oRockyBeach1, oRockyBeach2, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oRockyBeach2] = new System.Windows.Point(5, 1);

            Room oHermitsCave = AddRoom("Hermit Fisher");
            oHermitsCave.Mob1 = "Fisher";
            oHermitsCave.Experience1 = 60;
            AddExit(oRockyBeach2, oHermitsCave, "cave");
            AddExit(oHermitsCave, oRockyBeach2, "out");
            underBreeGraph.Rooms[oHermitsCave] = new System.Windows.Point(6, 1);

            Room oRockyAlcove = AddRoom("Rocky Alcove");
            AddExit(oRockyBeach1, oRockyAlcove, "alcove");
            AddExit(oRockyAlcove, oRockyBeach1, "north");
            underBreeGraph.Rooms[oRockyAlcove] = new System.Windows.Point(5, 0);

            Room oSewerDrain = AddRoom("Sewer Drain");
            AddBidirectionalSameNameExit(oRockyAlcove, oSewerDrain, "grate");
            underBreeGraph.Rooms[oSewerDrain] = new System.Windows.Point(7, 0);

            Room oDrainTunnel1 = AddRoom("Drain Tunnel");
            AddExit(oSewerDrain, oDrainTunnel1, "south");
            underBreeGraph.Rooms[oDrainTunnel1] = new System.Windows.Point(7, 1);

            Room oDrainTunnel2 = AddRoom("Drain Tunnel");
            AddExit(oDrainTunnel1, oDrainTunnel2, "north");
            underBreeGraph.Rooms[oDrainTunnel2] = new System.Windows.Point(8, 0);

            Room oDrainTunnel3 = AddRoom("Drain Tunnel");
            AddExit(oDrainTunnel2, oDrainTunnel3, "south");
            underBreeGraph.Rooms[oDrainTunnel3] = new System.Windows.Point(8, 1);

            Room oDrainTunnel4 = AddRoom("Drain Tunnel");
            AddExit(oDrainTunnel3, oDrainTunnel4, "south");
            underBreeGraph.Rooms[oDrainTunnel4] = new System.Windows.Point(8, 2);

            Room sewerTunnelToTConnection = AddRoom("Sewer Tunnel");
            AddBidirectionalExits(oDrainTunnel4, sewerTunnelToTConnection, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(sewerTunnelToTConnection, oSewerTConnection, BidirectionalExitType.NorthSouth);
            underBreeGraph.Rooms[sewerTunnelToTConnection] = new System.Windows.Point(8, 3);

            Room oBoardedSewerTunnel = AddRoom("Boarded Tunnel");
            AddBidirectionalExits(sewerTunnelToTConnection, oBoardedSewerTunnel, BidirectionalExitType.WestEast);
            underBreeGraph.Rooms[oBoardedSewerTunnel] = new System.Windows.Point(9, 3);

            Room oSewerOrcChamber = AddRoom("Sewer Orc Guards");
            oSewerOrcChamber.Mob1 = "Guard";
            oSewerOrcChamber.Experience1 = 70;
            oSewerOrcChamber.Experience2 = 70;
            Exit e = AddExit(oBoardedSewerTunnel, oSewerOrcChamber, "busted");
            e.Hidden = true;
            e = AddExit(oSewerOrcChamber, oBoardedSewerTunnel, "busted");
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

        private void AddBreeSewers(Room[,] breeStreets, Room[,] breeSewers, out Room oSmoulderingVillage)
        {
            RoomGraph breeSewersGraph = new RoomGraph("Bree Sewers");
            breeSewersGraph.ScalingFactor = 100;
            _graphs[MapType.BreeSewers] = breeSewersGraph;

            //add exits for the sewers. due to screwiness on periwinkle this can't be done automatically.
            AddBidirectionalExits(breeSewers[0, 10], breeSewers[0, 9], BidirectionalExitType.NorthSouth);
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

            Room oTunnel = AddRoom("Tunnel");
            Exit e = AddExit(breeSewers[0, 10], oTunnel, "tunnel");
            e.Hidden = true;
            AddExit(oTunnel, breeSewers[0, 10], "tunnel");
            breeSewersGraph.Rooms[oTunnel] = new System.Windows.Point(4, 2);

            Room oLatrine = AddRoom("Latrine");
            AddExit(oTunnel, oLatrine, "south");
            e = AddExit(oLatrine, oTunnel, "north");
            e.OmitGo = true;
            e.Hidden = true;
            breeSewersGraph.Rooms[oLatrine] = new System.Windows.Point(4, 3);

            Room oEugenesDungeon = AddRoom("Eugene's Dungeon");
            AddBidirectionalExits(oEugenesDungeon, oLatrine, BidirectionalExitType.SouthwestNortheast);
            breeSewersGraph.Rooms[oEugenesDungeon] = new System.Windows.Point(3, 2);

            Room oShadowOfIncendius = AddRoom("Shadow of Incendius");
            AddBidirectionalExits(oShadowOfIncendius, oEugenesDungeon, BidirectionalExitType.WestEast);
            breeSewersGraph.Rooms[oShadowOfIncendius] = new System.Windows.Point(2, 2);

            Room oEugeneTheExecutioner = AddRoom("Eugene the Executioner");
            oEugeneTheExecutioner.IsTrapRoom = true;
            AddExit(oEugenesDungeon, oEugeneTheExecutioner, "up");
            breeSewersGraph.Rooms[oEugeneTheExecutioner] = new System.Windows.Point(3, 1);

            Room oBurnedRemainsOfNimrodel = AddRoom("Nimrodel");
            oBurnedRemainsOfNimrodel.Mob1 = "Nimrodel";
            oBurnedRemainsOfNimrodel.Experience1 = 300;
            AddExit(oEugeneTheExecutioner, oBurnedRemainsOfNimrodel, "out");
            AddExit(oBurnedRemainsOfNimrodel, oEugeneTheExecutioner, "door");
            breeSewersGraph.Rooms[oBurnedRemainsOfNimrodel] = new System.Windows.Point(2, 1);

            Room aqueduct = AddRoom("Aqueduct");
            AddExit(oBurnedRemainsOfNimrodel, aqueduct, "pipe");
            AddExit(aqueduct, oBurnedRemainsOfNimrodel, "out");
            breeSewersGraph.Rooms[aqueduct] = new System.Windows.Point(1, 2);

            Room oShirriff = breeSewers[7, 3];
            oShirriff.Mob1 = "shirriff";
            oShirriff.Experience1 = 325;
            oShirriff.Experience2 = 325;

            Room oValveChamber = AddRoom("Valve Chamber");
            e = AddExit(breeSewers[7, 3], oValveChamber, "valve");
            e.Hidden = true;
            AddExit(oValveChamber, breeSewers[7, 3], "south");
            breeSewersGraph.Rooms[oValveChamber] = new System.Windows.Point(12, 8);

            Room oSewerPassageFromValveChamber = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSewerPassageFromValveChamber, oValveChamber, BidirectionalExitType.NorthSouth);
            breeSewersGraph.Rooms[oSewerPassageFromValveChamber] = new System.Windows.Point(12, 7);

            Room oSewerDemonThreshold = AddRoom("Central Sewer Channels");
            oSewerDemonThreshold.Mob1 = "demon";
            AddBidirectionalExits(oSewerDemonThreshold, oSewerPassageFromValveChamber, BidirectionalExitType.SoutheastNorthwest);
            breeSewersGraph.Rooms[oSewerDemonThreshold] = new System.Windows.Point(11, 6);

            oSmoulderingVillage = AddRoom("Smoldering Village");
            breeSewersGraph.Rooms[oSmoulderingVillage] = new System.Windows.Point(0, 0);

            Room oWell = AddRoom("Well");
            AddExit(oSmoulderingVillage, oWell, "well");
            AddExit(oWell, oSmoulderingVillage, "ladder");
            breeSewersGraph.Rooms[oWell] = new System.Windows.Point(1, 0);

            Room oKasnarTheGuard = AddRoom("Kasnar");
            oKasnarTheGuard.Mob1 = "Kasnar";
            oKasnarTheGuard.Experience1 = 535;
            AddExit(oWell, oKasnarTheGuard, "pipe");
            AddExit(oKasnarTheGuard, oWell, "north");
            breeSewersGraph.Rooms[oKasnarTheGuard] = new System.Windows.Point(1, 1);

            AddExit(aqueduct, oKasnarTheGuard, "north");
            //AddExit(oKasnarTheGuard, aqueduct, "south") //Exit is locked and knockable but not treating as an exit for the mapping

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
            string sWarriorBard = "Warrior bard";

            Room oPathToMansion1 = AddRoom("Construction Site");
            AddExit(oIxell, oPathToMansion1, "back");
            AddExit(oPathToMansion1, oIxell, "hoist");
            graphMillwoodMansion.Rooms[oPathToMansion1] = new System.Windows.Point(1, 1);

            Room oPathToMansion2 = AddRoom("Southern View");
            AddBidirectionalExits(oPathToMansion1, oPathToMansion2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion2] = new System.Windows.Point(1, 2);

            Room oPathToMansion3 = AddRoom("The South Wall");
            AddBidirectionalExits(oPathToMansion2, oPathToMansion3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion3] = new System.Windows.Point(1, 3);

            Room oPathToMansion4WarriorBardsx2 = AddRoom("Warrior Bards (Path)");
            oPathToMansion4WarriorBardsx2.Mob1 = sWarriorBard;
            oPathToMansion4WarriorBardsx2.Experience1 = 100;
            oPathToMansion4WarriorBardsx2.Alignment = AlignmentType.Red;
            AddExit(oPathToMansion3, oPathToMansion4WarriorBardsx2, "stone");
            AddExit(oPathToMansion4WarriorBardsx2, oPathToMansion3, "north");
            graphMillwoodMansion.Rooms[oPathToMansion4WarriorBardsx2] = new System.Windows.Point(1, 4);

            Room oPathToMansion5 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion4WarriorBardsx2, oPathToMansion5, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oPathToMansion5] = new System.Windows.Point(0, 5);

            Room oPathToMansion6 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion5, oPathToMansion6, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion6] = new System.Windows.Point(0, 6);

            Room oPathToMansion7 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion6, oPathToMansion7, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oPathToMansion7] = new System.Windows.Point(1, 7);

            Room oPathToMansion8 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion7, oPathToMansion8, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion8] = new System.Windows.Point(1, 8);

            Room oPathToMansion9 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion8, oPathToMansion9, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oPathToMansion9] = new System.Windows.Point(2, 9);

            Room oPathToMansion10 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion9, oPathToMansion10, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oPathToMansion10] = new System.Windows.Point(1, 10);

            Room oPathToMansion11 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion10, oPathToMansion11, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oPathToMansion11] = new System.Windows.Point(1, 11);

            Room oPathToMansion12 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion11, oPathToMansion12, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oPathToMansion12] = new System.Windows.Point(2, 11);

            Room oGrandPorch = AddRoom("Warrior Bard (Porch)");
            oGrandPorch.Mob1 = sWarriorBard;
            oGrandPorch.Experience1 = 100;
            oGrandPorch.Alignment = AlignmentType.Red;
            AddExit(oPathToMansion12, oGrandPorch, "porch");
            AddExit(oGrandPorch, oPathToMansion12, "path");
            graphMillwoodMansion.Rooms[oGrandPorch] = new System.Windows.Point(3, 11);

            Room oMansionInside1 = AddRoom("Mansion Inside");
            AddBidirectionalSameNameExit(oGrandPorch, oMansionInside1, "door", "open door");
            graphMillwoodMansion.Rooms[oMansionInside1] = new System.Windows.Point(4, 11);

            Room oMansionInside2 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionInside1, oMansionInside2, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionInside2] = new System.Windows.Point(5, 11);

            Room oMansionFirstFloorToNorthStairwell1 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell1, oMansionInside2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell1] = new System.Windows.Point(5, 10);

            Room oMansionFirstFloorToNorthStairwell2 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell2, oMansionFirstFloorToNorthStairwell1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell2] = new System.Windows.Point(5, 9);

            Room oMansionFirstFloorToNorthStairwell3 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell3, oMansionFirstFloorToNorthStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell3] = new System.Windows.Point(5, 8);

            Room oMansionFirstFloorToNorthStairwell4 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell4] = new System.Windows.Point(5, 7);

            Room oMansionFirstFloorToNorthStairwell5 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToNorthStairwell5] = new System.Windows.Point(6, 7);

            Room oWarriorBardMansionNorth = AddRoom("Warrior Bard Mansion N");
            oWarriorBardMansionNorth.Mob1 = sWarriorBard;
            oWarriorBardMansionNorth.Experience1 = 100;
            oWarriorBardMansionNorth.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oWarriorBardMansionNorth, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oWarriorBardMansionNorth] = new System.Windows.Point(6, 6);

            Room oMansionFirstFloorToSouthStairwell1 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToSouthStairwell1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell1] = new System.Windows.Point(5, 12);

            Room oMansionFirstFloorToSouthStairwell2 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell1, oMansionFirstFloorToSouthStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell2] = new System.Windows.Point(5, 13);

            Room oMansionFirstFloorToSouthStairwell3 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell2, oMansionFirstFloorToSouthStairwell3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell3] = new System.Windows.Point(5, 14);

            Room oMansionFirstFloorToSouthStairwell4 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell3, oMansionFirstFloorToSouthStairwell4, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell4] = new System.Windows.Point(5, 15);

            Room oMansionFirstFloorToSouthStairwell5 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell4, oMansionFirstFloorToSouthStairwell5, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToSouthStairwell5] = new System.Windows.Point(6, 15);

            Room oWarriorBardMansionSouth = AddRoom("Warrior Bard Mansion S");
            oWarriorBardMansionSouth.Mob1 = sWarriorBard;
            oWarriorBardMansionSouth.Experience1 = 100;
            oWarriorBardMansionSouth.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell5, oWarriorBardMansionSouth, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oWarriorBardMansionSouth] = new System.Windows.Point(6, 16);

            Room oMansionFirstFloorToEastStairwell1 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToEastStairwell1, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell1] = new System.Windows.Point(6, 11);

            Room oMansionFirstFloorToEastStairwell2 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell1, oMansionFirstFloorToEastStairwell2, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell2] = new System.Windows.Point(7, 11);

            Room oMansionFirstFloorToEastStairwell3 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell2, oMansionFirstFloorToEastStairwell3, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell3] = new System.Windows.Point(8, 11);

            Room oMansionFirstFloorToEastStairwell4 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell3, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell4] = new System.Windows.Point(9, 11);

            Room oMansionFirstFloorToEastStairwell5 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.SouthwestNortheast);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell5] = new System.Windows.Point(10, 10);

            Room oMansionFirstFloorToEastStairwell6 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.SoutheastNorthwest);
            graphMillwoodMansion.Rooms[oMansionFirstFloorToEastStairwell6] = new System.Windows.Point(11, 11);

            Room oWarriorBardMansionEast = AddRoom("Warrior Bard Mansion E");
            oWarriorBardMansionEast.Mob1 = sWarriorBard;
            oWarriorBardMansionEast.Experience1 = 100;
            oWarriorBardMansionEast.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oWarriorBardMansionEast, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oWarriorBardMansionEast] = new System.Windows.Point(10, 11);

            Room oNorthHallway1 = AddRoom("North Hallway");
            AddBidirectionalExits(oNorthHallway1, oMansionFirstFloorToEastStairwell2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway1] = new System.Windows.Point(7, 10);

            Room oNorthHallway2 = AddRoom("North Hallway");
            AddBidirectionalExits(oNorthHallway2, oNorthHallway1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway2] = new System.Windows.Point(7, 9);

            Room oNorthHallway3 = AddRoom("North Hallway");
            AddBidirectionalExits(oNorthHallway3, oNorthHallway2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oNorthHallway3] = new System.Windows.Point(7, 8);

            Room oDungeonGuardNorth = AddRoom("Dungeon Guard");
            oDungeonGuardNorth.Mob1 = "guard";
            oDungeonGuardNorth.Experience1 = 120;
            AddBidirectionalExits(oNorthHallway3, oDungeonGuardNorth, BidirectionalExitType.WestEast);
            graphMillwoodMansion.Rooms[oDungeonGuardNorth] = new System.Windows.Point(8, 8);

            Room oSouthHallway1 = AddRoom("South Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell2, oSouthHallway1, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway1] = new System.Windows.Point(7, 12);

            Room oSouthHallway2 = AddRoom("South Hallway");
            AddBidirectionalExits(oSouthHallway1, oSouthHallway2, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway2] = new System.Windows.Point(7, 13);

            Room oSouthHallway3 = AddRoom("South Hallway");
            AddBidirectionalExits(oSouthHallway2, oSouthHallway3, BidirectionalExitType.NorthSouth);
            graphMillwoodMansion.Rooms[oSouthHallway3] = new System.Windows.Point(7, 14);

            Room oDungeonGuardSouth = AddRoom("Dungeon Guard");
            oDungeonGuardSouth.Mob1 = "guard";
            oDungeonGuardSouth.Experience1 = 120;
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

            Room oGrandStaircaseUpstairs = AddRoom("Grand Staircase");
            AddBidirectionalExits(oGrandStaircaseUpstairs, eastStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oGrandStaircaseUpstairs] = new System.Windows.Point(5, 6);

            Room oRoyalHallwayUpstairs = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oGrandStaircaseUpstairs, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayUpstairs] = new System.Windows.Point(4, 6);

            Room oRoyalHallwayToMayor = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oRoyalHallwayToMayor, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayToMayor] = new System.Windows.Point(4, 7);

            Room oRoyalHallwayToChancellor = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayToChancellor, oRoyalHallwayUpstairs, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallwayToChancellor] = new System.Windows.Point(4, 5);

            Room oRoyalHallway1 = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallway1, oRoyalHallwayUpstairs, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway1] = new System.Windows.Point(3, 6);

            Room oRoyalHallway2 = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallway2, oRoyalHallway1, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway2] = new System.Windows.Point(2, 6);

            Room oRoyalHallway3 = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallway3, oRoyalHallway2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oRoyalHallway3] = new System.Windows.Point(1, 6);

            Room oNorthCorridor1 = AddRoom("North Corridor");
            AddBidirectionalExits(oNorthCorridor1, oRoyalHallway3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor1] = new System.Windows.Point(1, 5);

            Room oNorthCorridor2 = AddRoom("North Corridor");
            AddBidirectionalExits(oNorthCorridor2, oNorthCorridor1, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor2] = new System.Windows.Point(1, 4);

            Room oDiningArea = AddRoom("Dining Area");
            AddBidirectionalExits(oDiningArea, oNorthCorridor2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oDiningArea] = new System.Windows.Point(0, 4);

            Room oNorthCorridor3 = AddRoom("North Corridor");
            AddBidirectionalExits(oNorthCorridor3, oNorthCorridor2, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor3] = new System.Windows.Point(1, 3);

            Room oNorthCorridor4 = AddRoom("North Corridor");
            AddBidirectionalExits(oNorthCorridor4, oNorthCorridor3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oNorthCorridor4] = new System.Windows.Point(1, 2);

            Room oMeditationChamber = AddRoom("Meditation Chamber");
            Exit e = AddExit(oNorthCorridor4, oMeditationChamber, "door");
            e.PreCommand = "open door";
            AddExit(oMeditationChamber, oNorthCorridor4, "out");
            oMeditationChamber.IsHealingRoom = true;
            millwoodMansionUpstairsGraph.Rooms[oMeditationChamber] = new System.Windows.Point(0, 2);

            Room oNorthernStairwell = AddRoom("Northern Stairwell");
            AddBidirectionalExits(oNorthernStairwell, oNorthCorridor4, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthernStairwell, northStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oNorthernStairwell] = new System.Windows.Point(1, 1);

            Room oSouthCorridor1 = AddRoom("South Corridor");
            AddBidirectionalExits(oRoyalHallway3, oSouthCorridor1, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor1] = new System.Windows.Point(1, 7);

            Room oSouthCorridor2 = AddRoom("South Corridor");
            AddBidirectionalExits(oSouthCorridor1, oSouthCorridor2, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor2] = new System.Windows.Point(1, 8);

            Room oKnightsQuarters = AddRoom("Knights' Quarters");
            AddBidirectionalExits(oKnightsQuarters, oSouthCorridor2, BidirectionalExitType.WestEast);
            millwoodMansionUpstairsGraph.Rooms[oKnightsQuarters] = new System.Windows.Point(0, 8);

            Room oSouthCorridor3 = AddRoom("South Corridor");
            AddBidirectionalExits(oSouthCorridor2, oSouthCorridor3, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor3] = new System.Windows.Point(1, 9);

            Room oSouthCorridor4 = AddRoom("South Corridor");
            AddBidirectionalExits(oSouthCorridor3, oSouthCorridor4, BidirectionalExitType.NorthSouth);
            millwoodMansionUpstairsGraph.Rooms[oSouthCorridor4] = new System.Windows.Point(1, 10);

            Room oStorageRoom = AddRoom("Storage Room");
            e = AddExit(oSouthCorridor4, oStorageRoom, "door");
            e.PreCommand = "open door";
            AddExit(oStorageRoom, oSouthCorridor4, "out");
            millwoodMansionUpstairsGraph.Rooms[oStorageRoom] = new System.Windows.Point(0, 10);

            Room oSouthernStairwell = AddRoom("Southern Stairwell");
            AddBidirectionalExits(oSouthCorridor4, oSouthernStairwell, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oSouthernStairwell, southStairwell, BidirectionalExitType.UpDown);
            millwoodMansionUpstairsGraph.Rooms[oSouthernStairwell] = new System.Windows.Point(1, 11);

            //mayor is immune to stun
            Room oMayorMillwood = AddRoom("Mayor Millwood");
            oMayorMillwood.Experience1 = 220;
            oMayorMillwood.Alignment = AlignmentType.Grey;
            e = AddExit(oRoyalHallwayToMayor, oMayorMillwood, "chamber");
            e.PreCommand = "open chamber";
            AddExit(oMayorMillwood, oRoyalHallwayToMayor, "out");
            oMayorMillwood.Mob1 = oRoyalHallwayToMayor.Mob1 = "mayor";
            millwoodMansionUpstairsGraph.Rooms[oMayorMillwood] = new System.Windows.Point(4, 8);

            Room oChancellorOfProtection = AddRoom("Chancellor of Protection");
            oChancellorOfProtection.Experience1 = 200;
            oChancellorOfProtection.Alignment = AlignmentType.Blue;
            e = AddExit(oRoyalHallwayToChancellor, oChancellorOfProtection, "chamber");
            e.PreCommand = "open chamber";
            AddExit(oChancellorOfProtection, oRoyalHallwayToChancellor, "out");
            oChancellorOfProtection.Mob1 = oRoyalHallwayToChancellor.Mob1 = "chancellor";
            millwoodMansionUpstairsGraph.Rooms[oChancellorOfProtection] = new System.Windows.Point(4, 4);

            AddLocation(_aBreePerms, oChancellorOfProtection);
            AddLocation(_aBreePerms, oMayorMillwood);
        }

        private void AddBreeToImladris(int level, out Room oOuthouse)
        {
            RoomGraph breeToImladrisGraph = new RoomGraph("Bree/Imladris");
            breeToImladrisGraph.ScalingFactor = 100;
            _graphs[MapType.BreeToImladris] = breeToImladrisGraph;

            _breeEastGateOutside = AddRoom("East Gate Outside");
            AddExit(_breeEastGateInside, _breeEastGateOutside, "gate");
            _breeStreetsGraph.Rooms[_breeEastGateOutside] = new System.Windows.Point(15, 3);
            breeToImladrisGraph.Rooms[_breeEastGateOutside] = new System.Windows.Point(3, 4);

            Room oGreatEastRoad1 = AddRoom("Great East Road");
            AddBidirectionalExits(_breeEastGateOutside, oGreatEastRoad1, BidirectionalExitType.WestEast);
            AddToFarmHouseAndUglies(oGreatEastRoad1, out oOuthouse, breeToImladrisGraph, level);
            breeToImladrisGraph.Rooms[oGreatEastRoad1] = new System.Windows.Point(4, 4);

            Room oGreatEastRoad2 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad1, oGreatEastRoad2, BidirectionalExitType.WestEast);
            AddGalbasiDowns(oGreatEastRoad2);
            breeToImladrisGraph.Rooms[oGreatEastRoad2] = new System.Windows.Point(5, 4);

            Room oGreatEastRoad3 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad2, oGreatEastRoad3, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad3] = new System.Windows.Point(6, 4);

            Room oGreatEastRoad4 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad3, oGreatEastRoad4, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad4] = new System.Windows.Point(7, 4);

            Room oGreatEastRoad5 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad4, oGreatEastRoad5, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad5] = new System.Windows.Point(8, 4);

            Room oGreatEastRoad6 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad5, oGreatEastRoad6, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad6] = new System.Windows.Point(9, 4);

            Room oGreatEastRoadGoblinAmbushGobLrgLrg = AddRoom("Gob Ambush #1");
            oGreatEastRoadGoblinAmbushGobLrgLrg.Mob1 = "goblin";
            oGreatEastRoadGoblinAmbushGobLrgLrg.Experience1 = 50;
            oGreatEastRoadGoblinAmbushGobLrgLrg.Experience2 = 50;
            oGreatEastRoadGoblinAmbushGobLrgLrg.Experience3 = 45;
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad6, BidirectionalExitType.SouthwestNortheast);
            breeToImladrisGraph.Rooms[oGreatEastRoadGoblinAmbushGobLrgLrg] = new System.Windows.Point(10, 3);

            Room oGreatEastRoad8 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad8, BidirectionalExitType.SoutheastNorthwest);
            breeToImladrisGraph.Rooms[oGreatEastRoad8] = new System.Windows.Point(11, 4);

            Room oGreatEastRoad9 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad8, oGreatEastRoad9, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad9] = new System.Windows.Point(12, 4);

            Room oGreatEastRoad10 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad9, oGreatEastRoad10, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad10] = new System.Windows.Point(13, 4);

            Room oGreatEastRoad11 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad10, oGreatEastRoad11, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad11] = new System.Windows.Point(14, 4);

            Room oGreatEastRoad12 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad11, oGreatEastRoad12, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad12] = new System.Windows.Point(15, 4);

            Room oGreatEastRoad13 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad12, oGreatEastRoad13, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad13] = new System.Windows.Point(16, 4);

            Room oGreatEastRoad14 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad13, oGreatEastRoad14, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oGreatEastRoad14] = new System.Windows.Point(17, 4);

            _imladrisWestGateOutside = _imladrisWestGateOutside = AddRoom("West Gate Outside");
            AddBidirectionalExits(oGreatEastRoad14, _imladrisWestGateOutside, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[_imladrisWestGateOutside] = new System.Windows.Point(18, 4);

            Room oNorthBrethilForest1 = AddRoom("North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oGreatEastRoadGoblinAmbushGobLrgLrg, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest1] = new System.Windows.Point(10, 2);

            Room oNorthBrethilForest2 = AddRoom("North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oNorthBrethilForest2, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest2] = new System.Windows.Point(11, 2);

            Room oDarkFootpath = AddRoom("Dark Footpath");
            AddBidirectionalExits(oDarkFootpath, oGreatEastRoad10, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthBrethilForest2, oDarkFootpath, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oDarkFootpath] = new System.Windows.Point(12, 2);

            //North Brethil Forest
            Room oNorthBrethilForest3 = AddRoom("North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest3, oDarkFootpath, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest3] = new System.Windows.Point(12, 1);

            Room oNorthBrethilForest4GobAmbushThreshold = AddRoom("Gob Ambush #2 Threshold");
            oNorthBrethilForest4GobAmbushThreshold.Mob1 = "goblin";
            AddBidirectionalExits(oNorthBrethilForest4GobAmbushThreshold, oNorthBrethilForest3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oNorthBrethilForest3] = new System.Windows.Point(12, 0);

            Room oNorthBrethilForest5GobAmbush = AddRoom("Gob Ambush #2");
            oNorthBrethilForest5GobAmbush.Mob1 = "goblin";
            oNorthBrethilForest5GobAmbush.Experience1 = 70;
            oNorthBrethilForest5GobAmbush.Experience2 = 50;
            oNorthBrethilForest5GobAmbush.Experience3 = 50;
            AddBidirectionalExits(oNorthBrethilForest4GobAmbushThreshold, oNorthBrethilForest5GobAmbush, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oNorthBrethilForest5GobAmbush] = new System.Windows.Point(12, 0);

            //South Brethil Forest
            Room oDeepForest = AddRoom("Deep Forest");
            AddBidirectionalExits(oGreatEastRoad9, oDeepForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oDeepForest] = new System.Windows.Point(12, 5);

            Room oBrethilForest = AddRoom("Brethil Forest");
            AddBidirectionalExits(oDeepForest, oBrethilForest, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oBrethilForest] = new System.Windows.Point(12, 6);

            Room oSpriteGuards = AddRoom("Sprite Guards");
            oSpriteGuards.Mob1 = "guard";
            oSpriteGuards.Experience1 = 120;
            oSpriteGuards.Experience2 = 120;
            oSpriteGuards.Alignment = AlignmentType.Blue;
            AddExit(oBrethilForest, oSpriteGuards, "brush");
            AddExit(oSpriteGuards, oBrethilForest, "east");
            breeToImladrisGraph.Rooms[oSpriteGuards] = new System.Windows.Point(11, 6);

            AddLocation(_aBreePerms, oGreatEastRoadGoblinAmbushGobLrgLrg);
            AddLocation(_aBreePerms, oNorthBrethilForest5GobAmbush);
            AddLocation(_aBreePerms, oSpriteGuards);
            AddLocation(_aMisc, _breeEastGateOutside);
        }

        private void AddToFarmHouseAndUglies(Room oGreatEastRoad1, out Room oOuthouse, RoomGraph breeToImladrisGraph, int level)
        {
            Room oRoadToFarm1 = AddRoom("Farmland");
            AddBidirectionalExits(oGreatEastRoad1, oRoadToFarm1, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm1] = new System.Windows.Point(4, 5);

            Room oRoadToFarm2 = AddRoom("Farmland");
            AddBidirectionalExits(oRoadToFarm1, oRoadToFarm2, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm2] = new System.Windows.Point(4, 6);

            Room oRoadToFarm3 = AddRoom("Farmland");
            AddBidirectionalExits(oRoadToFarm2, oRoadToFarm3, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm3] = new System.Windows.Point(4, 7);

            Room oRoadToFarm4 = AddRoom("Farmland");
            AddBidirectionalExits(oRoadToFarm3, oRoadToFarm4, BidirectionalExitType.NorthSouth);
            breeToImladrisGraph.Rooms[oRoadToFarm4] = new System.Windows.Point(4, 8);

            Room oRoadToFarm5 = AddRoom("Path to Ranch House");
            AddBidirectionalExits(oRoadToFarm5, oRoadToFarm4, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oRoadToFarm5] = new System.Windows.Point(3, 8);

            Room oRoadToFarm6 = AddRoom("Ranch House Front Steps");
            AddBidirectionalExits(oRoadToFarm6, oRoadToFarm5, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oRoadToFarm6] = new System.Windows.Point(2, 8);

            oOuthouse = AddRoom("Outhouse");
            AddBidirectionalExits(oRoadToFarm4, oOuthouse, BidirectionalExitType.WestEast);
            breeToImladrisGraph.Rooms[oOuthouse] = new System.Windows.Point(5, 8);

            Room oSwimmingPond = AddRoom("Swimming Pond");
            AddExit(oOuthouse, oSwimmingPond, "pond");
            AddExit(oSwimmingPond, oOuthouse, "west");
            breeToImladrisGraph.Rooms[oSwimmingPond] = new System.Windows.Point(6, 8);

            Room oMuddyPath = AddRoom("Muddy Path");
            AddExit(oSwimmingPond, oMuddyPath, "path");
            AddExit(oMuddyPath, oSwimmingPond, "pond");

            Room oSmallPlayground = AddRoom("Small Playground");
            AddBidirectionalExits(oSmallPlayground, oMuddyPath, BidirectionalExitType.SouthwestNortheast);

            Room oUglyKidSchoolEntrance = AddRoom("Ugly Kid School Entrance");
            AddBidirectionalSameNameExit(oSmallPlayground, oUglyKidSchoolEntrance, "gate");

            Room oMuddyFoyer = AddRoom("Muddy Foyer");
            if (level < 11)
            {
                AddExit(oUglyKidSchoolEntrance, oMuddyFoyer, "front");
            }
            AddExit(oMuddyFoyer, oUglyKidSchoolEntrance, "out");

            Room oUglyKidClassroomK7 = AddRoom("Ugly Kid Classroom K-7");
            AddExit(oMuddyFoyer, oUglyKidClassroomK7, "classroom");
            AddExit(oUglyKidClassroomK7, oMuddyFoyer, "foyer");

            Room oHallway = AddRoom("Hallway");
            AddExit(oUglyKidClassroomK7, oHallway, "hallway");
            AddExit(oHallway, oUglyKidClassroomK7, "classroom");

            Room oRoadToFarm7HoundDog = AddRoom("Hound Dog");
            oRoadToFarm7HoundDog.Mob1 = "dog";
            oRoadToFarm7HoundDog.Experience1 = 150;
            oRoadToFarm7HoundDog.Alignment = AlignmentType.Blue;
            AddExit(oRoadToFarm7HoundDog, oRoadToFarm6, "out");
            AddExit(oRoadToFarm6, oRoadToFarm7HoundDog, "porch");
            breeToImladrisGraph.Rooms[oRoadToFarm7HoundDog] = new System.Windows.Point(2, 7);

            Room oFarmParlorManagerMulloyThreshold = AddRoom("Farm Parlor");
            oFarmParlorManagerMulloyThreshold.Mob1 = "manager";
            AddBidirectionalSameNameExit(oFarmParlorManagerMulloyThreshold, oRoadToFarm7HoundDog, "door", "open door");
            breeToImladrisGraph.Rooms[oFarmParlorManagerMulloyThreshold] = new System.Windows.Point(2, 6);
            Room oManagerMulloy = AddRoom("Manager Mulloy");
            oManagerMulloy.Mob1 = "manager";
            oManagerMulloy.Experience1 = 600;
            oManagerMulloy.Alignment = AlignmentType.Blue;
            AddExit(oFarmParlorManagerMulloyThreshold, oManagerMulloy, "study");
            AddExit(oManagerMulloy, oFarmParlorManagerMulloyThreshold, "out");
            breeToImladrisGraph.Rooms[oManagerMulloy] = new System.Windows.Point(2, 5);

            Room oFarmKitchen = AddRoom("Kitchen");
            AddExit(oFarmParlorManagerMulloyThreshold, oFarmKitchen, "kitchen");
            AddExit(oFarmKitchen, oFarmParlorManagerMulloyThreshold, "parlor");
            breeToImladrisGraph.Rooms[oFarmKitchen] = new System.Windows.Point(1, 5);

            Room oFarmBackPorch = AddRoom("Back Porch");
            AddExit(oFarmKitchen, oFarmBackPorch, "backdoor");
            AddExit(oFarmBackPorch, oFarmKitchen, "kitchen");
            breeToImladrisGraph.Rooms[oFarmBackPorch] = new System.Windows.Point(1, 6);

            Room oFarmCat = AddRoom("Farm Cat");
            oFarmCat.Mob1 = "cat";
            oFarmCat.Experience1 = 550;
            AddExit(oFarmBackPorch, oFarmCat, "woodshed");
            Exit e = AddExit(oFarmCat, oFarmBackPorch, "out");
            e.NoFlee = true;
            breeToImladrisGraph.Rooms[oFarmCat] = new System.Windows.Point(1, 7);

            Room oCrabbe = AddRoom("Crabbe");
            oCrabbe.Mob1 = "Crabbe";
            oCrabbe.Experience1 = 250;
            AddExit(oHallway, oCrabbe, "detention");
            AddExit(oCrabbe, oHallway, "out");

            Room oMrWartnose = AddRoom("Mr. Wartnose");
            oMrWartnose.Mob1 = "Wartnose";
            oMrWartnose.Experience1 = 235;
            AddExit(oUglyKidClassroomK7, oMrWartnose, "office");
            AddExit(oMrWartnose, oUglyKidClassroomK7, "out");

            AddLocation(_aBreePerms, oRoadToFarm7HoundDog);
            AddLocation(_aBreePerms, oManagerMulloy);
            AddLocation(_aBreePerms, oFarmCat);
            AddLocation(_aInaccessible, oMrWartnose);
            AddLocation(_aInaccessible, oCrabbe);
        }

        private void AddGalbasiDowns(Room oGreatEastRoad2)
        {
            Room oGalbasiDownsEntrance = AddRoom("Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsEntrance, oGreatEastRoad2, BidirectionalExitType.NorthSouth);

            Room oGalbasiDownsNorth = AddRoom("Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsEntrance, BidirectionalExitType.NorthSouth);

            Room oGalbasiDownsNortheast = AddRoom("Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsNortheast, BidirectionalExitType.WestEast);

            Room oGalbasiDownsFurthestNorth = AddRoom("Galbasi Downs End");
            AddExit(oGalbasiDownsFurthestNorth, oGalbasiDownsNortheast, "southeast");
            Exit e = AddExit(oGalbasiDownsNortheast, oGalbasiDownsFurthestNorth, "northwest");
            e.Hidden = true;

            AddLocation(_aMisc, oGalbasiDownsFurthestNorth);
        }

        private void AddImladrisCity(out Room oImladrisSouthGateInside)
        {
            RoomGraph imladrisGraph = new RoomGraph("Imladris");
            imladrisGraph.ScalingFactor = 100;
            _graphs[MapType.Imladris] = imladrisGraph;

            _imladrisWestGateInside = AddRoom("West Gate Inside");
            AddExit(_imladrisWestGateInside, _imladrisWestGateOutside, "gate");
            imladrisGraph.Rooms[_imladrisWestGateOutside] = new System.Windows.Point(-1, 5);
            imladrisGraph.Rooms[_imladrisWestGateInside] = new System.Windows.Point(0, 5);

            Room oImladrisCircle1 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle1, _imladrisWestGateInside, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle1] = new System.Windows.Point(5D / 3, 5 - (4D / 3));

            Room oImladrisCircle2 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle2, oImladrisCircle1, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle2] = new System.Windows.Point(10D / 3, 5 - (8D / 3));

            Room oImladrisCircle3 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle2, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle3] = new System.Windows.Point(5, 1);

            Room oImladrisCircle4 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle4, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle4] = new System.Windows.Point(5 + (4D / 3), 1 + (4D / 3));

            Room oImladrisCircle5 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle4, oImladrisCircle5, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle5] = new System.Windows.Point(5 + (8D / 3), 1 + (8D / 3));

            Room oImladrisMainStreet1 = AddRoom("Main");
            AddBidirectionalExits(_imladrisWestGateInside, oImladrisMainStreet1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet1] = new System.Windows.Point(1, 5);

            Room oImladrisMainStreet2 = AddRoom("Main");
            AddBidirectionalExits(oImladrisMainStreet1, oImladrisMainStreet2, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet2] = new System.Windows.Point(2, 5);

            Room oImladrisMainStreet3 = AddRoom("Main");
            AddBidirectionalExits(oImladrisMainStreet2, oImladrisMainStreet3, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet3] = new System.Windows.Point(3, 5);

            Room oImladrisMainStreet4 = AddRoom("Main");
            AddBidirectionalExits(oImladrisMainStreet3, oImladrisMainStreet4, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet4] = new System.Windows.Point(4, 5);

            Room oImladrisMainStreet5 = AddRoom("Main");
            AddBidirectionalExits(oImladrisMainStreet4, oImladrisMainStreet5, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet5] = new System.Windows.Point(5, 5);

            Room oImladrisAlley3 = AddRoom("Side Alley");
            AddBidirectionalExits(oImladrisMainStreet5, oImladrisAlley3, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisAlley3] = new System.Windows.Point(6, 5);

            Room oImladrisAlley4 = AddRoom("Side Alley");
            AddBidirectionalExits(oImladrisAlley3, oImladrisAlley4, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisAlley4] = new System.Windows.Point(6, 6);

            Room oImladrisAlley5 = AddRoom("Side Alley");
            AddBidirectionalExits(oImladrisAlley4, oImladrisAlley5, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisAlley5] = new System.Windows.Point(6, 7);

            Room oImladrisSmallAlley1 = AddRoom("Small Alley");
            AddExit(oImladrisAlley3, oImladrisSmallAlley1, "alley");
            AddExit(oImladrisSmallAlley1, oImladrisAlley3, "south");
            imladrisGraph.Rooms[oImladrisSmallAlley1] = new System.Windows.Point(6, 4);

            Room oImladrisSmallAlley2 = AddRoom("Small Alley");
            AddBidirectionalExits(oImladrisSmallAlley2, oImladrisSmallAlley1, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisSmallAlley2] = new System.Windows.Point(6, 3);

            Room oImladrisPawnShop = AddRoom("Sharkey's Pawn Shop");
            AddBidirectionalSameNameExit(oImladrisPawnShop, oImladrisSmallAlley2, "door");
            imladrisGraph.Rooms[oImladrisPawnShop] = new System.Windows.Point(5, 3);

            Room oImladrisTownCircle = AddRoom("Town Circle");
            AddBidirectionalExits(oImladrisAlley3, oImladrisTownCircle, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisTownCircle] = new System.Windows.Point(7, 5);

            Room oImladrisMainStreet6 = AddRoom("Main");
            AddBidirectionalExits(oImladrisTownCircle, oImladrisMainStreet6, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oImladrisMainStreet6] = new System.Windows.Point(8, 5);

            Room oEastGateOfImladrisInside = AddRoom("East Gate Inside");
            AddBidirectionalExits(oImladrisCircle5, oEastGateOfImladrisInside, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisMainStreet6, oEastGateOfImladrisInside, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oEastGateOfImladrisInside] = new System.Windows.Point(9, 5);

            Room oEastGateOfImladrisOutside = AddRoom("East Gate Outside");
            AddBidirectionalSameNameExit(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, "gate");
            imladrisGraph.Rooms[oEastGateOfImladrisOutside] = new System.Windows.Point(10, 5);

            Room oImladrisCircle6 = AddRoom("Circle");
            AddBidirectionalExits(oEastGateOfImladrisInside, oImladrisCircle6, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle6] = new System.Windows.Point(9 - (4D / 3), 5 + (4D / 3));

            Room oImladrisCircle7 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle6, oImladrisCircle7, BidirectionalExitType.SouthwestNortheast);
            imladrisGraph.Rooms[oImladrisCircle7] = new System.Windows.Point(9 - (8D / 3), 5 + (8D / 3));

            Room oImladrisCircle10 = AddRoom("Circle");
            AddBidirectionalExits(_imladrisWestGateInside, oImladrisCircle10, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle10] = new System.Windows.Point(5 - (10D / 3), 9 - (8D / 3));

            Room oImladrisCircle9 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle10, oImladrisCircle9, BidirectionalExitType.SoutheastNorthwest);
            imladrisGraph.Rooms[oImladrisCircle9] = new System.Windows.Point(5 - (5D / 3), 9 - (4D / 3));

            Room oImladrisCircle8 = AddRoom("Circle");
            AddBidirectionalExits(oImladrisCircle9, oImladrisCircle8, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisCircle7, oImladrisCircle8, BidirectionalExitType.SouthwestNortheast);
            AddExit(oImladrisAlley5, oImladrisCircle8, "south");
            imladrisGraph.Rooms[oImladrisCircle8] = new System.Windows.Point(5, 9);

            Room oRearAlley = AddRoom("Rear Alley");
            AddBidirectionalExits(oImladrisCircle10, oRearAlley, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oRearAlley, oImladrisAlley5, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oRearAlley] = new System.Windows.Point(5, 7);

            Room oPoisonedDagger = AddRoom("Master Assassins");
            oPoisonedDagger.Mob1 = oRearAlley.Mob1 = "assassin";
            oPoisonedDagger.Experience1 = 600;
            oPoisonedDagger.Experience2 = 600;
            AddBidirectionalSameNameExit(oRearAlley, oPoisonedDagger, "door");
            imladrisGraph.Rooms[oPoisonedDagger] = new System.Windows.Point(5, 6.5);

            oImladrisSouthGateInside = AddRoom("South Gate Inside");
            AddBidirectionalExits(oImladrisCircle8, oImladrisSouthGateInside, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisSouthGateInside] = new System.Windows.Point(5, 10);

            Room oImladrisCityDump = AddRoom("City Dump");
            Exit e = AddExit(oImladrisCircle8, oImladrisCityDump, "north");
            e.OmitGo = true;
            AddExit(oImladrisCityDump, oImladrisCircle8, "south");
            e = AddExit(oImladrisCityDump, oRearAlley, "north");
            e.Hidden = true;
            imladrisGraph.Rooms[oImladrisCityDump] = new System.Windows.Point(5, 8);

            Room oImladrisHealingHand = AddRoom("Healing Hand");
            oImladrisHealingHand.IsHealingRoom = true;
            AddBidirectionalExits(oImladrisHealingHand, oImladrisMainStreet5, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oImladrisHealingHand] = new System.Windows.Point(5, 4.5);

            Room oTyriesPriestSupplies = AddRoom("Tyrie's Priest Supplies");
            AddBidirectionalExits(oImladrisMainStreet5, oTyriesPriestSupplies, BidirectionalExitType.NorthSouth);
            imladrisGraph.Rooms[oTyriesPriestSupplies] = new System.Windows.Point(5, 5.5);

            Room oMountainPath1 = AddRoom("Mountain Path");
            AddBidirectionalExits(oEastGateOfImladrisOutside, oMountainPath1, BidirectionalExitType.WestEast);

            Room oMountainPath2 = AddRoom("Mountain Path");
            AddBidirectionalExits(oMountainPath2, oMountainPath1, BidirectionalExitType.SouthwestNortheast);

            Room oMountainTrail1 = AddRoom("Mountain Trail");
            AddBidirectionalExits(oMountainTrail1, oMountainPath2, BidirectionalExitType.NorthSouth);

            Room oMountainTrail2 = AddRoom("Mountain Trail");
            AddBidirectionalExits(oMountainTrail2, oMountainTrail1, BidirectionalExitType.SouthwestNortheast);

            Room oIorlas = AddRoom("Iorlas");
            oIorlas.Mob1 = "Iorlas";
            oIorlas.Experience1 = 200;
            oIorlas.Alignment = AlignmentType.Grey;
            AddExit(oMountainTrail2, oIorlas, "shack");
            AddExit(oIorlas, oMountainTrail2, "door");

            AddLocation(_aImladrisTharbadPerms, oImladrisHealingHand);
            AddLocation(_aImladrisTharbadPerms, oIorlas);
            AddLocation(_aImladrisTharbadPerms, oPoisonedDagger);
        }

        private void AddBreeToHobbiton(Room oBreeWestGateInside, Room oSmoulderingVillage, int level)
        {
            Room oBreeWestGateOutside = AddRoom("West Gate Outside");
            AddBidirectionalSameNameExit(oBreeWestGateInside, oBreeWestGateOutside, "gate");
            _breeStreetsGraph.Rooms[oBreeWestGateOutside] = new System.Windows.Point(-1, 3);

            Room oLeviathanNorthForkWestern = AddRoom("The Grand Intersection - Leviathan Way/North Fork Road/Western Road");
            AddBidirectionalExits(oLeviathanNorthForkWestern, oBreeWestGateOutside, BidirectionalExitType.WestEast);

            Room oNorthFork1 = AddRoom("North Fork Road");
            AddBidirectionalExits(oNorthFork1, oLeviathanNorthForkWestern, BidirectionalExitType.SoutheastNorthwest);

            Room oWesternRoad1 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad1, oLeviathanNorthForkWestern, BidirectionalExitType.WestEast);

            Room oWesternRoad2 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad2, oWesternRoad1, BidirectionalExitType.WestEast);

            Room oWesternRoad3 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad3, oWesternRoad2, BidirectionalExitType.WestEast);

            Room oWesternRoad4 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad4, oWesternRoad3, BidirectionalExitType.WestEast);

            Room oWesternRoad5 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad5, oWesternRoad4, BidirectionalExitType.WestEast);

            Room oWesternRoad6 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad6, oWesternRoad5, BidirectionalExitType.WestEast);

            Room oWesternRoad7 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad7, oWesternRoad6, BidirectionalExitType.WestEast);

            Room oWesternRoad8 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad8, oWesternRoad7, BidirectionalExitType.WestEast);

            Room oWesternRoad9 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad9, oWesternRoad8, BidirectionalExitType.WestEast);

            Room oVillageOfHobbiton1 = AddRoom("Village of Hobbiton");
            AddBidirectionalExits(oVillageOfHobbiton1, oWesternRoad9, BidirectionalExitType.WestEast);

            Room oMainSquareOfHobbiton = AddRoom("Main Square of Hobbiton");
            AddBidirectionalExits(oMainSquareOfHobbiton, oVillageOfHobbiton1, BidirectionalExitType.WestEast);

            Room oVillageOfHobbiton2 = AddRoom("Village of Hobbiton");
            AddBidirectionalExits(oMainSquareOfHobbiton, oVillageOfHobbiton2, BidirectionalExitType.NorthSouth);

            Room oValleyRoad = AddRoom("Valley Road");
            AddBidirectionalExits(oVillageOfHobbiton2, oValleyRoad, BidirectionalExitType.NorthSouth);

            Room oHillAtBagEnd = AddRoom("Hill at Bag End");
            AddBidirectionalExits(oValleyRoad, oHillAtBagEnd, BidirectionalExitType.SoutheastNorthwest);

            Room oBilboFrodoHobbitHoleCondo = AddRoom("Bilbo/Frodo Hobbit Hole Condo");
            AddExit(oHillAtBagEnd, oBilboFrodoHobbitHoleCondo, "down");
            AddExit(oBilboFrodoHobbitHoleCondo, oHillAtBagEnd, "out");

            Room oBilboFrodoCommonArea = AddRoom("Common Area");
            AddBidirectionalSameNameExit(oBilboFrodoHobbitHoleCondo, oBilboFrodoCommonArea, "curtain");

            Room oEastwingHallway = AddRoom("Eastwing Hallway");
            AddExit(oBilboFrodoCommonArea, oEastwingHallway, "eastwing");
            AddExit(oEastwingHallway, oBilboFrodoCommonArea, "common");

            Room oSouthwingHallway = AddRoom("Southwing Hallway");
            AddExit(oEastwingHallway, oSouthwingHallway, "southwing");
            AddExit(oSouthwingHallway, oEastwingHallway, "eastwing");

            Room oBilboBaggins = AddRoom("Bilbo Baggins");
            oBilboBaggins.Mob1 = "Bilbo";
            oBilboBaggins.Experience1 = 260;
            oBilboBaggins.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oSouthwingHallway, oBilboBaggins, "oakdoor", "open oakdoor");

            Room oFrodoBaggins = AddRoom("Frodo Baggins");
            oFrodoBaggins.Mob1 = "Frodo";
            oFrodoBaggins.Experience1 = 260;
            oFrodoBaggins.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oSouthwingHallway, oFrodoBaggins, "curtain");

            Room oGreatHallOfHeroes = AddRoom("Great Hall of Heroes");
            AddExit(oGreatHallOfHeroes, oLeviathanNorthForkWestern, "out");
            AddExit(oLeviathanNorthForkWestern, oGreatHallOfHeroes, "hall");

            //something is hasted
            Room oSomething = AddRoom("Something");
            oSomething.Mob1 = "Something";
            oSomething.Experience1 = 140;
            if (level < 11)
            {
                Exit e = AddExit(oGreatHallOfHeroes, oSomething, "curtain");
                e.Hidden = true;
            }
            AddExit(oSomething, oGreatHallOfHeroes, "curtain");

            Room oShepherd = AddRoom("Shepherd");
            oShepherd.Mob1 = "Shepherd";
            oShepherd.Experience1 = 60;
            oShepherd.Alignment = AlignmentType.Blue;
            AddExit(oNorthFork1, oShepherd, "pasture");
            AddExit(oShepherd, oNorthFork1, "south");

            //Gate is locked (and knocking doesn't work) so not treating as an exit. This is only accessible from the other way around.
            //AddExit(oShepherd, oSmoulderingVillage, "gate");
            AddExit(oSmoulderingVillage, oShepherd, "gate");

            AddLocation(_aInaccessible, oSomething);
            AddLocation(_aBreePerms, oBilboBaggins);
            AddLocation(_aBreePerms, oFrodoBaggins);
            AddLocation(_aBreePerms, oShepherd);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside, out Room oTharbadGateOutside)
        {
            RoomGraph imladrisGraph = _graphs[MapType.Imladris];

            RoomGraph imladrisToTharbadGraph = new RoomGraph("Imladris/Tharbad");
            imladrisToTharbadGraph.ScalingFactor = 100;
            _graphs[MapType.ImladrisToTharbad] = imladrisToTharbadGraph;

            Room oMistyTrail1 = AddRoom("South Gate Outside");
            AddBidirectionalSameNameExit(oImladrisSouthGateInside, oMistyTrail1, "gate");
            imladrisGraph.Rooms[oMistyTrail1] = new System.Windows.Point(5, 11);
            imladrisToTharbadGraph.Rooms[oMistyTrail1] = new System.Windows.Point(5, 0);

            Room oBrunskidTradersGuild1 = AddRoom("Brunskid Guild");
            AddBidirectionalExits(oBrunskidTradersGuild1, oMistyTrail1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oBrunskidTradersGuild1] = new System.Windows.Point(4, 11);
            imladrisToTharbadGraph.Rooms[oBrunskidTradersGuild1] = new System.Windows.Point(4, 0);

            Room oCutthroatAssassinThreshold = AddRoom("Guildmaster");
            AddBidirectionalExits(oCutthroatAssassinThreshold, oBrunskidTradersGuild1, BidirectionalExitType.WestEast);
            imladrisGraph.Rooms[oCutthroatAssassinThreshold] = new System.Windows.Point(3, 11);
            imladrisToTharbadGraph.Rooms[oCutthroatAssassinThreshold] = new System.Windows.Point(3, 0);

            Room oCutthroatAssassin = AddRoom("Hiester");
            AddBidirectionalExits(oCutthroatAssassin, oCutthroatAssassinThreshold, BidirectionalExitType.WestEast);
            oCutthroatAssassin.Mob1 = "hiester";
            oCutthroatAssassin.Mob2 = "assassin";
            oCutthroatAssassin.Mob3 = "cutthroat";
            oCutthroatAssassin.Experience1 = 1200;
            oCutthroatAssassin.Experience2 = 600;
            oCutthroatAssassin.Experience3 = 500;
            imladrisGraph.Rooms[oCutthroatAssassin] = new System.Windows.Point(2, 11);
            imladrisToTharbadGraph.Rooms[oCutthroatAssassin] = new System.Windows.Point(2, 0);

            Room oMistyTrail2 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail1, oMistyTrail2, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail2] = new System.Windows.Point(5, 1);

            Room oMistyTrail3 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail2, oMistyTrail3, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail3] = new System.Windows.Point(5, 2);

            Room oMistyTrail4 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail3, oMistyTrail4, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail4] = new System.Windows.Point(4, 3);

            Room oPotionFactoryReception = AddRoom("Potion Factory Guard");
            AddBidirectionalExits(oPotionFactoryReception, oMistyTrail4, BidirectionalExitType.WestEast);
            oPotionFactoryReception.Mob1 = "Guard";
            oPotionFactoryReception.Experience1 = 110;
            imladrisToTharbadGraph.Rooms[oPotionFactoryReception] = new System.Windows.Point(3, 3);

            Room oPotionFactoryAdministrativeOffices = AddRoom("Potion Factory Administrative Offices");
            AddBidirectionalExits(oPotionFactoryReception, oPotionFactoryAdministrativeOffices, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oPotionFactoryAdministrativeOffices] = new System.Windows.Point(3, 4);

            Room oMarkFrey = AddRoom("Mark Frey");
            oMarkFrey.Mob1 = "Frey";
            oMarkFrey.Experience1 = 450;
            AddExit(oPotionFactoryAdministrativeOffices, oMarkFrey, "door");
            AddExit(oMarkFrey, oPotionFactoryAdministrativeOffices, "out");
            imladrisToTharbadGraph.Rooms[oMarkFrey] = new System.Windows.Point(3, 5);

            Room oMistyTrail5 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail4, oMistyTrail5, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail5] = new System.Windows.Point(4, 4);

            Room oMistyTrail6 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail5, oMistyTrail6, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail6] = new System.Windows.Point(4, 5);

            Room oMistyTrail7 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail6, oMistyTrail7, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail7] = new System.Windows.Point(4, 6);

            Room oMistyTrail8 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail7, oMistyTrail8, BidirectionalExitType.NorthSouth);
            AddShantyTown(oMistyTrail8);
            imladrisToTharbadGraph.Rooms[oMistyTrail8] = new System.Windows.Point(4, 7);

            Room oMistyTrail9 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail8, oMistyTrail9, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail9] = new System.Windows.Point(4, 8);

            Room oMistyTrail10 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail9, oMistyTrail10, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail10] = new System.Windows.Point(3, 9);

            Room oMistyTrail11 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail10, oMistyTrail11, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail11] = new System.Windows.Point(2, 10);

            Room oMistyTrail12 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail11, oMistyTrail12, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oMistyTrail12] = new System.Windows.Point(2, 11);

            Room oMistyTrail13 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail12, oMistyTrail13, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail13] = new System.Windows.Point(1, 12);

            Room oMistyTrail14 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail13, oMistyTrail14, BidirectionalExitType.SouthwestNortheast);
            imladrisToTharbadGraph.Rooms[oMistyTrail14] = new System.Windows.Point(0, 13);

            oTharbadGateOutside = AddRoom("North Gate");
            AddBidirectionalExits(oMistyTrail14, oTharbadGateOutside, BidirectionalExitType.NorthSouth);
            imladrisToTharbadGraph.Rooms[oTharbadGateOutside] = new System.Windows.Point(0, 14);

            AddLocation(_aImladrisTharbadPerms, oCutthroatAssassin);
            AddLocation(_aImladrisTharbadPerms, oMarkFrey);
        }

        private void AddShantyTown(Room oMistyTrail8)
        {
            RoomGraph imladrisToTharbadGraph = _graphs[MapType.ImladrisToTharbad];

            RoomGraph oShantyTownGraph = new RoomGraph("Shanty Town");
            oShantyTownGraph.ScalingFactor = 100;
            _graphs[MapType.ShantyTown] = oShantyTownGraph;

            oShantyTownGraph.Rooms[oMistyTrail8] = new System.Windows.Point(5, 0);

            Room oRuttedDirtRoad = AddRoom("Dirt Road");
            AddBidirectionalExits(oRuttedDirtRoad, oMistyTrail8, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oRuttedDirtRoad] = new System.Windows.Point(4, 0);
            imladrisToTharbadGraph.Rooms[oRuttedDirtRoad] = new System.Windows.Point(3, 7);

            Room oHouseOfPleasure = AddRoom("House of Pleasure");
            AddBidirectionalSameNameExit(oRuttedDirtRoad, oHouseOfPleasure, "door");
            oShantyTownGraph.Rooms[oHouseOfPleasure] = new System.Windows.Point(4, -1);

            Room oNorthEdgeOfShantyTown = AddRoom("Shanty Town");
            AddBidirectionalExits(oRuttedDirtRoad, oNorthEdgeOfShantyTown, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oNorthEdgeOfShantyTown] = new System.Windows.Point(4, 1);

            Room oRedFoxLane = AddRoom("Red Fox");
            AddBidirectionalExits(oRedFoxLane, oNorthEdgeOfShantyTown, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oRedFoxLane] = new System.Windows.Point(3, 1);

            Room oGypsyCamp = AddRoom("Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp, oRedFoxLane, BidirectionalExitType.SoutheastNorthwest);
            oShantyTownGraph.Rooms[oGypsyCamp] = new System.Windows.Point(2, 0);

            Room oGypsyCamp2 = AddRoom("Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp2, oGypsyCamp, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oGypsyCamp2] = new System.Windows.Point(1, 0);

            Room oMadameProkawskiPalmReadingService = AddRoom("Palm Reading Service");
            AddExit(oGypsyCamp2, oMadameProkawskiPalmReadingService, "wagon");
            AddExit(oMadameProkawskiPalmReadingService, oGypsyCamp2, "out");
            oShantyTownGraph.Rooms[oMadameProkawskiPalmReadingService] = new System.Windows.Point(1, -1);

            Room oGypsyAnimalKeep = AddRoom("Gypsy Animal Keep");
            AddBidirectionalSameNameExit(oGypsyCamp2, oGypsyAnimalKeep, "gate");
            oShantyTownGraph.Rooms[oGypsyAnimalKeep] = new System.Windows.Point(0, 0);

            Room oExoticAnimalKeep = AddRoom("Exotic Animal Wagon");
            AddExit(oGypsyAnimalKeep, oExoticAnimalKeep, "wagon");
            AddExit(oExoticAnimalKeep, oGypsyAnimalKeep, "out");
            oShantyTownGraph.Rooms[oExoticAnimalKeep] = new System.Windows.Point(-1, 0);

            Room oNorthShantyTown = AddRoom("Shanty Town");
            AddBidirectionalExits(oRedFoxLane, oNorthShantyTown, BidirectionalExitType.SouthwestNortheast);
            oShantyTownGraph.Rooms[oNorthShantyTown] = new System.Windows.Point(2, 2);

            Room oShantyTownDump = AddRoom("Town Dump");
            AddBidirectionalExits(oNorthShantyTown, oShantyTownDump, BidirectionalExitType.SouthwestNortheast);
            oShantyTownGraph.Rooms[oShantyTownDump] = new System.Windows.Point(1, 3);

            Room oShantyTownWest = AddRoom("Shanty Town");
            AddBidirectionalExits(oShantyTownDump, oShantyTownWest, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTownWest] = new System.Windows.Point(1, 4);

            Room oCopseOfTrees = AddRoom("Copse of Trees");
            AddBidirectionalExits(oShantyTownWest, oCopseOfTrees, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oCopseOfTrees] = new System.Windows.Point(1, 5);

            Room oBluff = AddRoom("Bluff");
            AddBidirectionalExits(oCopseOfTrees, oBluff, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oBluff] = new System.Windows.Point(1, 6);

            Room oShantyTown1 = AddRoom("Shanty Town");
            AddBidirectionalExits(oNorthEdgeOfShantyTown, oShantyTown1, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTown1] = new System.Windows.Point(4, 2);

            Room oShantyTown2 = AddRoom("Shanty Town");
            AddBidirectionalExits(oShantyTown1, oShantyTown2, BidirectionalExitType.NorthSouth);
            oShantyTownGraph.Rooms[oShantyTown2] = new System.Windows.Point(4, 3);

            Room oShantyTown3 = AddRoom("Shanty Town");
            AddBidirectionalExits(oShantyTown2, oShantyTown3, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oBluff, oShantyTown3, BidirectionalExitType.WestEast);
            oShantyTownGraph.Rooms[oShantyTown3] = new System.Windows.Point(3, 6);

            Room oPentampurisPalace = AddRoom("Pentampuri's Palace");
            AddExit(oShantyTown3, oPentampurisPalace, "shack");
            AddExit(oPentampurisPalace, oShantyTown3, "out");
            oShantyTownGraph.Rooms[oPentampurisPalace] = new System.Windows.Point(4, 6);

            Room oPrinceBrunden = AddRoom("Prince Brunden");
            oPrinceBrunden.Mob1 = "Prince";
            oPrinceBrunden.Experience1 = 150;
            oPrinceBrunden.Alignment = AlignmentType.Blue;
            AddExit(oGypsyCamp, oPrinceBrunden, "wagon");
            AddExit(oPrinceBrunden, oGypsyCamp, "out");
            oShantyTownGraph.Rooms[oPrinceBrunden] = new System.Windows.Point(2, -1);

            Room oNaugrim = AddRoom("Naugrim");
            oNaugrim.Mob1 = "Naugrim";
            oNaugrim.Experience1 = 205;
            oNaugrim.Alignment = AlignmentType.Red;
            AddExit(oNorthShantyTown, oNaugrim, "cask");
            AddExit(oNaugrim, oNorthShantyTown, "out");
            oShantyTownGraph.Rooms[oNaugrim] = new System.Windows.Point(1, 1);

            Room oHogoth = AddRoom("Hogoth");
            oHogoth.Mob1 = "Hogoth";
            oHogoth.Experience1 = 260;
            oHogoth.Alignment = AlignmentType.Blue;
            AddExit(oShantyTownWest, oHogoth, "shack");
            AddExit(oHogoth, oShantyTownWest, "out");
            oShantyTownGraph.Rooms[oHogoth] = new System.Windows.Point(0, 4);

            Room oFaornil = AddRoom("Faornil");
            oFaornil.Mob1 = "Faornil";
            oFaornil.Experience1 = 250;
            oFaornil.Alignment = AlignmentType.Red;
            AddExit(oShantyTown1, oFaornil, "tent");
            AddExit(oFaornil, oShantyTown1, "out");
            oShantyTownGraph.Rooms[oFaornil] = new System.Windows.Point(5, 2);

            Room oGraddy = AddRoom("Graddy");
            oGraddy.Mob1 = "Graddy";
            oGraddy.Experience1 = 350;
            AddExit(oShantyTown2, oGraddy, "wagon");
            AddExit(oGraddy, oShantyTown2, "out");
            oShantyTownGraph.Rooms[oGraddy] = new System.Windows.Point(5, 3);

            Room oGraddyOgre = AddRoom("Ogre");
            oGraddyOgre.Mob1 = "Ogre";
            oGraddyOgre.Experience1 = 150;
            Exit e = AddExit(oGraddy, oGraddyOgre, "gate");
            e.PreCommand = "open gate";
            e = AddExit(oGraddyOgre, oGraddy, "gate");
            e.PreCommand = "open gate";
            oShantyTownGraph.Rooms[oGraddyOgre] = new System.Windows.Point(5, 4);

            AddLocation(_aImladrisTharbadPerms, oPrinceBrunden);
            AddLocation(_aImladrisTharbadPerms, oNaugrim);
            AddLocation(_aImladrisTharbadPerms, oHogoth);
            AddLocation(_aImladrisTharbadPerms, oFaornil);
            AddLocation(_aImladrisTharbadPerms, oGraddy);
        }

        private void AddIntangible(Room oBreeTownSquare)
        {
            Area oIntangible = _areasByName[AREA_INTANGIBLE];

            _treeOfLife = AddRoom("Tree of Life");
            AddExit(_treeOfLife, oBreeTownSquare, "down");

            Room oLimbo = AddRoom("Limbo");
            Exit e = AddExit(oLimbo, _treeOfLife, "green");
            e.PreCommand = "open green";

            AddLocation(oIntangible, _treeOfLife);
            AddLocation(oIntangible, oLimbo);
        }

        private Room AddRoom(string roomName)
        {
            Room r = new Room(roomName);
            _map.AddVertex(r);
            return r;
        }

        private void AddLocation(Area area, Room locRoom)
        {
            area.Locations.Add(locRoom);
        }

        private Exit AddExit(Room a, Room b, string exitText)
        {
            Exit e = new Exit(a, b, exitText);
            _map.AddEdge(e);
            return e;
        }

        private void AddBidirectionalSameNameExit(Room aRoom, Room bRoom, string exitText)
        {
            AddBidirectionalSameNameExit(aRoom, bRoom, exitText, null);
        }

        private void AddBidirectionalSameNameExit(Room aRoom, Room bRoom, string exitText, string preCommand)
        {
            Exit e = new Exit(aRoom, bRoom, exitText);
            e.PreCommand = preCommand;
            _map.AddEdge(e);
            e = new Exit(bRoom, aRoom, exitText);
            e.PreCommand = preCommand;
            _map.AddEdge(e);
        }

        private void AddBidirectionalExits(Room aRoom, Room bRoom, BidirectionalExitType exitType)
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
            _map.AddEdge(new Exit(aRoom, bRoom, exitAtoB));
            _map.AddEdge(new Exit(bRoom, aRoom, exitBtoA));
        }

        private Area AddArea(string areaName)
        {
            Area a = new Area(areaName);
            _areas.Add(a);
            _areasByName[a.Name] = a;
            return a;
        }

        public void SetNightEdges(bool isNight)
        {
            foreach (Exit e in _nightEdges)
            {
                _map.RemoveEdge(e);
            }
            if (!isNight)
            {
                _nightEdges.Add(AddExit(_breeEastGateOutside, _breeEastGateInside, "gate"));
                _nightEdges.Add(AddExit(_imladrisWestGateOutside, _imladrisWestGateInside, "gate"));
            }
        }

        public void SetCelduinExpressEdges(string selectedItem)
        {
            foreach (Exit e in _celduinExpressEdges)
            {
                _map.RemoveEdge(e);
            }
            if (selectedItem == "Bree")
            {
                _celduinExpressEdges.Add(AddExit(_breeDocks, _boatswain, "steamboat"));
                _celduinExpressEdges.Add(AddExit(_boatswain, _breeDocks, "dock"));
            }
        }
    }

    internal enum BidirectionalExitType
    {
        WestEast,
        NorthSouth,
        SoutheastNorthwest,
        SouthwestNortheast,
        UpDown,
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

    internal enum AlignmentType
    {
        Blue,
        Grey,
        Red,
    }

    internal enum MapType
    {
        BreeStreets,
        BreeSewers,
        UnderBree,
        MillwoodMansion,
        MillwoodMansionUpstairs,
        BreeToImladris,
        Imladris,
        ImladrisToTharbad,
        ShantyTown,
        Tharbad
    }
}
