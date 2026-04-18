import {TestFormSliceState} from "@application/state-slices/test-form/testFormSlice.types";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {TestFormModel} from "@presentation/components/forms/TestForm/testForm.types";

const getInitialState = (): TestFormSliceState => {
    return {
        form: null
    }
}

const testFormSlice = createSlice({
   name: "testForm",
   initialState: getInitialState,
   reducers: {
       updateForm: (_: TestFormSliceState, message: PayloadAction<TestFormModel>) => {
            return {
                form: message.payload
            }
       }
   }
});

export const {
    updateForm
} = testFormSlice.actions;

export const testFormReducer = testFormSlice.reducer;