import React, { Component } from "react";
import { Link } from "react-router-dom";
import axios from "axios";
import "./style.css";

class Login extends Component {

  constructor(props) {
    super(props);

    this.state = {
      email: "",
      password: "",
    };
  }

  changeHandler = (e) => {
    this.setState({ [e.target.name]: e.target.value });
  };

  validateEmail = (e) => {
    if (/^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(e) && e !== "") {
      return true;
    }

    if (e === '') {
      alert("Empty email address");
    } else {
      alert("Invalid Email");
    }
    document.loginForm.email.focus();
    return false;
  };

  validatePassword = (e) => {
    if (8 <= e.length && e.length <= 16 && e !== '') {
      return true;
    }
    if (e === '') {
      alert("Empty password");
    } else {
      alert("Invalid password");
    }
    document.loginForm.password.focus();

    return false;
  };

  submitHandler = (e) => {
    e.preventDefault();

    const form = new FormData();

    let username = this.state.email;
    let passwd = this.state.password;

    if (this.validateEmail(username) && this.validatePassword(passwd)) {
      form.append("username", username);
      form.append("passwd", passwd);

      axios
        .post("http://localhost:1331/login", form, {
          headers: { "Content-Type": "multipart/form-data" }, withCredentials: true,
        })
        .then((response) => {
          /* save this token inside localStorage */
          const token = response.headers['auth'];
          localStorage.setItem('auth', token);
          this.props.history.push('/dashboard');
        })
        .catch((error) => {
          alert('Email ou password errada.');
        });
    } else {
      document.loginForm.email.value = "";
      document.loginForm.password.value = ""
    }
  };

  render() {
    const { email, password } = this.state;

    return (
      <form
        id='loginForm'
        name="loginForm"
        className="login-box"
        onSubmit={this.submitHandler}
      >
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
