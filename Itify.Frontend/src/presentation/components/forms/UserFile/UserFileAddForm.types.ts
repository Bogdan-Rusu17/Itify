import { FormController } from "../FormController";
import {
    UseFormHandleSubmit,
    UseFormRegister,
    FieldErrorsImpl,
    DeepRequired,
    UseFormWatch
} from "react-hook-form";

export type UserFileAddFormModel = {
    description?: string;
    file: File
};

export type UserFileAddFormState = {
    errors: FieldErrorsImpl<DeepRequired<UserFileAddFormModel>>;
};

export type UserFileAddFormActions = {
    register: UseFormRegister<UserFileAddFormModel>;
    watch: UseFormWatch<UserFileAddFormModel>;
    handleSubmit: UseFormHandleSubmit<UserFileAddFormModel>;
    submit: (body: UserFileAddFormModel) => void;
    setFile: (file: File) => void;
};
export type UserFileAddFormComputed = {
    isSubmitting: boolean
};

export type UserFileAddFormController = FormController<UserFileAddFormState, UserFileAddFormActions, UserFileAddFormComputed>;