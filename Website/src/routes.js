import React from "react";
import { BrowserRouter, Route, Switch, Redirect as Router } from "react-router-dom";
import Login from "./components";
import Register from "./components";

const Routes = () => (
    <BrowserRouter>
        <Switch>
            <Route exact path="/" component={() => <h1>Hello World</h1>}/>
        </Switch>
    </BrowserRouter>
);

export default Routes;