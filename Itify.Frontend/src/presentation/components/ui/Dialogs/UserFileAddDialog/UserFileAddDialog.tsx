import { Button, Dialog, DialogContent, DialogTitle } from "@mui/material";
import { useUserFileAddDialogController } from "./UserFileAddDialog.controller";
import { useIntl } from "react-intl";
import { UserFileAddForm } from "@presentation/components/forms/UserFile/UserFileAddForm";

/**
 * This component wraps the user file add form into a modal dialog.
 */
export const UserFileAddDialog = () => {
  const { open, close, isOpen } = useUserFileAddDialogController();
  const { formatMessage } = useIntl();

  return <div>
    <Button variant="outlined" onClick={open}>
      {formatMessage({ id: "labels.addUserFile" })}
    </Button>
    <Dialog
      open={isOpen}
      onClose={close}>
      <DialogTitle>
        {formatMessage({ id: "labels.addUserFile" })}
      </DialogTitle>
      <DialogContent>
        <UserFileAddForm onSubmit={close} />
      </DialogContent>
    </Dialog>
  </div>
};