import { Component } from "react";
import { NavMenu } from "../NavMenu";

class AppHeader extends Component {

    render() {
        return (
            <NavMenu user={this.props.user} ></NavMenu>
        );
    }
}
export default AppHeader;