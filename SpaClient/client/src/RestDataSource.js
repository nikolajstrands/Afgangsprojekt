import Axios from "axios";

// Her defineres de metoder der bruges til at hente data fra katalog- og brugerdata-services
export class RestDataSource {

    constructor(errorCallback) {
        // URL'er hentes fra environment-variable
        this.CATALOG_API_URL = process.env.REACT_APP_CATALOG_LOAD_BALANCER;
        this.USER_DATA_API_URL = process.env.REACT_APP_VM_USER_DATA_API;
        this.handleError = errorCallback;
    }

    GetAlbumsFromQuery(query, callback) {
        // Hvis query er tom laves ikke noget server-kald
        if(query === ""){
            return callback([]);
        }
        // Der bruges Axios-biblioteket som er et HTTP-klientbibliotek
        Axios.request({
            method: "get",
            url: this.CATALOG_API_URL,
            params: {
                query: query
            }
        })
        .then(response => callback(response.data))  
        .catch(error => this.handleError(error.message));
    }

    GetAlbumsFromList(userAlbumIds, callback) {
        
        let query = userAlbumIds.map(id => "ids=" + id).join("&");

        Axios.request({
            method: "get",
            url: this.CATALOG_API_URL + "/?" + query
        //  Der returneres et promise, som kalder en callback-funktion, der ingår som parameter
        })
        .then(response => callback(response.data))
        .catch(error => this.handleError("Albuminfo kunne ikke hentes" + (error.reponse ? " (status: " + error.response.code + ")" : "") ));

    };

    GetAlbumById(id, callback) {
        Axios.request({
            method: "get",
            url: this.CATALOG_API_URL + "/" + id,          
        })
        .then(response => callback(response.data))
        .catch(error => this.handleError("Albuminfo kunne ikke hentes" + (error.reponse ? " (status: " + error.response.code + ")" : "") ));
    }

    GetUserAlbums(token, callback) {
        Axios.request({
            method: "get",
            url: this.USER_DATA_API_URL,
            // Der sættes access token på i en header
            headers: {'Authorization': token === ""  ? "" : 'Bearer ' + token},
        })
        .then(response => callback(response.data))
        .catch(error => this.handleError("Brugerdata kunne ikke hentes" + (error.reponse ? " (status: " + error.response.code + ")" : "") ));

    }

    async PutUserAlbums(albumIds, token, callback) {
        Axios.request({
            method: "put",
            url: this.USER_DATA_API_URL,
            headers: {'Authorization': token === ""  ? "" : 'Bearer ' + token},
            data: albumIds
        })
        .then(response => callback(response.data))
        .catch(error => this.handleError("Brugerdata kunne ikke opdateres" + (error.reponse ? " (status: " + error.response.code + ")" : "") ));

    }

    async DeleteUserAlbums(albumIds, token, callback) {
        Axios.request({
            method: "delete",
            url: this.USER_DATA_API_URL,
            headers: {'Authorization': token === ""  ? "" : 'Bearer ' + token},
            data: albumIds
        })
        .then(response => callback(response.data))
        .catch(error => this.handleError("Brugerdata kunne ikke opdateres" + (error.reponse ? " (status: " + error.response.code + ")" : "") ));
    }  
}
