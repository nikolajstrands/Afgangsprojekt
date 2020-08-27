// Dette er konfigurationsoplysninger til brug for autentificering vha. react-oidc-biblioteket
const configuration = {
    client_id: 'SpaClient',
    redirect_uri: 'http://localhost:3000/authentication/callback',
    response_type: 'code',
    post_logout_redirect_uri: 'http://localhost:3000/',
    scope: 'openid profile StreamingServer:Read UserDataApi',
    // Autorisationsserverens adresse hentes fra environment-variabel
    authority: process.env.REACT_APP_VM_AUTH_SERVER,
    silent_redirect_uri: 'http://localhost:3000/authentication/silent_callback',
    automaticSilentRenew: false,
    loadUserInfo: true,
    triggerAuthFlow: true
  };
  
  export default configuration;