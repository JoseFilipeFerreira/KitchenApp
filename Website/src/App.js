import React, { Component } from "react";
import { BrowserRouter as Router, Route, Switch } from "react-router-dom";
import { Login } from './login'
import { Register } from './register'

class App extends Component {
  render() {
    return (
      <React.Fragment>
        <Router>
          <Switch>
            <Route exact path="/" component={Login}/>
            <Route path="/register" component={Register}/>
          </Switch>
        </Router>
      </React.Fragment>
    )
  }
}



export default App;
