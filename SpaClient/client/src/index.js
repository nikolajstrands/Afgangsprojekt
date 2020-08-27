import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';

import 'bootstrap/dist/css/bootstrap.css';
import { AuthenticationProvider, oidcLog} from '@axa-fr/react-oidc-context';
import CustomCallback from './CustomCallback';
import oidcConfiguration from './configuration';

// Her tilføjes React-appen til DOM
ReactDOM.render(
  <AuthenticationProvider configuration={oidcConfiguration} 
                                loggerLevel={oidcLog.DEBUG}
                                callbackComponentOverride={CustomCallback}
                                
   >
      <App />
   </AuthenticationProvider>
, document.getElementById('root')
);
