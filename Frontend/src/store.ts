import { writable } from "svelte/store";
import { Role } from "./types";

export const userRole = writable<Role>(0);

