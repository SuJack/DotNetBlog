﻿var React = require('react')
var {Mixin} = require('formsy-react')
var {FormGroup} = require("react-bootstrap")

const Input = React.createClass({
    // Add the Formsy Mixin
    mixins: [Mixin],

    // setValue() will set the value of the component, which in
    // turn will validate it and the rest of the form
    changeValue(event) {
        this.setValue(event.currentTarget[this.props.type === 'checkbox' ? 'checked' : 'value']);
    },
    render() {
        // Set a specific className based on the validation
        // state of this component. showRequired() is true
        // when the value is empty and the required prop is
        // passed to the input. showError() is true when the
        // value typed is invalid
        const className = this.isValid() ? '' : 'has-error';

        let errorMessage;
        if (this.showRequired()) {
            errorMessage = this.props.requireMessage;
        }
        else {
            errorMessage = this.getErrorMessage();
        }

        return (
            <FormGroup className={className}>
                <label htmlFor={this.props.name}>{this.props.title}</label>
                <input
                    {...this.props}
                    type={this.props.type || 'text'}
                    name={this.props.name}
                    onChange={this.changeValue}
                    className="form-control"
                    value={this.getValue() }
                    checked={this.props.type === 'checkbox' && this.getValue() ? 'checked' : null}
                    />
                <span className='help-block'>{errorMessage}</span>
            </FormGroup>
        );
    }
});

Input.propTypes = {
    titie: React.PropTypes.string
}

Input.defaultProps.requireMessage = "不能为空";

module.exports = Input