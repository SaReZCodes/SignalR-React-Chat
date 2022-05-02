import React, { Component, Suspense } from "react";
import { Redirect, Route, Switch } from "react-router-dom";
import Routes from "../../routes";
class AppContent extends Component {
    render() {
        return (
            <div className="body flex-grow-1 px-3" style={{ minHeight: "270px" }}>
                <div className="container-lg">
                    <div className="row">
                        <Suspense fallback={<loading />}>
                            <Switch>
                                {Routes.map((route, idx) => {
                                    return (
                                        route.component && (
                                            <Route
                                                key={idx}
                                                path={route.path}
                                                exact={route.exact}
                                                name={route.name}
                                                render={(props) => (
                                                    <>
                                                        <route.component {...props} />
                                                    </>
                                                )}
                                            />
                                        )
                                    )
                                })}
                                <Redirect from="/" to="/" />
                            </Switch>
                        </Suspense>
                    </div>
                </div>
            </div>
        );
    }
}
export default React.memo(AppContent);