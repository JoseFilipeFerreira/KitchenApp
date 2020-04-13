import React, { Component } from "react";
import { Link } from "react-router-dom";
import axios from "axios";
import "./userStyle.css";

const initialState = {
  email: "asdas",
  name: "asda",
  phone: "asda",
  password: "asd",
  new_email: "",
  new_password: "",
};


class UserInfo extends Component {
  constructor(props) {
    super(props);

    this.state = initialState;
  }

  changeHandler = (e) => {
    this.setState({ [e.target.name]: e.target.value });
  };

  validateEmail = (e) => {
    if (/^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(e) && e !== "") {
      return true;
    }
    if (e === "") {
      alert("Empty email address");
    } else {
      alert("Invalid Email");
    }
    document.loginForm.new_email.focus();
    return false;
  };

  validatePassword = (e) => {
    if (8 <= e.length && e.length <= 16 && e !== "") {
      return true;
    }
    if (e === "") {
      alert("Empty password");
    } else {
      alert("Invalid password");
    }
    document.loginForm.new_password.focus();

    return false;
  };

  submitHandler = (e) => {
    e.preventDefault();
    console.log(this.state);

    const form = new FormData();

    let username = this.state.new_email;
    let passwd = this.state.new_password;
    let change = 0;

    /** Validate if email is not empty so it knows a request should be send*/
    if (username !== '' && this.validateEmail(username)) {
            form.append("username", username);
            change = 1;
    }

    /** Validate if password is not empty so it knows a request should be send*/
    if (passwd !== '' && this.validatePassword(passwd)) {
        form.append("passwd", passwd);
        change = 1;
    }

    /** If variable change is 1 it sends a request */
    if (change === 1) {
    
        /** Request creation */
      axios
        .post("https://jsonplaceholder.typicode.com/posts", form, {
          headers: { "Content-Type": "multipart/form-data" },
        })
        .then((response) => {
          console.log(response);
          /** Good response from server */
          /** Change old email and old password*/
          this.setState({
            email: username,
            password: passwd,
          });

          const { token } = response.headers["set-cookie"];
          console.log(response.headers["set-cookie"]);
          /* save this token inside localStorage */
          localStorage.setItem("token", token);
        })
        .catch((error) => {
            /** Bad response from server */
          console.log(this.state.email);
        });
    } else {
        /** Clear fields */
      document.loginForm.new_email.value = "";
      document.loginForm.new_password.value = "";
      this.setState(initialState);
    }
  };

  render() {
    const { new_email, new_password } = this.state;

    return (
      <form
        id="loginForm"
        name="loginForm"
        className="login-box"
        onSubmit={this.submitHandler}
      >
        <div className="logo">Kitchen App</div>
        <div className="user-info" onLoad="addInfo">
          <div id="email" className="info">
            <b>✉️ Email: </b>
            {this.state.email}
          </div>
          <div id="name" className="info">
            <b>🧑 Name: </b>
            {this.state.name}
          </div>
          <div id="phone" className="info">
            <b>📞 Phone: </b>
            {this.state.phone}
          </div>
        </div>
        <div className="login-text">Email</div>
        <input
          className="field"
          type="text"
          name="new_email"
          value={new_email}
          onChange={this.changeHandler}
        ></input>
        <div className="login-text">Password</div>
        <input
          className="field"
          type="password"
          name="new_password"
          value={new_password}
          onChange={this.changeHandler}
        ></input>
        <input className="change-button" type="submit" value="Change"></input>
        <div className="buttons">
          <Link to="/inventories">
            <input className="login-button" type="button" value="Back"></input>
          </Link>
        </div>
      </form>
    );
  }
}

export default UserInfo;
