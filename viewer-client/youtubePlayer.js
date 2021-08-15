
/* variables */
var player;
var done = false;

/* Init Youtube */
var tag = document.createElement('script');
tag.src = "https://www.youtube.com/iframe_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

/* Functions */
function onYouTubeIframeAPIReady(){   
    player = new YT.Player('player', {
        height: '450',
        width: '800',
        videoId: 'bgjbAkuDle4', // GJDNkVDGM_s
        playerVars:{
            rel:0,
            autoplay:0
        },
        events: {
            'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
        }
    });
}           

function readyVideo(id){
    if(player != null){
        console.log("received id:", id)
        player.cueVideoById(id);
    }
}

function onPlayerReady(event){
}

function onPlayerStateChange(event, element){
    if(event.data == YT.PlayerState.PLAYING && !done){
        //setTimeOut(stopVideo, 6000);
        done = true;
    }
    if(event.date == YT.PlayerState.ENDED){

    }
}

function stopVideo(){
    player.stopVideo();
}
function playVideo(){
    player.playVideo();
}