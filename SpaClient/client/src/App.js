import React, { Component } from 'react';
import './App.css';
import { AlbumDisplay } from "./AlbumDisplay";
import { Player } from './Player';
import { BrowserRouter as Router, Route, Switch, Redirect } from "react-router-dom";
import { AuthenticationContext  } from '@axa-fr/react-oidc-context';
import { AlbumSearch } from './AlbumSearch';
import { MyAlbums } from './MyAlbums';
import { RestDataSource } from './RestDataSource';
import  Home  from "./Home";
import Header from './Header';
import { AppStateContext } from './AppStateContext';
import { RequestError } from "./RequestError";

class App extends Component {

  // App holder state for det meste af applikationens data vha. appState
  constructor(props){
    super(props);
    this.state = {
      appState: {
        user: null,
        userAlbums: {
            userAlbumIds: [],
            userAlbums: [],
            getUserAlbumIds: this.getUserAlbumIds,
            getUserAlbums: this.getUserAlbums,
            toggleUserAlbum: this.toggleUserAlbum,
        },
        player: {
            nowPlaying: null,
            playing: false,
            togglePlay: this.togglePlay,
            changeTrack: this.changeTrack,
        },
        search: {
            query: "",
            searchResult: [],
            getSearchResult: this.getSearchResult,
            setQuery: this.setQuery
        },
        error: {
          shown: false,
          message: "",
          setError: this.setError 
        }
      },
    };
    this.dataSource = new RestDataSource((err) => this.setError(true, err));
  }

  // Her sættes fejlmeddelelse
  setError = (shown, message) => {
    this.setState(prevState => ({
      appState: {
        ...prevState.appState,
        error: {
          ...prevState.appState.error,
          shown: shown, 
          message: message
        } 
      }
    }));
  }

  // Her opdateres det track, som afspilles
  changeTrack = (t) => {
    this.setState(prevState => ({
      appState: {
        ...prevState.appState,
        player: {
          ...prevState.appState.player,
          nowPlaying: t, 
          playing: true
        } 
      }
    }));
  }

  // Stop eller sæt afspilning på pause
  togglePlay = () => {

    let playing = this.state.appState.player.playing;

    this.setState(prevState => ({
      appState: {
        ...prevState.appState,
        player: {
          ...prevState.appState.player,
          playing: !playing
        } 
      }
    }));

  }

  // Opdater om at album er gemt i brugerens bibliotek eller ej
  toggleUserAlbum = (event, id) => {

    event.preventDefault();

    let token = this.context.oidcUser.access_token;

    if(this.state.appState.userAlbums.userAlbumIds.includes(id))
    {
      // Brugeren har gemt albummet, dvs. det skal slettes
      let newUserAlbums = this.state.appState.userAlbums.userAlbumIds.filter(function(i) { return i !== id});

      this.dataSource.DeleteUserAlbums([id], token, data => this.setState(prevState => ({
        appState: {
          ...prevState.appState,
          userAlbums: {
            ...prevState.appState.userAlbums,
            userAlbumIds: newUserAlbums
          }
        }
      })));      
    }
    else
    {
      // Brugeren har ikke gemt albummet, så det skal gemmes
      let newUserAlbums = this.state.appState.userAlbums.userAlbumIds.concat(id);

      this.dataSource.PutUserAlbums([id], token, data => this.setState(prevState => ({
        appState: {
          ...prevState.appState,
          userAlbums: {
            ...prevState.appState.userAlbums,
            userAlbumIds: newUserAlbums
          }
        }
      })));      
    }  
  }

  // Hent data for brugerens gemte albums i kataloget
  getUserAlbums = () => {
    if(this.state.appState.userAlbums.userAlbumIds.length !== 0){

      this.dataSource.GetAlbumsFromList(this.state.appState.userAlbums.userAlbumIds, data => this.setState(prevState => ({
        appState: {
          ...prevState.appState,
          userAlbums: {
            ...prevState.appState.userAlbums,
            userAlbums: data
          }
        }
      })));
    }     
  }

  // Hent Id'er for brugerens gemte albums
  getUserAlbumIds = () => {
    let context = this.context;

    if(context.oidcUser){
      let token = context.oidcUser?.access_token;
      this.dataSource.GetUserAlbums(token, userIds => this.setState(prevState => ({
        appState: {
          ...prevState.appState,
          userAlbums: {
            ...prevState.appState.userAlbums,
            userAlbumIds: userIds
          } 
        }
      })));
    }
  }

  // Registrer søgestreng i appState
  setQuery = (query) => {
 
    this.setState(prevState => ({
      appState: {
        ...prevState.appState,
        search: {
          ...prevState.appState.search,
          query: query
        } 
      }
    }));
  }

  // Hent albums der matcher søge resultat i katalog
  getSearchResult = () => {

    var query = this.state.appState.search.query;

    this.dataSource.GetAlbumsFromQuery(query, data => this.setState(prevState => ({
      appState: {
        ...prevState.appState,
        search: {
          ...prevState.appState.search,
          searchResult: data
        } 
      }
    })));
  }

  render() {
    return (
      <Router>
        <AppStateContext.Provider value={this.state.appState}>
          <div>
            <Header />
            {
            this.state.appState.error.shown ? (
              <RequestError />
            ) : "" 
            }     
            <AuthenticationContext.Consumer>
              { props => 
              <React.Fragment>
                <Switch>
                  {/* Her defineres appens routing */}
                  <Route path="/home" component={Home} />
                  <Route path="/albums/search" exact={ true } component={AlbumSearch} />
                  <Route path="/albums/:id" render={ (routeProps) => 
                    <AlbumDisplay albumId={ routeProps.match.params.id } /> 
                  } />
                  <Route path="/myalbums" exact={ true } component={MyAlbums} />
                  <Route path="/error/:message" component={ RequestError } />
                  <Redirect to="/home" />
                </Switch>     
                <div>
                { /* Hvis brugeren er logget ind, vises afspilleren */}
                { props.oidcUser ? (<Player />) : ("") }
                </div>  
              </React.Fragment>   
              }
            </AuthenticationContext.Consumer>  
            <div className="mb-5" />
          </div>
        </AppStateContext.Provider>
      </Router>
    )   
  }
  
  // Når appen mounter, hentes brugerens album-id'er
  componentDidMount() {   
    this.setState(prevState => ({
      appState: {
        ...prevState.appState,
        user: this.context.oidcUser
      }
    }), this.getUserAlbumIds());
    console.log("Nu mounter App");
    console.log("oidcUser: " + this.context.oidcUser);
    console.log("user: " + this.state.appState.user);
    console.log("AuthContext " + this.context);
    console.log(this.context.error);

    // oidcUser: User | null;
    // isEnabled: boolean;
    // login: Function;
    // logout: Function;
    // events: UserManagerEvents;
    // authenticating: ComponentType;
    // isLoading: boolean;
    // isLoggingOut: boolean;
    // userManager: UserManager;
    // error: string;
  }

}
// Her sættes en kontekst der indeholder information om autentifikation med OIDC
App.contextType = AuthenticationContext;

export default App;