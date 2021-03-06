"use strict";
function sp_init_map_script(_map_id, dataList){




//var data = {
//	"status": 'found',
//	"listing": [
//		{
//			"id": 1,
//			"longitude": 74.330910,
//			"latitude": 31.483498,
//			"image": "../Images/serviceproviders/img-01.jpg",
//			"subjects": "MDS - Paedodontics & Preventive Dentistry",
//			"title": "Bright Future Group &amp; Company",
//			"url": "#",
//			"featured": 'no',
//			"marker": '../Images/icons/markerone.png',
//		}
//	]
//}





	var directory_path = '../';
	var _data_list = dataList;
	var dir_latitude = '31.503912';
	var dir_longitude = '74.331631';
	var dir_map_type	 = 'ROADMAP';
	var dir_close_marker		= directory_path+'images/icons/close.png';
	var dir_cluster_marker		= directory_path+'images/icons/cluster.png';
	var dir_map_marker			= directory_path+'images/icons/marker.png';
	var dir_cluster_color		= '#000';
	var dirZoom				= '12';
	var dir_map_scroll			= 'false';
	var gmap_norecod			= '';
	var loader_html	= '<div class="provider-loader-wrap"><div class="provider-loader"><div class="bounce1"></div><div class="bounce2"></div><div class="bounce3"></div></div></div>';
	var locationCenter = null;
    debugger;
	if (_data_list.status == 'found' && _data_list.listing.length>0) {
		var response_data	= _data_list.listing;
		 locationCenter = new google.maps.LatLng(response_data[0].latitude,response_data[0].longitude);
	} else{
		 locationCenter = new google.maps.LatLng(dir_latitude,dir_longitude);
	}

	if(dir_map_type == 'ROADMAP'){
		var map_id = google.maps.MapTypeId.ROADMAP;
	} else if(dir_map_type == 'SATELLITE'){
		var map_id = google.maps.MapTypeId.SATELLITE;
	} else if(dir_map_type == 'HYBRID'){
		var map_id = google.maps.MapTypeId.HYBRID;
	} else if(dir_map_type == 'TERRAIN'){
		var map_id = google.maps.MapTypeId.TERRAIN;
	} else {
		var map_id = google.maps.MapTypeId.ROADMAP;
	}

	var scrollwheel	= true;
	var lock			 = 'unlock';

	if( dir_map_scroll == 'false' ){
		scrollwheel	= false;
		lock			 = 'lock';
	}

	var mapOptions = {
		zoom: 12,
		maxZoom: 15,
		mapTypeId: map_id,
		scaleControl: true,
		scrollwheel: scrollwheel,
		disableDefaultUI: true
	}

	var map = new google.maps.Map(document.getElementById(_map_id), mapOptions);


	if (dataList != undefined && dataList.listing != null) {
	    var currentPosObj = dataList.listing[0];
	    map.setCenter(new google.maps.LatLng(currentPosObj.latitude, currentPosObj.longitude));
	    var radius = parseInt($('#locDistance').val()) * 1000;
	    var posCenter = { lat: parseFloat(currentPosObj.latitude), lng: parseFloat(currentPosObj.longitude) };
	    // Add the circle for this city to the map.
	    var cityCircle = new google.maps.Circle({
	        strokeColor: '#FF0000',
	        strokeOpacity: 0.8,
	        strokeWeight: 1,
	        fillColor: '#0080ff',
	        fillOpacity: 0.35,
	        map: map,
	        center: posCenter,
	        radius: radius
	    });
	}
	var bounds = new google.maps.LatLngBounds();

	//Zoom In
	if(document.getElementById('doc-mapplus') ){ 
	    google.maps.event.addDomListener(document.getElementById('doc-mapplus'), 'click', function () {
			var current= parseInt( map.getZoom(),10 );
			current++;
			if(current>20){
				current=20;
			}
			map.setZoom(current);
		});
	}

	function reduceSIze() {
	    var current = parseInt(map.getZoom(), 10);
	    current--;
	    if (current < 0) {
	        current = 0;
	    }
	    map.setZoom(current);
	}
	//Zoom Out
	if(document.getElementById('doc-mapminus') ) {
	    google.maps.event.addDomListener(document.getElementById('doc-mapminus'), 'click', function () {
	        reduceSIze();

	    });
	}
    
	//Lock Map
	if( document.getElementById('doc-lock') ){ 
		google.maps.event.addDomListener(document.getElementById('doc-lock'), 'click', function () {
			if(lock == 'lock'){
				map.setOptions({ 
						scrollwheel: true,
						draggable: true 
					}
				);
				jQuery("#doc-lock").html('<i class="fa fa-unlock-alt" aria-hidden="true"></i>');
				lock = 'unlock';
			}else if(lock == 'unlock'){
				map.setOptions({ 
						scrollwheel: false,
						draggable: false 
					}
				);
				jQuery("#doc-lock").html('<i class="fa fa-lock" aria-hidden="true"></i>');
				lock = 'lock';
			}
		});
	}

	//Map resize
	jQuery(document).on("click",'.tg-btnmapview', function(e){
        jQuery('.tg-mapinnerbanner').toggleClass('tg-open');
		if( jQuery('.tg-mapinnerbanner').hasClass('tg-open') ) {
			jQuery('.tg-mapinnerbanner').append(loader_html);
		}
		setTimeout(function(){
			jQuery('.tg-mapinnerbanner').find('.provider-loader-wrap').remove();
			google.maps.event.trigger(map,"resize");
			map.setCenter(locationCenter);
		},1000);
    });
			
	if( _data_list.status == 'found' ){
		jQuery('#gmap-noresult').html('').hide(); //Hide No Result Div
		var markers = new Array();
		var info_windows = new Array();

		if (response_data !== undefined) {
            for (var i = 0; i < response_data.length; i++) {
                markers[i] = new google.maps.Marker({
                    position: new google.maps.LatLng(response_data[i].latitude, response_data[i].longitude),
                    map: map,
                    icon: response_data[i].marker,
                    title: response_data[i].title,
                    animation: google.maps.Animation.DROP,
                    visible: true
                });
                bounds.extend(markers[i].getPosition());
                var boxText = document.createElement("div");
                boxText.className = 'tg-infoBox';

                var infobox_html = "";
                infobox_html += '<div class="tg-serviceprovider">';
                infobox_html += '<figure class="tg-featuredimg"><img src="' + response_data[i].image + '" alt="' + response_data[i].title + '"></figure>';
                infobox_html += '<div class="tg-companycontent">';
                if (response_data[i].featured == 'yes') {
                    infobox_html += '<span class="tg-featured"><i class="fa fa-bolt"></i></span>';
                }
                /*if(response_data[i].featured == 'yes'){
					infobox_html += '<span class="tg-featured"><i class="fa fa-bolt"></i></span>';
				}*/
                infobox_html += '<div class="tg-subjects">' + response_data[i].subjects + '</div>';
                infobox_html += '<ul class="tg-tags"><li><a class="tg-tag tg-featuredtag" href="#">featured</a></li><li><a class="tg-tag tg-verifiedtag" href="#">verified</a></li><li><a class="tg-tag tg-verifiedtag" href="#">' + response_data[i].CatName + '</a></li></ul>';
                infobox_html += '<div class="tg-title"><h3><a href="' + response_data[i].url + '">' + response_data[i].title + '</a></h3></div>';
                infobox_html += '<ul class="tg-matadata"><li><span class="tg-stars"><span></span></span></li><li><i class="fa fa-thumbs-o-up"></i><em>99% (1009 votes)</em></li></ul>';
                infobox_html += '</div>';
                boxText.innerHTML = infobox_html;

                var myOptions = {
                    content: boxText,
                    disableAutoPan: true,
                    maxWidth: 0,
                    alignBottom: true,
                    pixelOffset: new google.maps.Size(-390, -70),
                    zIndex: null,
                    closeBoxMargin: "0 0 -16px -16px",
                    closeBoxURL: dir_close_marker,
                    infoBoxClearance: new google.maps.Size(1, 1),
                    isHidden: false,
                    pane: "floatPane",
                    enableEventPropagation: false
                };
                var ib = new InfoBox(myOptions);
               if (i !== 0) {
                   attachInfoBoxToMarker(map, markers[i], ib);
               }
            }
        }
		
		map.fitBounds(bounds);
		/* Marker Clusters */
		var markerClustererOptions = {
			ignoreHidden: true,
			//maxZoom: 14,
			styles: [{
				textColor: dir_cluster_color,
				url: dir_cluster_marker,
				width: 23,
				height: 30,
			}]
		};
		var markerClusterer = new MarkerClusterer( map, markers, markerClustererOptions );
	} else{
		map.fitBounds(bounds);
		jQuery('#gmap-noresult').html(gmap_norecod).show();
	}
	if (dataList.listing != null) {
	    debugger;
	    if (dataList.listing.length ===1) {
	        reduceSIze();
	        reduceSIze();
	        reduceSIze();
	    }
	    if (dataList.listing.length === 2) {
	        reduceSIze();
	    }
	}
	
}
//Assign Info window to marker
function attachInfoBoxToMarker( map, marker, infoBox ){
    google.maps.event.addListener(marker, 'click', function () {
		var scale = Math.pow( 2, map.getZoom() );
		var offsety = ( (100/scale) || 0 );
		var projection = map.getProjection();
		var markerPosition = marker.getPosition();
		var markerScreenPosition = projection.fromLatLngToPoint( markerPosition );
		var pointHalfScreenAbove = new google.maps.Point( markerScreenPosition.x, markerScreenPosition.y);
		var aboveMarkerLatLng = projection.fromPointToLatLng( pointHalfScreenAbove );
	///	map.setCenter( aboveMarkerLatLng );
		
		jQuery(".infoBox").hide();
		infoBox.open( map, marker );
		
	});
}