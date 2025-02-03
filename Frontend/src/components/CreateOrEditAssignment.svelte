<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
    import { CollaborationType, GradingTypeEnum, type NewAssignmentField } from 'src/types';
	import CustomButton from './CustomButton.svelte';
	import Save from 'lucide-svelte/icons/save';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
    import * as Table from '$lib/components/ui/table';
	import Checkbox from 'src/lib/components/ui/checkbox/checkbox.svelte';
	import TableCell from 'src/lib/components/ui/table/table-cell.svelte';
    import { Combobox } from 'bits-ui';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import { onMount } from 'svelte';

    let items: string[] = [
        'String',
        'Integer',
        'Boolean',
        'File',
    ];

    function addField(){
        fields.push({name: "", type: "String"})
        fields = fields
    }

    function toUTC(date: string){
        if (date != ""){
            const dateUTC = new Date(date);
            return dateUTC.toISOString()
        }
        return date
    }

    function removeField(field: NewAssignmentField){
        const index = fields.indexOf(field, 0);
        if (index > -1) {
            fields.splice(index, 1);
            fields = fields
        }     
    }
    $: fields

    export let asssignmentName: string = "",
        description: string = "",
        dueDate: string = "",
        collaberationType: CollaborationType = CollaborationType.Teams,
        draft: boolean = false,
        mandatory: boolean = false,
        fields: NewAssignmentField[] = [],
        redirect: string,
        edit: boolean = false,
        gradingType: GradingTypeEnum = GradingTypeEnum.NoGrading,
        maxPoints: number = 0,
        submitFunction: (
            assignmentName: string,
            dueDate: string,
            collaborationType: CollaborationType,
            draft: boolean,
            mandatory: boolean,
            fields: NewAssignmentField[],
            description: string,
            gradingType: GradingTypeEnum,
            maxPoints: number) => void;

    onMount(() => {
        if(dueDate != ""){
            const date = new Date(dueDate)
            dueDate = `${date.getFullYear()}-${String(date.getUTCMonth() + 1).padStart(2,'0')}-${String(date.getUTCDate()).padStart(2,'0')}`
        }
    })
</script>

