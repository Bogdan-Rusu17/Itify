export type ProfileState = {
    loggedIn: boolean;
    token: string | null;
    userId: string | null;
    name: string | null;
    email: string | null;
    exp: number | null;
};
