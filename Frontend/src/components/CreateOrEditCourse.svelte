<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
    import { Semester } from 'src/types';
	import CustomButton from './CustomButton.svelte';
	import Save from 'lucide-svelte/icons/save';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
    import * as Table from '$lib/components/ui/table';

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
            <Table.Root>
                <Table.Body >
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="courseName" class="text-lg w-1/5 pl-5 font-bold self-center">Course Name:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <input id="courseName" class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={courseName}/>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="courseCode" class="text-lg pl-5 w-1/5 font-bold self-center">Course Code:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <input id="courseCode" class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={courseCode}/>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="courseYear" class="text-lg pl-5 w-1/5 font-bold self-center">Year:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <input id="courseYear" type="number" min="2000" max="2099" class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={year}/>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="courseSemester"class="text-lg w-1/5 pl-5 space-y-5 font-bold"> Course Code:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <div class="w-1/2">
                                <input id="fall" type="radio" class="form-radio" value={Semester.Autumn} bind:group={semester}>
                                <span class="ml-2  text-lg">Autumn</span>
                            </div>
                            <div class="w-1/2">
                                <input id="spring" type="radio" class="form-radio" value={Semester.Spring} bind:group={semester}>
                                <span class="ml-2  text-lg">Spring</span>
                            </div>
                        </div>
                    </Table.Row>
                </Table.Body>
            </Table.Root>
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