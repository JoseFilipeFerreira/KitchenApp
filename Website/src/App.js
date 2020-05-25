import React, { Component } from "react";
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";
import axios from "axios";
import Login from './login'
import Register from './register'
import Dashboard from './dashboard'
import UserInfo from "./userinfo";
import Inventory from "./inventory";
import Friends from "./friends";



class App extends Component {


  isAuth = () => {
    let token = localStorage.getItem('auth');
    axios
      .get("http://localhost:1331/inventory/all", {
        headers: { "auth": token }
      }, { withCredentials: true })
      .then((response) => {
        console.log('Token valido');
      })
      .catch((error) => {
        console.log(error);
      });

    if (localStorage.getItem('auth') != null) {
      return true;
    } else {
      return false;
    }
  }

  render() {
    if (this.isAuth() === false) {
      return (
        <React.Fragment>
          <Router>
            <Switch>
              <Route path="/register" component={Register} />
              <Route exact path="*" component={Login} />
            </Switch>
          </Router>
        </React.Fragment>
      )
    } else {
      return (
        <React.Fragment>
          <Router>
            <Switch>
              <Route exact path="/dashboard" component={Dashboard} />
              <Route path="/dashboard/userinfo" component={UserInfo} />
              <Route path="/dashboard/inventory/" component={Inventory} />
              <Route path="/dashboard/friends/" component={Friends} />
              <Route exact path="*" component={Dashboard} />
            </Switch>
          </Router>
        </React.Fragment>
      )
    }
  }
}



export default App;
