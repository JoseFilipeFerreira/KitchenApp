import React, { Component } from "react";
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";
import Login from './login'
import Register from './register'
import InventoryMenu from './inventorymenu'
import UserInfo from './userinfo'

class App extends Component {
  render() {
    return (
      <React.Fragment>
        <Router>
          <Switch>
            <Route exact path="/" component={Login}/>
            <Route path="/register" component={Register}/>
            <Route path="/inventories" component={InventoryMenu}/>
            <Route path="/user" component={UserInfo}/>
          </Switch>
        </Router>
      </React.Fragment>
    )
  }
}



export default App;
