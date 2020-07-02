import React from "react";
import Swal from "sweetalert2";
import axios from "axios";

export default class FriendsPage extends React.Component {
  acceptFriend = (email) => {
    let token = localStorage.getItem("auth");
    let form = new FormData();
    form.append("friend", email);
    axios
      .post("http://localhost:1331/user/friends/accept", form, {
        headers: { "Content-Type": "multipart/form-data", auth: token },
        withCredentials: true,
      })
      .then((response) => {
        /* save this token inside localStorage */
        const token = response.headers["auth"];
        localStorage.setItem("auth", token);
        this.props.handler()
      })
      .catch((error) => {
        console.log(error);
      });
  };

  declineFriend = (email) => {
    let token = localStorage.getItem("auth");
    let form = new FormData();
    form.append("friend", email);
    axios
      .post("http://localhost:1331/user/friends/remove", form, {
        headers: { "Content-Type": "multipart/form-data", auth: token },
        withCredentials: true,
      })
      .then((response) => {
        /* save this token inside localStorage */
        const token = response.headers["auth"];
        localStorage.setItem("auth", token);
        this.props.handler()
      })
      .catch((error) => {
        console.log(error);
      });
  };

  addFriend = async () => {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    const { value: name } = await Swal.fire({
      title: "Enter user email",
      input: "email",
      inputPlaceholder: "Enter email",
      inputValidator: (value) => {
        if (!value) {
          return "Invalid Name";
        } else {
          form.append("friend", value);

          axios
            .post("http://localhost:1331/user/friends/add", form, {
              headers: { "Content-Type": "multipart/form-data", auth: token },
              withCredentials: true,
            })
            .then((response) => {
              /* save this token inside localStorage */
              const token = response.headers["auth"];
              localStorage.setItem("auth", token);
              this.props.handler()
            })
            .catch((error) => {
              console.log(error);
            });
        }
      },
    });
  };

  showRequests = function () {
    let emails = [],
      names = [];
    let json = this.props.requests;
    for (let x in json) {
      emails.push(x);
      names.push(json[x]);
    }
    if (Object.keys(json).length) {
      return emails.map((request, index) => {
        return (
          <tr key={request + names[index]}>
            <td key={names[index]}>{names[index]}</td>
            <td key={request}>{request}</td>
            <td className="table-edit" key={"accept" + names[index]}>
              <span
                onClick={() => {
                  this.acceptFriend(request);
                }}
              >
                ✔️
              </span>
            </td>
            <td className="table-edit" key={"decline" + names[index]}>
              <span
                onClick={() => {
                  this.declineFriend(request);
                }}
              >
                ❌
              </span>
            </td>
          </tr>
        );
      });
    } else {
      return null;
    }
  };

  showSent = function () {
    let emails = [],
      names = [];
    let json = this.props.sent;
    for (let x in json) {
      emails.push(x);
      names.push(json[x]);
    }
    if (Object.keys(json).length) {
      return emails.map((request, index) => {
        return (
          <tr key={request + names[index]}>
            <td key={names[index]}>{names[index]}</td>
            <td key={request}>{request}</td>
            <td className="table-edit" key={"decline" + names[index]}>
              <span
                onClick={() => {
                  this.declineFriend(request);
                }}
              >
                ❌
              </span>
            </td>
          </tr>
        );
      });
    } else {
      return null;
    }
  };

  showFriends = function () {
    let emails = [],
      names = [];
    let json = this.props.friends;
    for (let x in json) {
      emails.push(x);
      names.push(json[x]);
    }
    if (Object.keys(json).length) {
      return emails.map((request, index) => {
        return (
          <tr key={request + names[index]}>
            <td key={names[index]}>{names[index]}</td>
            <td key={request}>{request}</td>
            <td className="table-edit" key={"decline" + names[index]}>
              <span
                onClick={() => {
                  this.declineFriend(request);
                }}
              >
                ❌
              </span>
            </td>
          </tr>
        );
      });
    } else {
      return null;
    }
  };

  render() {
    return (
      <section className="grid">
        <article className="inventories">
          <div className="inventories-text">Friend Requests</div>
          <table id="inventoryList">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th></th>
                <th></th>
              </tr>
            </thead>
            <tbody>{this.showRequests()}</tbody>
          </table>
        </article>
        <article className="inventories">
          <div className="inventories-text">Sent Requests</div>
          <table id="inventoryList">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th></th>
              </tr>
            </thead>
            <tbody>{this.showSent()}</tbody>
          </table>
        </article>
        <article className="inventories">
          <div className="inventories-text">Friends</div>
          <table id="inventoryList">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th></th>
              </tr>
            </thead>
            <tbody>{this.showFriends()}</tbody>
          </table>
          <div className="inventory-button">
            <input
              className="create-button"
              type="button"
              value="Add Friend"
              onClick={this.addFriend}
            ></input>
          </div>
        </article>
      </section>
    );
  }
}
