using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Goui;
using Goui.Html;

namespace Goui.Plugin.Maps.Html {
    public class GoogleMap : Div {
        #region Classes and Enums
        /// <summary>
        /// Supported Map Types by the Google Maps API
        /// </summary>
        public enum MapType {
            /// <summary>
            /// normal, default 2D map
            /// </summary>
            ROADMAP,
            /// <summary>
            /// photographic map
            /// </summary>
            SATELLITE,
            /// <summary>
            /// photographic map + roads and city names
            /// </summary>
            HYBRID,
            /// <summary>
            /// map with mountains, rivers, etc.
            /// </summary>
            TERRAIN,
        }

        public class MapMarker : IEquatable<MapMarker> {
            public double lat;
            public double lng;
            public string title;
            public MapInfoWindow infoWindow;

            public bool Equals(MapMarker other) {
                if (lat == other.lat && lng == other.lng && title == other.title && (infoWindow?.content == other.infoWindow?.content)) {
                    return true;
                }
                return base.Equals(other);
            }
        }

        public class Position {
            public double latitude;
            public double longitude;
        }

        public class MapInfoWindow {
            public string content;
        }
        #endregion

        #region Private Globals
        private readonly bool _firstMapControlOnPage;
        private Position _position;
        private MapType _mapType;
        private System.Collections.Generic.List<MapMarker> Markers { get; } = new System.Collections.Generic.List<MapMarker>();
        #endregion

        #region Public Globals
        public string APIKey { get; set; }
        #endregion

        #region Constructors
        public GoogleMap(string apiKey = "", Position startPos = null, MapType mapType = MapType.ROADMAP,/*string mapId = "basic_map",*/ bool firstMapControlOnPage = true) {
            //Id = mapId;
            _firstMapControlOnPage = firstMapControlOnPage;
            APIKey = apiKey;
            _position = startPos ?? new Position {
                latitude = 51.5073346,
                longitude = -0.1276831,
            };
            _mapType = mapType;
        }
        #endregion

        #region Overrides
        public override void WriteInnerHtml(XmlWriter w) {
            base.WriteInnerHtml(w);
        }

        public override void WriteOuterHtml(XmlWriter w) {
            base.WriteOuterHtml(w);

            if (_firstMapControlOnPage) {
                WriteScriptsOnFirstInstance(w);
            } else {
                WriteMapScript(w);
            }
        }

        public override string OuterHtml {
            get {
                return base.OuterHtml
                    .Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    ;
            }
        }

        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        public override string ToString() {
            if (_firstMapControlOnPage) {
                return $@"{base.ToString()}
<script src=""http://ajax.googleapis.com/ajax/libs/jquery/1.7/jquery.min.js""></script>
<script type=""text/javascript"" src=""http://maps.google.com/maps/api/js?key={APIKey}""></script>
<script type=""text/javascript"" src=""/GoogleMaps.js""></script>
<script>
{ReplaceTokens(MapsScript)}
</script>"
                    .Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    ;
                //<script src=""/googlemaps.js""></script>";
            }
            return $@"{base.ToString()}
<script>
{ReplaceTokens(MapsScript)}
</script>"
                    .Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    ;
        }

        #endregion

