import React, { Component } from "react";
import ReactPlayer from "react-player";
import Duration from "./Duration";
import { AppStateContext } from './AppStateContext';

// Komponent til HLS-afspilleren
export class Player extends Component {

    // Denne komponent indeholder state for selve den igangværende afspilning
    constructor(props){
        super(props);
        this.state = {
          played: 0,
          duration: 0,
          seeking: false
        };
      }

    // Viderediriger når der trykkes play/pause
    handlePlayPause = () => {
        this.context.player.togglePlay();
      }
    
    handlePlay = () => {
      console.log('onPlay')
    }
  
    // Der hoppes til et nyt sted i tracket, det håndterer disse tre metoder
    handleSeekMouseDown = e => {
      this.setState({ seeking: true })
    }
  
    handleSeekChange = e => {
      this.setState({ played: parseFloat(e.target.value) })
    }
  
    handleSeekMouseUp = e => {
      this.setState({ seeking: false })
      this.player.seekTo(parseFloat(e.target.value))
    }
    
    // Når der afspilles, opdateres state i disse to metoder
    handleProgress = state => {
      console.log('onProgress', state)
      // Vi opdaterer kun slideren, hvis der ikke seekes
      if (!this.state.seeking) {
        this.setState(state)
      }
    }
  
    handleDuration = (duration) => {
      console.log('onDuration', duration)
      this.setState({ duration })
    }
  
    ref = player => {
      this.player = player
    }

    render() {
      
      const playing = this.context.player.playing;
      const played = this.state.played;
      // Definer URL for Master Playlist
      const url = this.context.player.nowPlaying !== null ? process.env.REACT_APP_STREAMING_SERVER + "/"
                      + this.context.player.nowPlaying.id 
                      + ".m3u8" : "";

      return (      
            <div className="bg-dark fixed-bottom">
              <div className="row">
                <div className="text-white text-center col-sm-12">
                  { this.context.player.nowPlaying !== null ? "Nu spiller: " + this.context.player.nowPlaying.number + ": " + this.context.player.nowPlaying.title : "" }
                  </div>
              </div>
              <div className="row mt-1 mb-1">
                    <span className="mx-auto text-white" onClick={ this.handlePlayPause } >
                      { playing ? (
                        <svg width="2em" height="2em" viewBox="0 0 16 16" className="bi bi-pause-fill" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                          <path d="M5.5 3.5A1.5 1.5 0 0 1 7 5v6a1.5 1.5 0 0 1-3 0V5a1.5 1.5 0 0 1 1.5-1.5zm5 0A1.5 1.5 0 0 1 12 5v6a1.5 1.5 0 0 1-3 0V5a1.5 1.5 0 0 1 1.5-1.5z"/>
                        </svg>
                      ) : (
                        <svg width="2em" height="2em" viewBox="0 0 16 16" className="bi bi-play-fill" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                            <path d="M11.596 8.697l-6.363 3.692c-.54.313-1.233-.066-1.233-.697V4.308c0-.63.692-1.01 1.233-.696l6.363 3.692a.802.802 0 0 1 0 1.393z"/>
                        </svg>
                      )  } 
                    </span>
                )
              </div>
              <div className="row m-1">
                {/* Her defineres afspilningskomponenten fra react-player */}
                <ReactPlayer                  
                  ref={this.ref}
                  url={ url } 
                  controls={false} 
                  duration={ this.state.duration }
                  style={ {display: 'none'}}
                  playing={playing}
                  onPlay={this.handlePlay}
                  onProgress={this.handleProgress}
                  onDuration={this.handleDuration}
                  onSeek={e => console.log('onSeek', e)}
                  onError={(e, data) => {
                    console.log('onError', data)
                      if(data?.fatal){
                        this.context.error.setError(true, "Nummeret kan ikke afspilles")
                      }
                  }}
                  config={{
                    file: 
                    {
                      forceHLS: true, 
                      forceAudio: true,
                      hlsOptions: {
                        forceHLS: true,
                        debug: true,
                        xhrSetup: (xhr, url) => {
                          xhr.setRequestHeader('Authorization', "Bearer " + this.context.user.access_token);
                        }
                      }
                    } 
                  }}
                />
                <Duration className="text-white col-sm-1" seconds={this.state.duration * this.state.played} />
                <div className="col-sm-10">
                  {/* Statuslinjen for afspilning er et input */}
                  <input
                        className="custom-range w-100"
                        type='range' min={0} max={0.999999} step='any'
                        value={played}
                        onMouseDown={this.handleSeekMouseDown}
                        onChange={this.handleSeekChange}
                        onMouseUp={this.handleSeekMouseUp}
                    />
                </div>
                <Duration className="text-white col-sm-1" seconds={this.state.duration} />         
              </div>
            </div>
      )
    }
}
Player.contextType = AppStateContext;

