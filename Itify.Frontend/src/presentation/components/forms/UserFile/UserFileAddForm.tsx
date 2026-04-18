import {
    Button,
    CircularProgress,
    FormControl,
    FormHelperText,
    FormLabel,
    Stack,
    OutlinedInput,
} from "@mui/material";
import { FormattedMessage, useIntl } from "react-intl";
import { useUserFileAddFormController } from "./UserFileAddForm.controller";
import { isEmpty, isUndefined } from "lodash";
import { UploadButton } from "@presentation/components/ui/UploadButton";

/**
 * Here we declare the user file add form component.
 * This form may be used in modals so the onSubmit callback could close the modal on completion.
 */
export const UserFileAddForm = (props: { onSubmit?: () => void }) => {
    const { formatMessage } = useIntl();
    const { state, actions, computed } = useUserFileAddFormController(props.onSubmit);

    return <form onSubmit={actions.handleSubmit(actions.submit)}> {/* Wrap your form into a form tag and use the handle submit callback to validate the form and call the data submission. */}
        <Stack spacing={4} style={{ width: "100%" }}>
            <div className="grid grid-cols-2 gap-y-5 gap-x-5">
                <div className="col-span-1">
                    <FormControl
                        fullWidth
                        error={!isUndefined(state.errors.description)}
                    > {/* Wrap the input into a form control and use the errors to show the input invalid if needed. */}
                        <FormLabel>
                            <FormattedMessage id="globals.description" />
                        </FormLabel> {/* Add a form label to indicate what the input means. */}
                        <OutlinedInput
                            {...actions.register("description")} // Bind the form variable to the UI input.
                            placeholder={formatMessage(
                                { id: "globals.placeholders.textInput" },
                                {
                                    fieldName: formatMessage({
                                        id: "globals.description",
                                    }),
                                })}
                            autoComplete="none"
                        /> {/* Add a input like a textbox shown here. */}
                        <FormHelperText
                            hidden={isUndefined(state.errors.description)}
                        >
                            {state.errors.description?.message}
                        </FormHelperText> {/* Add a helper text that is shown then the input has a invalid value. */}
                    </FormControl>
                </div>
                <div className="col-span-1 flex justify-center items-center">
                    <FormControl
                        fullWidth
                        error={!isUndefined(state.errors.file)}
                    >
                        <FormLabel required>
                            <FormattedMessage id="globals.file" />
                        </FormLabel>
                        <UploadButton // You can add inputs via buttons and wrap them into other components to show errors.
                            onUpload={actions.setFile}
                            isLoading={computed.isSubmitting}
                            disabled={computed.isSubmitting}
                            text={formatMessage({ id: "labels.addUserFile" })}
                            acceptFileType="*/*" />
                        <FormHelperText
                            hidden={isUndefined(state.errors.file)}
                        >
                            {state.errors.file?.message}
                        </FormHelperText>
                    </FormControl>
                </div>
                <Button className="-col-end-1 col-span-1" type="submit" disabled={!isEmpty(state.errors) || computed.isSubmitting}> {/* Add a button with type submit to call the submission callback if the button is a descended of the form element. */}
                    {!computed.isSubmitting && <FormattedMessage id="globals.submit" />}
                    {computed.isSubmitting && <CircularProgress />}
                </Button>
            </div>
        </Stack>
    </form>
};