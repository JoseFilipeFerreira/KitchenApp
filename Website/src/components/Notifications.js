import React from "react";
import axios from "axios";

export default class Notifications extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      requests: {},
      expired: [],
      inventories: {},
      number: 0,
    };
  }

  getInventories = () => {
    let token = localStorage.getItem("auth");
    axios
      .get(
        "http://localhost:1331/inventory/all",
        {
          headers: { auth: token },
        },
        { withCredentials: true }
      )
      .then((response) => {
        this.setState({
          inventories: response.data,
        });
        this.getExpiredItems();
      })
      .catch((error) => {
        console.log(error);
      });

    if (localStorage.getItem("auth") != null) {
      return true;
    } else {
      return false;
    }
  };

  getRequests = () => {
    let token = localStorage.getItem("auth");
    axios
      .get(
        "http://localhost:1331/user/friends/pending",
        {
          headers: { auth: token },
        },
        { withCredentials: true }
      )
      .then((response) => {
        this.setState({
          requests: response.data,
        });
      })
      .catch((error) => {
        console.log(error);
      });

    if (localStorage.getItem("auth") != null) {
      return true;
    } else {
      return false;
    }
  };

  getExpiredItems = () => {
    let token = localStorage.getItem("auth");
    axios
      .get(
        "http://localhost:1331/inventory/expired",
        {
          headers: { auth: token },
        },
        { withCredentials: true }
      )
      .then((response) => {
        this.setState({
          expired: response.data,
        });
        let number =
          Object.keys(this.state.requests).length +
          Object.keys(this.state.expired).length;
        this.setState({
          number: number,
        });
        if (this.state.number > 99)
          this.setState({
            number: "99+",
          });
      })
      .catch((error) => {
        console.log(error);
      });
  };

  showFriendNotifications = () => {
    let json = this.state.requests;
    if (Object.keys(json).length) {
      return (
        <li>
          <a href="/dashboard/friends">
            <div id="topic-friends">
              <span
                className="edit-button"
                role="img"
                aria-label="jsx-a11y/aria-proptypes"
              >
                👥
              </span>{" "}
              Friends
            </div>
            <div id="desc">
              You have {Object.keys(json).length} friend requests.
            </div>
          </a>
        </li>
      );
    } else {
      return null;
    }
  };

  showInventoryNotifications = () => {
    let products = [],
      uid = [],
      names = [];
    let json = this.state.expired;
    for (let x in json) {
      products.push(x);
      let prodinfo = json[x];
      names.push(prodinfo["name"]);
      uid.push(prodinfo["guid"]);
    }

    if (Object.keys(json).length) {
      return products.map((request, index) => {
        return (
          <li key={uid[index] + products[index]}>
            <a href={"/dashboard/inventory/" + uid[index]}>
              <div id="topic-inventory">
                <span
                  className="edit-button"
                  role="img"
                  aria-label="jsx-a11y/aria-proptypes"
                >
                  🥗
                </span>{" "}
                Inventory {names[index]}
              </div>
              <div id="desc">
                <i>
                  <u>{products[index]}</u>
                </i>{" "}
                expired/is expiring in less than 30 days.
              </div>
            </a>
          </li>
        );
      });
    } else {
      return null;
    }
  };

  expandNotifications() {
    if (
      document.getElementById("notificationMenu").className ===
      "notificationMenu"
    ) {
      document.getElementById("notificationMenu").className =
        "notificationMenuHidden";
    } else {
      document.getElementById("notificationMenu").className =
        "notificationMenu";
    }
  }

  componentDidMount() {
    this.getRequests();
    this.getInventories();
  }

  render() {
    return (
      <div>
        <section className="search-and-user">
          <div className="admin-profile">
            <span className="greeting">Hello {this.props.name}</span>
            <div className="notifications" onClick={this.expandNotifications}>
              <span className="badge">{this.state.number}</span>
              <svg>
                <use href="#users"></use>
              </svg>
            </div>
          </div>
        </section>
        <div className="notificationMenuHidden" id="notificationMenu">
          <div id="title">
            <span
              className="edit-button"
              role="img"
              aria-label="jsx-a11y/aria-proptypes"
            >
              🔔
            </span>{" "}
            Notifications
          </div>
          <ul>
            {this.showFriendNotifications()}
            {this.showInventoryNotifications()}
          </ul>
        </div>
      </div>
    );
  }
}
