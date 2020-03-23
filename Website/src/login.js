import React, { Component } from "react";
import { Link } from "react-router-dom";
import axios from "axios";
import "./style.css";

class Login extends Component {
  constructor(props) {
    super(props);

    this.state = {
      email: "",
      password: ""
    };
  }

  changeHandler = e => {
    this.setState({ [e.target.name]: e.target.value });
  };

  submitHandler = e => {
    e.preventDefault();
    console.log(this.state);
    
    const form = new FormData();

    let username = this.state.email;
    let passwd = this.state.password;
    
    form.append('username', username);
    form.append('passwd', passwd);

    axios.post('https://jsonplaceholder.typicode.com/posts', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
      .then(response => {
        console.log(response)
      })
      .catch(error => {
        console.log(error)
      });
  };

  render() {
    const { email, password } = this.state;
    return (
      <form className="login-box" onSubmit={this.submitHandler}>
        <div className="logo">Kitchen App</div>
        <div className="login-text">Email</div>
        <input 
          className="field" 
          type="text" 
          name="email" 
          value={email}
          onChange={this.changeHandler}
        ></input>
        <div className="login-text">Password</div>
        <input 
          className="field" 
          type="password" 
          name="password" 
          value={password}
          onChange={this.changeHandler}
        ></input>
        <div className="buttons">
          <input className="login-button" type="submit" value="Login"></input>
          <Link to="/register">
            <input
              className="register-button"
              type="button"
              value="Register"
            ></input>
          </Link>
        </div>
      </form>
    );
  }
}

export default Login;
