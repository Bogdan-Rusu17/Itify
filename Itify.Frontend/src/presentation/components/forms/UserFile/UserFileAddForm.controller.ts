import { UserFileAddFormController, UserFileAddFormModel } from "./UserFileAddForm.types";
import { yupResolver } from "@hookform/resolvers/yup";
import { useIntl } from "react-intl";
import * as yup from "yup";
import { useForm } from "react-hook-form";
import { useQueryClient } from "@tanstack/react-query";
import {useAddUserFile} from "@infrastructure/apis/api-management";
import { useCallback } from "react";

/**
 * Create a hook to get the validation schema.
 */
const useInitUserFileAddForm = () => {
    const { formatMessage } = useIntl();

    const schema = yup.object().shape({
        file: yup.mixed<File>() // For files the schema used should be mixed.
            .required(formatMessage(
                { id: "globals.validations.requiredField" },
                {
                    fieldName: formatMessage({
                        id: "globals.file",
                    }),
                }))
    });

    const resolver = yupResolver(schema);

    return { defaultValues: {}, resolver };
}

/**
 * Create a controller hook for the form and return any data that is necessary for the form.
 */
export const useUserFileAddFormController = (onSubmit?: () => void): UserFileAddFormController => {
    const { defaultValues, resolver } = useInitUserFileAddForm();
    const { mutateAsync: add, status } = useAddUserFile();
    const queryClient = useQueryClient();
    const submit = useCallback((data: UserFileAddFormModel) => // Create a submit callback to send the form data to the backend.
        add(data).then(() => {
            if (onSubmit) {
                onSubmit();
            }
        }), [add, queryClient]);

    const {
        register,
        handleSubmit,
        watch,
        setValue,
        formState: { errors }
    } = useForm<UserFileAddFormModel>({ // Use the useForm hook to get callbacks and variables to work with the form.
        defaultValues, // Initialize the form with the default values.
        resolver // Add the validation resolver.
    });

    const setFile = useCallback((file: File) => { // The file will be added via a button so create a callback to set the file value.
        setValue("file", file, {
            shouldValidate: true
        });
    }, [setValue]);

    return {
        actions: { // Return any callbacks needed to interact with the form.
            handleSubmit, // Add the form submit handle.
            submit, // Add the submit handle that needs to be passed to the submit handle.
            register, // Add the variable register to bind the form fields in the UI with the form variables.
            watch,  // Add a watch on the variables, this function can be used to watch changes on variables if it is needed in some locations.
            setFile
        },
        computed: {
            isSubmitting: status === "pending" // Return if the form is still submitting or nit.
        },
        state: {
            errors // Return what errors have occurred when validating the form input.
        }
    }
}