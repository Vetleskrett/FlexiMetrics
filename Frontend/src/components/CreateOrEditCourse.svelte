<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
    import { Semester } from 'src/types';
	import CustomButton from './CustomButton.svelte';
	import Save from 'lucide-svelte/icons/save';
	import Undo_2 from 'lucide-svelte/icons/undo-2';

    export let courseName: string = "", courseCode: string = "", year: number  = 2025, semester: Semester = Semester.Autumn, redirect: string, edit: boolean = false;
    export let submitFunction: (name: string, code: string, year: number, semester: Semester) => void;
</script>

<form method="POST" on:submit|preventDefault={() => submitFunction(courseName, courseCode, year, semester)}>
    <Card.Root class="m-auto mt-16 w-[700px] overflow-hidden p-0">
        <Card.Header class="mb-6 flex flex-row items-center justify-between">
            <div class="flex items-center">
                <img
                    width="48"
                    height="48"
                    src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
                    alt="knowledge-sharing"
                />
                <Card.Title class="ml-4 text-3xl">Register a new Course</Card.Title>
            </div>

        </Card.Header>
        <Card.Content class="p-0">
            <div class="space-y-4">
                <Separator class="w-full" />
                <div class="inline-flex space-x-5 w-full item-center pl-10">
                    <label for="courseName" class="text-lg w-1/5 font-bold self-center"> Course Name:</label>
                    <input id="courseName" class="border p-3 text-lg" bind:value={courseName}/>
                </div>
                <Separator class="w-full" />
                <div class="inline-flex space-x-5 w-full item-center pl-10">
                    <label for="courseCode" class="text-lg w-1/5 font-bold self-center">Course Code:</label>
                    <input id="courseCode" class="border p-3 text-lg" bind:value={courseCode}/>
                </div>
                <Separator class="w-full" />
                <div class="inline-flex space-x-5 w-full item-center pl-10">
                    <label for="courseYear" class="text-lg w-1/5 font-bold self-center"> Year:</label>
                    <input id="courseYear" type="number" min="2000" max="2099" class="border p-3 text-lg" bind:value={year}/>
                </div>
                <Separator class="w-full" />
                <div class="inline-flex space-x-5 w-full item-center pl-10 self-center py-3">
                    <label for="courseSemester"class="text-lg w-1/5 font-bold"> Semester:</label>
                    <div>
                        <input id="fall" type="radio" class="form-radio" value={Semester.Autumn} bind:group={semester}>
                        <span class="ml-2">Autumn</span>
                    </div>
                    <div>        
                        <input id="spring" type="radio" class="form-radio" value={Semester.Spring} bind:group={semester}>
                        <span class="ml-2">Spring</span>
                    </div>
                </div>
            </div>
        </Card.Content>
        <Card.Footer class="flex flex-col items-middle p-0">
            <Separator />
            <div class="m-4 flex gap-4">
                <CustomButton color="yellow" href={redirect}>
                    <Undo_2 size="20" />
                    <p>Return</p>
                </CustomButton>
                <CustomButton color="submit">
                    <Save size="20" />
                    <p>{edit ? 'Edit' : 'Create'}</p>
                </CustomButton>
            </div>
        </Card.Footer>
    </Card.Root>
</form>