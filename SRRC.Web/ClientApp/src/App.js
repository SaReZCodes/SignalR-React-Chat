import React, { Component } from 'react';
import { HashRouter, Switch, Route } from "react-router-dom";
import './custom.css'
import ProtectedRoute from './components/protectedRoute';

const loading = (
    <div className="pt-3 text-center">
        <p>
        </p>
        <div className="sk-spinner sk-spinner-pulse"></div>
    </div>
);

const Default = React.lazy(() => import('./layout/Layout'))
const LoginPage = React.lazy(() => import('./views/auth/loginForm'))
const LogoutPage = React.lazy(() => import('./views/auth/logout'))
const Page404 = React.lazy(() => import('./views/pages/page404/Page404'))
const Page500 = React.lazy(() => import('./views/pages/page500/Page500'))

export default class App extends Component {
    static displayName = App.name;

    render() {

        return (
            <HashRouter>
                <React.Suspense fallback={loading}>
                    <Switch>
                        <Route exact path='/page404' name="page404" render={(props) => <Page404 {...props} />} />
                        <Route exact path="/page500" name="page500" render={(props) => <Page500 {...props} />} />
                        <Route exact path="/login" name="Login Page" render={(props) => <LoginPage {...props} />} />
                        <Route exact path='/logout' name="logout Page" render={(props) => <LogoutPage {...props} />} />
                        <ProtectedRoute path="/" name="Home" render={(props) => <Default {...props} />} />
                    </Switch>
                </React.Suspense>
            </HashRouter>

        );
    }


}