        #region Public Methods
        public void AddMarker(MapMarker marker) {
            try {
                Markers.Add(marker);
                Send(Message.Event(Id, "addMarker", marker));
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void CenterOn(Position position) {
            try {
                _position = position;
                Send(Message.Event(Id, "setCenter", _position));
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<Position> GetCenter()//This requires the map to have loaded already
        {
            try {
                Position pos = null;
                await Task.Run(() => {
                    var mre = new ManualResetEvent(false);
                    CentreReceived += GoogleMap_CentreReceived;
                    Send(Message.Event(Id, "raiseCenter"));
                    mre.WaitOne(90000);//90 Second Timeout

                    void GoogleMap_CentreReceived(object sender, DOMEventArgs e) {
                        CentreReceived -= GoogleMap_CentreReceived;
                        pos = new Position {
                            latitude = (double)e.Data["latitude"],
                            longitude = (double)e.Data["longitude"],
                        };
                        mre.Set();
                    }
                });
                return pos ?? _position;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return _position;
            }
        }

        public void RemoveMarker(MapMarker mapMarker) {
            try {
                var index = Markers.IndexOf(Markers.First(m => m.Equals(mapMarker)));
                Markers.Remove(mapMarker);
                Send(Message.Event(Id, "removeMarker", index));
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void ClearMarkers() {
            try {
                Markers.Clear();
                Send(Message.Event(Id, "clearMarkers"));
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void ChangeMapType(MapType mapType) {
            try {
                _mapType = mapType;
                Send(Message.Event(Id, "changeMapType", mapType.ToString()));
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

        #region Private Events
        private event DOMEventHandler CentreReceived {
            add => AddEventListener("centerRaised", value);
            remove => RemoveEventListener("centerRaised", value);
        }
        #endregion

        #region Private Methods
        private void WriteScriptsOnFirstInstance(XmlWriter w) {
            w.WriteString(@"
");
            w.WriteStartElement("script");
            w.WriteAttributeString("src", "http://ajax.googleapis.com/ajax/libs/jquery/1.7/jquery.min.js");
            w.WriteString(@"
");
            w.WriteFullEndElement();

            w.WriteString(@"
");
            w.WriteStartElement("script");
            w.WriteAttributeString("type", "text/javascript");
            w.WriteAttributeString("src", $"http://maps.google.com/maps/api/js?key={APIKey}");
            w.WriteString(@"
");
            w.WriteFullEndElement();

            w.WriteString(@"
");
            /*w.WriteStartElement("script");
            w.WriteRaw(_gmaps);
            w.WriteString(@"
");
            w.WriteFullEndElement();*/

            WriteMapScript(w);
        }

        private void WriteMapScript(XmlWriter w) {
            w.WriteString(@"
");
            w.WriteStartElement("script");
            w.WriteRaw(ReplaceTokens(MapsScript));
            w.WriteString(@"
");
            w.WriteFullEndElement();
        }

        private string ReplaceTokens(string tokenizedString) {
            return tokenizedString
                .Replace("$$Map_ID$$", Id);
        }

        private string GetMarkers() {
            string markers = string.Empty;
            foreach (var marker in Markers) {
                markers += $@"
    map.addMarker({Newtonsoft.Json.JsonConvert.SerializeObject(marker)});
";
            }
            return markers;
        }
        #endregion

        #region JavaScript Scripts
        private string MapsScript => $@"
$(document).ready(function () {{
    
    var mapOwner = $('#$$Map_ID$$');
    mapOwner.height('500px');
    var map = new GMaps({{
        el: '#$$Map_ID$$',
        lat: {_position.latitude.ToString().Replace(',', '.')},
        lng: {_position.longitude.ToString().Replace(',', '.')},
        zoom: 12,
        zoomControl: true,
        zoomControlOpt: {{
            style: 'SMALL',
            position: 'TOP_LEFT'
        }},
        panControl: false,
        mapType: '{_mapType.ToString()}',
    }});
    {GetMarkers()}
    mapOwner.on('clearMarkers', clearMarkersEventRaised);
    function clearMarkersEventRaised(e) {{
        console.log ('Clearing Map markers',map.markers);
        var markerLength = map.markers.length;
        for (var i = 0, len = markerLength; i < len; i++) {{
            map.removeMarker(map.markers[0]);
        }}
        console.log ('After clearing map markers',map.markers);
    }}
    mapOwner.on('changeMapType', changeMapTypeEventRaised);
    function changeMapTypeEventRaised(e) {{
        console.log ('Changing map type',e.originalEvent.detail,e.originalEvent,e);
        var center = map.getCenter();
        var coords = {{
            latitude: center.lat(),
            longitude: center.lng(),
        }};
        var arr = map.markers;
        map = new GMaps({{
            el: '#$$Map_ID$$',
            lat: coords.latitude,
            lng: coords.longitude,
            zoom: 12,
            zoomControl: true,
            zoomControlOpt: {{
                style: 'SMALL',
                position: 'TOP_LEFT'
            }},
            panControl: false,
            mapType: e.originalEvent.detail,
        }});
        for (var i = 0, len = arr.length; i < len; i++) {{
          map.addMarker(arr[i]);
        }}
    }}
    mapOwner.on('addMarker', addMarkerEventRaised);
    function addMarkerEventRaised(e) {{
        console.log ('Adding Map marker',e.originalEvent.detail,e.originalEvent,e);
        map.addMarker(e.originalEvent.detail);
    }}
    mapOwner.on('removeMarker', removeMarkerEventRaised);
    function removeMarkerEventRaised(e) {{
        console.log ('Removing Map marker',e.originalEvent.detail,e.originalEvent,e);
        map.removeMarker(map.markers[e.originalEvent.detail]);
    }}
    mapOwner.on('setCenter', setCenterEventRaised);
    function setCenterEventRaised(e) {{
        console.log ('Setting Map Center',e.originalEvent.detail,e.originalEvent,e);
        map.setCenter(e.originalEvent.detail.latitude, e.originalEvent.detail.longitude);
    }}
    mapOwner.on('raiseCenter', raiseCenterEventRaised);
    function raiseCenterEventRaised(e) {{
        
        console.log ('Getting Map Center');
        var center = map.getCenter();
        var coords = {{
            latitude: center.lat(),
            longitude: center.lng(),
        }};
        console.log ('Current Map Center',center);
        var eventMsg = new CustomEvent('centerRaised', {{ detail: coords }});
        //mapOwner.trigger( 'centerRaised', [ coords ] );
        mapOwner[0].dispatchEvent(eventMsg);
    }}
}});
";
        #endregion
    }
}
