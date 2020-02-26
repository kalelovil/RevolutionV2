using Kalelovil.Revolution.Missions;
using Kalelovil.Revolution.Provinces;
using Kalelovil.Revolution.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WorldMapStrategyKit;

namespace Kalelovil.Revolution.Units
{
    public class Brigade : MonoBehaviour
    {
        public enum AgentType
        {
            Brigade,
            Saboteur,
            Diplomat,
        }

        [Header("Name")]
        [SerializeField] string _name;
        internal string Name => gameObject.name;//_name;


        #region Leader
        [Header("Leader")]
        [SerializeField] BrigadeLeader _leader;
        public BrigadeLeader Leader { get => _leader; set => _leader = value; }
        #endregion

        #region Units
        [Header("Units")]
        [SerializeField] List<BrigadeElement> _unitList;
        public List<BrigadeElement> UnitList { get => _unitList; set => _unitList = value; }
        #endregion

        #region Stats
        [Header("Stats")]
        [SerializeField] float _attack;
        public float Attack { get => _attack; }
        [SerializeField] float _health;
        public float Health { get => _health; }
        [SerializeField] float _speed;
        public float Speed { get => _speed; }
        #endregion

        [Header("WMSK Animator")]
        [SerializeField] GameObjectAnimator _goAnimator;
        public GameObjectAnimator GoAnimator { get => _goAnimator; private set => _goAnimator = value; }

        [Header("Line Animator")]
        [SerializeField] LineMarkerAnimator _lineAnimatorPrefab;
        [SerializeField] LineMarkerAnimator _lineAnimator;
        public LineMarkerAnimator LineAnimator { get => _lineAnimator; private set => _lineAnimator = value; }


        [Header("Destination Mission Type")]
        [SerializeField] MissionType _destinationMissionType;
        public MissionType DestinationMissionType { get => _destinationMissionType; set => _destinationMissionType = value; }
        [Header("Current Mission")]
        [SerializeField] MissionProgress _currentMission;
        public MissionProgress CurrentMission { get => _currentMission; set => _currentMission = value; }

        [Header("Visual")]
        [SerializeField] Renderer[] _renderers;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void Initialise(BrigadeLeader leader, Headquarters hq)
        {
            Vector2 position = hq.MountPoint.unity2DLocation;
            GoAnimator = gameObject.WMSK_MoveTo(position.x, position.y);
            GoAnimator.terrainCapability = TERRAIN_CAPABILITY.OnlyGround;
            GoAnimator.OnPointerDown += (GameObjectAnimator anim) => UnitClicked(anim);
            GoAnimator.OnMoveEnd += (GameObjectAnimator anim) => MoveEnded(anim);
            GoAnimator.OnProvinceRegionEnter += (GameObjectAnimator anim) => ProvinceRegionEntered(anim);

            // Ensure unit is limited terrain, avoid water
            GoAnimator.terrainCapability = TERRAIN_CAPABILITY.OnlyGround;
        }

        private void ProvinceRegionEntered(GameObjectAnimator anim)
        {
            var region = WMSK.instance.GetProvinceRegion(anim.currentMap2DLocation);
            Debug.LogWarning("Tank has entered province region " + region.entity.name + "{" + region.regionIndex + "}" + ".");
            SetMovementSpeed(region);
        }

        private void SetMovementSpeed(Region region)
        {
            var feature = Province_Manager.Instance.GetFeatureForRegion(region);
            GoAnimator._speedMultiplier = (feature == null) ? 1f : 1f / feature.Movement_Multiplier;

            if (GoAnimator.Route != null && GoAnimator.Route.Count > 0)
            {
                List<Vector2> remainingRoute = GetRemainingRoute(GoAnimator);
                remainingRoute.Insert(0, GoAnimator.currentMap2DLocation);
                MoveWithPathFinding(remainingRoute, false);
            }
        }

        private List<Vector2> GetRemainingRoute(GameObjectAnimator goAnimator)
        {
            int index = Mathf.CeilToInt((goAnimator.Route.Count - 1) * goAnimator.Progress);
            List<Vector2> fullRoute = goAnimator.Route;
            List<Vector2> remainingRoute = fullRoute.GetRange(index, fullRoute.Count - index);
            return remainingRoute;
        }

        private void UnitClicked(GameObjectAnimator anim)
        {
            Debug.Log($"Unit Clicked: {anim.gameObject.name}");
            Unit_Manager.Instance.SelectedUnit = (Unit_Manager.Instance.SelectedUnit == this) ? null : this;
        }


        /// <summary>
        /// Moves the unit with path finding.
        /// </summary>
        internal void MoveWithPathFinding(Vector2 destination, bool resetLine = true)
        {
            if (Speed > float.Epsilon)
            {
                List<Vector2> route = GoAnimator.FindRoute(destination);
                MoveWithPathFinding(route, resetLine);
            }
            else
            {
                Debug.LogWarning($"Unit ({Name}) Has 0 Speed: Cannot Give It A Movement Order");
            }
        }
        internal void MoveWithPathFinding(List<Vector2> route, bool resetLine = true)
        {
            if (Speed > float.Epsilon)
            {
                if (route.Count > 0)
                {
                    GoAnimator.MoveTo(route, 1e4f / Speed, DURATION_TYPE.MapLap);

                    if (resetLine)
                    {
                        // Remove existing line
                        if (LineAnimator)
                        {
                            DestroyImmediate(LineAnimator.gameObject);
                            LineAnimator = null;
                        }

                        // Add Line
                        LineAnimator = WMSK.instance.AddLine(
                            route.ToArray(), _lineAnimatorPrefab.color, _lineAnimatorPrefab.arcElevation, _lineAnimatorPrefab.lineWidth);
                        LineAnimator.reverseMode = _lineAnimatorPrefab.reverseMode;
                        LineAnimator.autoFadeAfter = _lineAnimatorPrefab.autoFadeAfter;
                        LineAnimator.dashAnimationDuration = _lineAnimatorPrefab.dashAnimationDuration;
                        LineAnimator.dashInterval = _lineAnimatorPrefab.dashInterval;
                        LineAnimator.drawingDuration = _lineAnimatorPrefab.drawingDuration;
                        LineAnimator.fadeOutDuration = _lineAnimatorPrefab.fadeOutDuration;
                    }
                }
            }
        }
        private void LateUpdate()
        {
            RemoveLineBehindUnit();
        }

        private void RemoveLineBehindUnit()
        {
            // TODO Examine performance impact
            if (GoAnimator.Route != null)
            {
                var routeIndex = Mathf.CeilToInt((GoAnimator.Route.Count - 1) * GoAnimator.Progress);
                var remainingPoints = GoAnimator.Route.Count - routeIndex;
                //Debug.Log($"Remaining Points: {remainingPoints}");
                var newLinePath = LineAnimator.path.ToList().GetRange(LineAnimator.path.Length - remainingPoints, remainingPoints);
                newLinePath.Insert(0, GoAnimator.currentMap2DLocation);
                LineAnimator.path = newLinePath.ToArray();
                LineAnimator.Start();
            }
        }

        private void MoveEnded(GameObjectAnimator anim)
        {
            Region region = WMSK.instance.GetProvinceRegion(anim.currentMap2DLocation);
            Debug.Log($"Unit Movement Ended: {anim.gameObject.name} In Region #{region.regionIndex} of Province {region.entity.name}");
        }
    }
}