import React from "react";

const Counter = React.lazy(() => import('./components/Counter'));
const FetchData = React.lazy(() => import('./components/FetchData'));
const routes = [
    { path: '/', exact: true, name: 'Home' },
    { path: '/counter', name: 'Dashboard', component: Counter, exact: true },
    { path: '/fetch-data', name: 'Theme', component: FetchData, exact: true }
];
export default routes;