<form method="POST" on:submit|preventDefault={() => submitFunction(asssignmentName, toUTC(dueDate), collaberationType, draft, mandatory, fields, description, gradingType, maxPoints)}>
    <Card.Root class="m-auto mt-16 w-[700px] overflow-hidden p-0">
        <Card.Header class="mb-6 flex flex-row items-center justify-between">
            <div class="flex items-center">
                <img
                    width="48"
                    height="48"
                    src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
                    alt="knowledge-sharing"
                />
                <Card.Title class="ml-4 text-3xl">Create a new Assignment</Card.Title>
            </div>
            <div class="flex items-center">
                <label class="pr-4"for="draftCheckbox">Draft</label>
                <Checkbox bind:checked={draft} id="draftCheckbox" on:click={() => {console.log(draft)}}/>
            </div>
            <div class="flex items-center">
                <label class="pr-4"for="draftCheckbox">Mandatory</label>
                <Checkbox bind:checked={mandatory} id="mandatoryCheckbox"/>
            </div>
        </Card.Header>
        <Card.Content class="p-0">
            <Table.Root class="w-full table-fixed border-collapse">
                <Separator/>
                <Table.Body >
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="assignmentName" class="text-lg w-1/5 pl-5 font-bold self-center">Assignment Name:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <input id="assignmentName" class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={asssignmentName}/>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="assignmentDescription" class="text-lg w-1/5 pl-5 font-bold self-center">Description:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <textarea id="assignmentDescription" class="overflow-hidden border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={description}/>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="dueDate" class="text-lg pl-5 w-1/5 font-bold self-center">Due Date:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <input id="dueDate" type="date" class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={dueDate}/>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="courseSemester"class="text-lg w-1/5 pl-5 space-y-5 font-bold"> Collaberation Type:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <Table.Cell>
                            <div class="ml-4 mr-4">
                                <div class="w-1/2">
                                    <input id="fall" type="radio" class="form-radio" value={CollaborationType.Individual} bind:group={collaberationType}>
                                    <span class="ml-2  text-lg">Individual</span>
                                </div>
                                <div class="w-1/2">        
                                    <input id="spring" type="radio" class="form-radio" value={CollaborationType.Teams} bind:group={collaberationType}>
                                    <span class="ml-2  text-lg">Teams</span>
                                </div>
                            </div>
                    </Table.Cell>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="gradingType"class="text-lg w-1/5 pl-5 space-y-5 font-bold"> Grading Type:</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <Table.Cell>
                            <div class="ml-4 mr-4">
                                <div class="w-1/2">
                                    <input id="fall" type="radio" class="form-radio" value={GradingTypeEnum.NoGrading} bind:group={gradingType}>
                                    <span class="ml-2  text-lg">None</span>
                                </div>
                                <div class="w-1/2">        
                                    <input id="spring" type="radio" class="form-radio" value={GradingTypeEnum.ApprovalGrading} bind:group={gradingType}>
                                    <span class="ml-2  text-lg">Approval</span>
                                </div>
                                <div class="w-1/2">        
                                    <input id="spring" type="radio" class="form-radio" value={GradingTypeEnum.LetterGrading} bind:group={gradingType}>
                                    <span class="ml-2  text-lg">Letter</span>
                                </div>
                                <div class="w-1/2">        
                                    <input id="spring" type="radio" class="form-radio" value={GradingTypeEnum.PointsGrading} bind:group={gradingType}>
                                    <span class="ml-2  text-lg">Points</span>
                                </div>
                            </div>
                        </Table.Cell>
                    </Table.Row>
                    {#if gradingType == GradingTypeEnum.PointsGrading}
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="maxPoints" class="text-lg pl-5 space-y-5 font-bold">Max Points</label>
                        </div>
                    </Table.Row>
                    <Table.Row class="border-none">
                        <div class="ml-4 mr-4">
                            <input type="number" class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={maxPoints}/>
                        </div>
                    </Table.Row>
                    {/if}
                {#if !edit}
                    <Table.Row class="border-none">
                        <div class="pt-5">
                            <label for="Fields"class="text-lg w-1/5 pl-5 space-y-5 font-bold"> Fields:</label>
                        </div>
                    </Table.Row>
                {#each fields as field}
                    <Table.Row class="border-none">
                        <Table.Root>
                            <Table.Row>
                                <TableCell class="w-2/6">
                                    <div class="pt-5">
                                        <label for="fieldName"class="text-lg w-1/5 pl-5 space-y-5 font-bold">Name</label>
                                        <input class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg" bind:value={field.name}/>
                                    </div>
                                </TableCell>
                                <TableCell class="w-2/6">
                                    <div class="pt-5">
                                        <label for="typeSelector"class="text-lg pl-5 space-y-5 font-bold">Type</label>
                                        <div>
                                            <Combobox.Root>
                                                <Combobox.Input
                                                    class="border-2 border-gray-800 rounded-lg p-1 w-full text-lg"
                                                    bind:value={field.type} 
                                                    placeholder={"Select Item"}/>
                                                <Combobox.Content class="w-full rounded-xl border border-muted bg-background px-1 py-3 shadow-popover outline-none">
                                                    {#each items as type}
                                                    <Combobox.Item 
                                                        value={type} 
                                                        on:click={() => {field.type = type}}
                                                        class="flex h-10 w-full select-none items-center rounded-button py-3 pl-5 pr-1.5 text-sm capitalize outline-none transition-all data-[highlighted]:bg-muted">
                                                        {type}
                                                    </Combobox.Item>
                                                    {/each}
                                                </Combobox.Content>
                                            </Combobox.Root>
                                        </div>
                                    </div>
                                </TableCell>
                                <TableCell class="w-1/6">
                                    <div class="pt-5">
                                        <label for="deleteFieldButton" class="text-lg pl-5 space-y-5 font-bold"></label>
                                        <CustomButton color="red" on:click={() => removeField(field)}>
                                            <Trash_2 size="20" />
                                        </CustomButton>
                                    </div>
                                </TableCell>
                            </Table.Row>
                        </Table.Root>
                    </Table.Row>
                {/each}
                    <Table.Row class="border-none">
                        <div class="pt-5 pb-5 flex items-center justify-center">
                            <CustomButton on:click={addField} color={"green"}>
                                <p>Add new Field</p>
                            </CustomButton>
                        </div>
                    </Table.Row>
                {/if}
                </Table.Body>
            </Table.Root>
        </Card.Content>
        <Card.Footer class="flex flex-col items-middle p-0">
            <Separator/>
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